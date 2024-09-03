using GalArc;
using System.Text;
using System.Text.Json;

namespace Utility
{
    class Util
    {
        public static void InsertSort(string[] pathString)
        {
            for (int i = 1; i < pathString.Length; i++)
            {
                string insrtVal = pathString[i];
                int insertIndex = i - 1;

                while (insertIndex >= 0 && string.CompareOrdinal(insrtVal, pathString[insertIndex]) < 0)//use compareordinal instead of compare to avoid culture
                {
                    string temp;
                    temp = pathString[insertIndex + 1];
                    pathString[insertIndex + 1] = pathString[insertIndex];
                    pathString[insertIndex] = temp;
                    insertIndex--;
                }
                pathString[insertIndex + 1] = insrtVal;
            }
        }
        public static byte[] ReadToAnsi(BinaryReader br, byte toThis)
        {
            //only used for continuous file names with a separator in between like '0x00',in which case we don't need to back a byte
            //if file name length is fixed,use getstring.trimend instead
            List<byte> name = new();
            byte aName;
            while ((aName = br.ReadByte()) != toThis)
            {
                name.Add(aName);
            }
            return name.ToArray();
        }
        public static string ReadToUnicode(BinaryReader br)
        {
            List<byte> name = new();
            byte[] aname = new byte[2];
            byte[] fix = new byte[2];
            while ((aname = br.ReadBytes(2))[0] != 0x00 || aname[1] != 0x00)
            {
                name.Add(aname[0]);
                name.Add(aname[1]);
            }
            return Encoding.Unicode.GetString(name.ToArray());
        }

        public static int GetFileCount_All(string folderPath)
        {
            string[] allFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
            return allFiles.Length;
        }
        public static int GetFileCount_TopOnly(string folderPath)
        {
            string[] allFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly);
            return allFiles.Length;
        }
        public static string[] GetUniqueExtension(string folderPath)
        {
            HashSet<string> uniqueExtension = new();
            DirectoryInfo d = new(folderPath);
            foreach (FileInfo file in d.GetFiles())
            {
                uniqueExtension.Add(file.Extension.Replace(".", string.Empty));
            }
            string[] ext = new string[uniqueExtension.Count];
            uniqueExtension.CopyTo(ext);
            return ext;
        }
        public static int GetNameLenSum(string[] filePath)
        {
            int sum = 0;
            foreach (string s in filePath)
            {
                sum += ArcEncoding.Encodings(932).GetBytes(Path.GetFileName(s)).Length;
            }
            return sum;
        }
        public static int GetNameLenSum(FileInfo[] fileInfo)
        {
            int sum = 0;
            foreach (FileInfo file in fileInfo)
            {
                sum += ArcEncoding.Encodings(932).GetBytes(file.Name).Length;
            }
            return sum;
        }
        public static int GetNameLenSum(IEnumerable<string> fileSet)
        {
            int sum = 0;
            foreach (string s in fileSet)
            {
                sum += ArcEncoding.Encodings(932).GetBytes(Path.GetFileName(s)).Length;
            }
            return sum;
        }

    }

    class Config
    {
        public int selEngine_SelectedIndex { get; set; }

        public static void saveConfig(int SelectedIndex)
        {
            Config config = new()
            {
                selEngine_SelectedIndex = SelectedIndex
            };
            string jsonConfig = JsonSerializer.Serialize(config);
            File.WriteAllText("config.json", jsonConfig);
        }
        public static void loadConfig()
        {
            if (!File.Exists("config.json"))
            {
                main.Main.selEngine.SelectedIndex = 0;
                return;
            }
            string jsonConfig = File.ReadAllText("config.json");
            if (!string.IsNullOrEmpty(jsonConfig))
            {
                Config? config = JsonSerializer.Deserialize<Config>(jsonConfig);
                if (config != null)
                {
                    main.Main.selEngine.SelectedIndex = config.selEngine_SelectedIndex;
                }
            }
            return;
        }


    }

}
