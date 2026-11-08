using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaDataStringEditor {

    public class EditorListItem {

        public bool IsEdit { private set; get; }
        public byte[] OriginStrBytes { private set; get; }
        public byte[] NewStrBytes { private set; get; }

        public string Text { get; set; }

        MetadataFile File;
        public int Tag;

        public EditorListItem(byte[] OriginStrBytes, MetadataFile File, int Tag) {
            this.OriginStrBytes = OriginStrBytes;
            IsEdit = false;
            this.Tag = Tag;
            this.File = File;

            Text = Encoding.UTF8.GetString(OriginStrBytes);
        }

        public void SetNewStr(string newString) {
            NewStrBytes = Encoding.UTF8.GetBytes(newString);
            IsEdit = !Equals(OriginStrBytes, NewStrBytes);

            Text = IsEdit ? newString : "";
            this.File.Update(this);
        }

        public void Discard() {
            NewStrBytes = null;
            IsEdit = false;
        }

        public bool MatchKeyWord(string keyWord) {
            return Text.Contains(keyWord, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
