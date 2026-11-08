using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LocalQuest
{
    internal static class Config
    {
        public static void Setup()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                SetupLinux();
                return;
            }
            string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string AppDataPath = AppData + "\\LocalQuest\\";
            if (!Directory.Exists(AppDataPath))
            {
                Directory.CreateDirectory(AppDataPath);
            }
            string TempPath = AppDataPath + "Temp\\";
            if (!Directory.Exists(TempPath))
            {
                Directory.CreateDirectory(TempPath);
            }
            if (!File.Exists(AppDataPath + "EqConfig.eqcfg"))
            {
                using (StreamWriter SW = File.CreateText(AppDataPath + "EqConfig.eqcfg"))
                {
                    SW.Close();
                    SW.Dispose();
                }
            }
        }

        public static void SetupLinux()
        {
            string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string AppDataPath = AppData + "/LocalQuest/";
            if (!Directory.Exists(AppDataPath))
            {
                Directory.CreateDirectory(AppDataPath);
            }
            string TempPath = AppDataPath + "Temp/";
            if (!Directory.Exists(TempPath))
            {
                Directory.CreateDirectory(TempPath);
            }
            if (!File.Exists(AppDataPath + "EqConfig.eqcfg"))
            {
                using (StreamWriter SW = File.CreateText(AppDataPath + "EqConfig.eqcfg"))
                {
                    SW.Close();
                    SW.Dispose();
                }
            }
        }

        public static void Clear()
        {
            string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string AppDataPath = AppData + "\\LocalQuest\\";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                AppDataPath = AppDataPath.Replace("\\", "/");
            }
            if (!Directory.Exists(AppDataPath))
            {
                Directory.CreateDirectory(AppDataPath);
            }
            string TempPath = AppDataPath + "Temp\\";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                TempPath = TempPath.Replace("\\", "/");
            }
            if (!Directory.Exists(TempPath))
            {
                Directory.CreateDirectory(TempPath);
            }
            if (File.Exists(AppDataPath + "EqConfig.eqcfg"))
            {
                File.WriteAllText(AppDataPath + "EqConfig.eqcfg", "");
            }
        }

        public static string GetString(string Key)
        {
            ConfigValue Value = GetValues().FirstOrDefault(A => A.Config == Key);
            if (Value == null)
                return null;
            return Value.Value;
        }

        public static void SetString(string Key, string Value)
        {
            if (!string.IsNullOrEmpty(Value) && (Value.Contains("=") || Value.Contains("|")))
            {
                Console.WriteLine("String contains invalid characters!");
                return;
            }
            SetValue(Key, Value);
        }

        public static bool GetBool(string Key)
        {
            ConfigValue Value = GetValues().FirstOrDefault(A => A.Config == Key);
            if (Value == null)
                return false;
            return bool.Parse(Value.Value);
        }

        public static void SetBool(string Key, bool Value)
        {
            SetValue(Key, Value.ToString());
        }

        public static int? GetInt(string Key)
        {
            ConfigValue Value = GetValues().FirstOrDefault(A => A.Config == Key);
            if (Value == null)
                return null;
            if(string.IsNullOrEmpty(Value.Value))
            {
                return null;
            }
            return int.Parse(Value.Value);
        }

        public static void SetInt(string Key, int? Value)
        {
            if(Value == null)
            {
                SetValue(Key, "");
            }
            else
            {
                SetValue(Key, Value.ToString());
            }
        }

        public static bool Exists(string Key)
        {
            ConfigValue? Value = GetValues().FirstOrDefault(A => A.Config == Key);
            if (Value == null)
                return false;
            return true;
        }

        static List<ConfigValue> GetValues()
        {
            string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string AppDataPath = AppData + "\\LocalQuest\\";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                AppDataPath = AppDataPath.Replace("\\", "/");
            }
            string[] Data = File.ReadAllText(AppDataPath + "EqConfig.eqcfg").Split("|");

            if (!Data[0].Contains("="))
                return new List<ConfigValue>();

            List<ConfigValue> Values = new List<ConfigValue>();
            foreach (string Value in Data)
            {
                string[] Config = Value.Split("=");
                Values.Add(new ConfigValue()
                {
                    Config = Config[0],
                    Value = Config[1]
                });
            }
            return Values;
        }

        public static string ReadBase()
        {
            string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string AppDataPath = AppData + "\\LocalQuest\\";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                AppDataPath = AppDataPath.Replace("\\", "/");
            }
            return File.ReadAllText(AppDataPath + "EqConfig.eqcfg");
        }

        public static void Update()
        {
            string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string AppDataPath = AppData + "\\LocalQuest\\";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                AppDataPath = AppDataPath.Replace("\\", "/");
            }
            string S = File.ReadAllText(AppDataPath + "EqConfig.eqcfg");
            S = S.Replace(":", "=");
            File.WriteAllText(AppDataPath + "EqConfig.eqcfg", S);
        }

        static void SetValue(string Config, string Value)
        {
            string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string AppDataPath = AppData + "\\LocalQuest\\";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                AppDataPath = AppDataPath.Replace("\\", "/");
            }
            List<ConfigValue> Values = GetValues();
            ConfigValue ExistingValue = Values.FirstOrDefault(A => A.Config == Config);
            if (ExistingValue != null)
            {
                Values.FirstOrDefault(A => A.Config == Config).Value = Value;
            }
            else
            {
                Values.Add(new ConfigValue()
                {
                    Config = Config,
                    Value = Value
                });
            }
            string NewData = "";
            foreach (var UConfig in Values)
            {
                NewData += "|" + UConfig.Config + "=" + UConfig.Value;
            }
            NewData = NewData.Remove(0, 1);
            File.WriteAllText(AppDataPath + "EqConfig.eqcfg", NewData);
        }

        public class ConfigValue
        {
            public string Config { get; set; }
            public string Value { get; set; }
        }
    }
}
