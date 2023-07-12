using System;

namespace Flashcards.Class
{
    public static class Link
    {
        public static string PathOfDictionaryFolder = Environment.CurrentDirectory + @"\Dictionary";

        public static string PathOfLibraryFolder = Environment.CurrentDirectory + @"\Library";

        public static string PathtoListDictionary = "DictionaryInfo.xml";

        public static string PathtoPersonalize = "Personalize.xml";

        public static string PathtoSettingsData = "Settings.xml";

        public static string PathOfDictionary(DictionaryInfo di)
        {
            return string.Format(@"{0}\{1}.xml", PathOfDictionaryFolder, di.NameOfDictionary);
        }

        public static string PathOfDictionary(string name)
        {
            return string.Format(@"{0}\{1}.xml", PathOfDictionaryFolder, name);
        }

        public static string PathOfLibrary(char name)
        {
            return string.Format(@"{0}\{1}.xml", PathOfLibraryFolder, name.ToString());
        }
    }
}
