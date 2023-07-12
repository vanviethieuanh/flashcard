using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace Flashcards.Class
{
    public static class FileManagement
    {
        public static FileInfo GetLatestFile(DirectoryInfo di)
        {
            FileInfo latest = di.GetFiles()[0];
            FileInfo[] files = di.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                if (File.GetLastWriteTime(files[i].FullName).Subtract(File.GetLastWriteTime(latest.FullName)).TotalSeconds > 0)
                {
                    latest = files[i];
                }
            }
            return latest;
        }

        public static void DeleteOldFile(DirectoryInfo di)
        {
            FileInfo latest = di.GetFiles()[0];
            FileInfo[] files = di.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                if (File.GetLastWriteTime(files[i].FullName).Subtract(File.GetLastWriteTime(latest.FullName)).TotalSeconds > 0)
                {
                    latest = files[i];
                }
            }
            foreach (FileInfo f in files)
            {
                if (f.FullName == latest.FullName)
                {
                }
                else
                {
                    File.Delete(f.FullName);
                }
            }
        }

        public static List<string> GetDictionaryFromFolder(string path)
        {
            List<string> Dictionary = Directory.GetFiles(path, "*.xml").ToList();
            for (int i = 0; i < Dictionary.Count; i++)
            {
                List<string> a = Regex.Split(Dictionary[i], "Dictionary").ToList();
                Dictionary[i] = a[1];
                Dictionary[i] = Dictionary[i].Substring(1);
                Dictionary[i] = Dictionary[i].Remove(Dictionary[i].Length - 4);
            }
            return Dictionary;
        }
    }
}
