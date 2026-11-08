using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LocalQuest;

namespace MetaDataStringEditor {
    public class MetadataFile : IDisposable {
        public BinaryReader reader;

        private uint stringLiteralOffset;
        private uint stringLiteralCount;
        private long DataInfoPosition;
        private uint stringLiteralDataOffset;
        private uint stringLiteralDataCount;
        private List<StringLiteral> stringLiterals = new List<StringLiteral>();
        public List<byte[]> strBytes = new List<byte[]>();
        public string CurrentFileName;

        byte[] StartingData = [];
        public MetadataFile(string fullName) {
            CurrentFileName = fullName;
            byte[] FullFile = File.ReadAllBytes(fullName);
            StartingData = FullFile;
            MemoryStream R = new(FullFile);
            reader = new BinaryReader(R);

            // Get the header
            ReadHeader();

            // Read the data
            ReadLiteral();
            ReadStrByte();

            Log.Info("Done!");
        }

        public void Update(EditorListItem Item)
        {
            if (Item.IsEdit)
                strBytes[Item.Tag] = Item.NewStrBytes;
            else
                strBytes[Item.Tag] = Item.OriginStrBytes;
        }

        private void ReadHeader() {
            Log.Info("Reading header...");
            uint vansity = reader.ReadUInt32();
            if (vansity != 0xFAB11BAF) {
                throw new Exception("标志检查不通过");
            }
            int version = reader.ReadInt32();
            Log.Info($"Version: {version}");
            stringLiteralOffset = reader.ReadUInt32();      // 列表区的位置，后面不会改了
            stringLiteralCount = reader.ReadUInt32();       // 列表区的大小，后面不会改了
            DataInfoPosition = reader.BaseStream.Position;  // 记一下当前位置，后面要用
            stringLiteralDataOffset = reader.ReadUInt32();  // 数据区的位置，可能要改
            stringLiteralDataCount = reader.ReadUInt32();   // 数据区的长度，可能要改
        }

        private void ReadLiteral() {
            Log.Info("Reading literal...");

            reader.BaseStream.Position = stringLiteralOffset;
            for (int i = 0; i < stringLiteralCount / 8; i++) {
                stringLiterals.Add(new StringLiteral {
                    Length = reader.ReadUInt32(),
                    Offset = reader.ReadUInt32()
                });
            }
        }

        private void ReadStrByte() {
            Log.Info("Reading bytes...");

            for (int i = 0; i < stringLiterals.Count; i++) {
                reader.BaseStream.Position = stringLiteralDataOffset + stringLiterals[i].Offset;
                strBytes.Add(reader.ReadBytes((int)stringLiterals[i].Length));
            }
        }

        public void WriteToNewFile(string fileName) {
            BinaryWriter writer = new BinaryWriter(File.OpenWrite(fileName));

            // 先全部复制过去
            reader.BaseStream.Position = 0;
            reader.BaseStream.CopyTo(writer.BaseStream);

            // writing literal...
            Log.Info("Writing literal...");
            writer.BaseStream.Position = stringLiteralOffset;
            uint count = 0;
            for (int i = 0; i < stringLiterals.Count; i++) {

                stringLiterals[i].Offset = count;
                stringLiterals[i].Length = (uint)strBytes[i].Length;

                writer.Write(stringLiterals[i].Length);
                writer.Write(stringLiterals[i].Offset);
                count += stringLiterals[i].Length;
            }

            // 进行一次对齐，不确定是否一定需要，但是Unity是做了，所以还是补上为好
            var tmp = (stringLiteralDataOffset + count) % 4;
            if (tmp != 0) count += 4 - tmp;

            // 检查是否够空间放置
            if (count > stringLiteralDataCount) {
                // 检查数据区后面还有没有别的数据，没有就可以直接延长数据区
                if (stringLiteralDataOffset + stringLiteralDataCount < writer.BaseStream.Length) {
                    // 原有空间不够放，也不能直接延长，所以整体挪到文件尾
                    stringLiteralDataOffset = (uint)writer.BaseStream.Length;
                }
            }
            stringLiteralDataCount = count;

            // 写入string
            Log.Info("Writing string...");
            writer.BaseStream.Position = stringLiteralDataOffset;
            for (int i = 0; i < strBytes.Count; i++) {
                writer.Write(strBytes[i]);
            }

            // 更新头部
            Log.Info("更新头部");
            writer.BaseStream.Position = DataInfoPosition;
            writer.Write(stringLiteralDataOffset);
            writer.Write(stringLiteralDataCount);

            Log.Info("更新完成");
            writer.Close();
        }

        public void WriteBackup(string fileName) {
            File.WriteAllBytes(fileName, StartingData);;
        }
        
        public void Dispose() {
            reader?.Dispose();
        }
        
        public class StringLiteral {
            public uint Length;
            public uint Offset;
        }
    }
}
