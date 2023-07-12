using System.Collections.Generic;
using System.IO;
using System;
using System.Xml.Linq;
using System.Linq;

namespace Flashcards.Class
{
    public class Word
    {
        string thisWord;
        string type;
        string pron;
        List<Translation> translations;

        public string ThisWord { get => thisWord; set => thisWord = value; }
        public string Type { get => type; set => type = value; }
        public string Pron { get => pron; set => pron = value; }
        public List<Translation> Translations { get => translations; set => translations = value; }

        /// <summary>
        /// Save word To Library
        /// </summary>
        /// <param name="words">array of word</param>
        public static void Save(params Word[] words)
        {
            List<string> Swords = (from w in words select w.ThisWord).ToList();

            var groupWords = StringProcessing.GroupWord(Swords);

            foreach (var group in groupWords)
            {
                char fistcharacter = group.Key[0];
                string pathLibrary = Link.PathOfLibrary(fistcharacter);
                XDocument xdoc;

                if (ExistedLibrary(fistcharacter))
                {
                    xdoc = XDocument.Load(pathLibrary);
                }
                else
                {
                    xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                                                    new XElement("Library"));
                }

                foreach (var Sw in group)
                {
                    if (!ExistWord(xdoc, Sw))
                    {
                        xdoc.Root.Add(CreateXElement(words.Where(w => w.ThisWord == Sw).FirstOrDefault()));
                    }
                }

                xdoc.Save(pathLibrary);
            }
        }

        /// <summary>
        /// Create a XElement from a Word
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        public static XElement CreateXElement(Word w)
        {
            XElement result = new XElement("Word",
                                new XAttribute("ThisWord", w.ThisWord),
                                new XAttribute("Type", w.Type),
                                new XAttribute("Pron", w.Pron),
                                new XElement("Translations",
                                    Translation.CreateXElement(w.Translations.ToArray())
                                )
                );

            return result;
        }

        /// <summary>
        /// Check if a word is existed
        /// </summary>
        /// <param name="xdoc"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool ExistWord(XDocument xdoc, string word)
        {
            IEnumerable<string> sameWord = from w in xdoc.Descendants("Word")
                                           where w.Attribute("ThisWord").Value == word
                                           select word;
            if (sameWord.Count() > 0)
                return true;
            else return false;

        }

        /// <summary>
        /// Check if a library of word have been existed
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool ExistedLibrary(char name)
        {
            if (!Directory.Exists(Link.PathOfLibraryFolder))
            {
                Directory.CreateDirectory(Link.PathOfLibraryFolder);
                return false;
            }
            else {
                if (File.Exists(Link.PathOfLibrary(name)))
                    return true;
                else return false;
            }
        }

        /// <summary>
        /// Convert word to word data
        /// </summary>
        /// <param name="word"></param>
        /// <param name="RelatedWords"></param>
        /// <returns></returns>
        public static WordData ConvertToData(Word word, params Word[] RelatedWords)
        {
            WordData data = new WordData();
            string related = ConvertToRelated(RelatedWords);

            data.ThisWord = word.ThisWord;
            data.Date = DateTime.Now.Date;
            data.RelatedWord = related;

            return data;
        }

        /// <summary>
        /// Convert a list of word to a string related word
        /// </summary>
        /// <param name="RelatedWords"></param>
        /// <returns></returns>
        public static string ConvertToRelated(params Word[] RelatedWords)
        {
            string result = "";

            foreach (Word w in RelatedWords)
            {
                result += string.Format("{0},", w.ThisWord);
            }

            return result;
        }

        /// <summary>
        /// Query word
        /// </summary>
        /// <param name="Words"></param>
        /// <returns></returns>
        public static List<Word> Load(params string[] Words)
        {
            var groups = StringProcessing.GroupWord(Words.ToList());
            var result = new List<Word>();

            foreach (var group in groups)
            {
                char fistcharacter = group.Key[0];
                string pathLibrary = Link.PathOfLibrary(fistcharacter);
                XDocument xdoc;

                if (ExistedLibrary(fistcharacter))
                {
                    xdoc = XDocument.Load(pathLibrary);
                }
                else
                {
                    continue;
                }

                List<Word> words = new List<Word>();

                foreach (var Sw in group)
                {
                    XElement xWord = xdoc.Descendants("Word").Where(w => w.Attribute("ThisWord").Value == Sw).FirstOrDefault();
                    words.Add(new Word()
                    {
                        Type = xWord.Attribute("Type").Value,
                        Pron = xWord.Attribute("Pron").Value,
                        ThisWord = xWord.Attribute("ThisWord").Value,
                        Translations = Translation.GetTranslation(xWord).ToList()
                    });
                }
                result.AddRange(words);
            }
            return result;
        }

        /// <summary>
        /// create a Xdocument that contain many libraries foreach one is a library group by fistchar
        /// </summary>
        /// <param name="Swords"></param>
        /// <returns></returns>
        public static XDocument CreateDoc(params string[] Swords)
        {
            IEnumerable<string> words = StringProcessing.RemoveDuplication(Swords.ToList());
            var groups = StringProcessing.GroupWord(words.ToList());

            XDocument result = new XDocument(new XDeclaration("1.0","utf-8","yes"), new XElement("Libraries"));
            List<XElement> libraries = new List<XElement>();

            foreach (var group in groups)
            {
                string fistChar = group.Key;
                string pathToLibrary = Link.PathOfLibrary(fistChar[0]);

                XElement newlibrary = new XElement("Library",new XAttribute("FistChar",fistChar));
                IEnumerable<XElement> library = XDocument.Load(pathToLibrary).Descendants("Word");
                foreach (var w in group)
                {
                    newlibrary.Add(library.Where(p => p.Attribute("ThisWord").Value == w).First());
                }
                libraries.Add(newlibrary);
            }

            result.Root.Add(libraries);
            return result;
        }

        /// <summary>
        /// Import Library from dictionary folder
        /// </summary>
        /// <param name="DictionaryFolderPath"></param>
        public static void ImportLibrary(string DictionaryFolderPath)
        {
            string DataPath = string.Format(@"{0}\Data\data.xml", DictionaryFolderPath);

            XDocument xdoc = XDocument.Load(DataPath);
            IEnumerable<XElement> libraries = xdoc.Descendants("Library");

            foreach(var library in libraries)
            {
                string pathLibrary = Link.PathOfLibrary(library.Attribute("FistChar").Value[0]);
                IEnumerable<XElement> words = library.Descendants("Word");
                if (File.Exists(pathLibrary))
                {
                    XDocument xdoc_library = XDocument.Load(pathLibrary);
                    foreach (var w in words)
                    {
                        if (!ExistWord(xdoc_library, w.Attribute("ThisWord").Value))
                            xdoc_library.Root.Add(w);
                    }
                    xdoc_library.Save(pathLibrary);
                }
                else {
                    if (Directory.Exists(Link.PathOfLibraryFolder))
                    {
                        XDocument xdoc_library = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                                                    new XElement("Library", words));
                        xdoc_library.Save(pathLibrary);
                    }
                    else {
                        Directory.CreateDirectory(Link.PathOfLibraryFolder);
                        XDocument xdoc_library = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                                                    new XElement("Library", words));
                        xdoc_library.Save(pathLibrary);
                    }
                }
            }
        }
    }
}
