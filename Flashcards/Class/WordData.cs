using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace Flashcards.Class
{
    public class WordData
    {
        private DateTime _date;
        private string _ThisWord;
        private string relatedWord;

        public DateTime Date { get => _date; set => _date = value; }
        public string ThisWord { get => _ThisWord; set => _ThisWord = value; }
        public string RelatedWord { get => relatedWord; set => relatedWord = value; }

        /// <summary>
        /// delete a list word in a dictionary
        /// </summary>
        /// <param name="path"></param>
        /// <param name="word">list word</param>
        public static void Delete(ref DictionaryInfo di, params WordData[] words)
        {
            string path = Link.PathOfDictionary(di);
            //change information
            di.NumberOfWord = (int.Parse(di.NumberOfWord) - words.Length).ToString();

            XDocument xdoc = XDocument.Load(path);
            IEnumerable<XElement> listWord = xdoc.Root.Elements();
            foreach (var w in words)
            {
                listWord.Where(word => word.Attribute("ThisWord").Value == w.ThisWord).Remove();
            }

            xdoc.Save(path);
        }

        /// <summary>
        /// delete a list word in a dictionary
        /// </summary>
        /// <param name="path"></param>
        /// <param name="indices">array indices that you want to remove</param>
        public static void Delete(ref DictionaryInfo di, params int[] indices)
        {
            string path = Link.PathOfDictionary(di);

            //change information
            di.NumberOfWord = (int.Parse(di.NumberOfWord) - indices.Length).ToString();

            //sort descending
            Array.Sort<int>(indices,
                            new Comparison<int>(
                                    (i1, i2) => i2.CompareTo(i1)
                            ));
            //delete
            XDocument xdoc = XDocument.Load(path);
            IEnumerable<XElement> listWord = xdoc.Descendants("Word");
            foreach (int index in indices)
            {
                listWord.ElementAt(index).Remove();
            }

            xdoc.Save(path);
        }

        /// <summary>
        /// Add a list word to a dictionary
        /// </summary>
        /// <param name="path"></param>
        /// <param name="words"></param>
        public static void Add(DictionaryInfo di, params WordData[] words)
        {
            string path = Link.PathOfDictionary(di);
            //change information
            di.NumberOfWord = (int.Parse(di.NumberOfWord) + words.Length).ToString();

            XDocument xdoc = XDocument.Load(path);
            XElement root = xdoc.Root;

            foreach (WordData w in words)
            {
                XElement e;
                if (w.RelatedWord == null)
                {
                    e = new XElement("Word",
                       new XAttribute("Date", DateTime.Now.Date),
                       new XAttribute("ThisWord", w.ThisWord),
                       new XAttribute("RelatedWords","")
                       );
                }
                else {
                    e = new XElement("Word",
                     new XAttribute("Date", DateTime.Now.Date),
                     new XAttribute("ThisWord", w.ThisWord),
                     new XAttribute("RelatedWords", w.RelatedWord)
                     );
                }
                
                root.Add(e);
            }
            xdoc.Save(path);
        }

        /// <summary>
        /// Create a dictionary, if dictionary existed, this method would overwrite.
        /// </summary>
        /// <param name="path">string path to file</param>
        /// <param name="data">data is a array of Word</param>
        public static void Save(DictionaryInfo di, params WordData[] data)
        {
            string path = Link.PathOfDictionary(di);

            XDocument xdoc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("NewVocab",
                    from word in data
                    select new XElement("Word",
                        new XAttribute("Date", word.Date),
                        new XAttribute("ThisWord", word.ThisWord),
                        new XAttribute("RelatedWords",word.RelatedWord)
                        )
                ));

            xdoc.Save(path);
        }

        /// <summary>
        /// Load a existed dictionary
        /// </summary>
        /// <param name="path">path to file</param>
        /// <returns></returns>
        public static ObservableCollection<WordOfDictionary> Load(DictionaryInfo di)
        {
            string path = Link.PathOfDictionary(di);

            XDocument xdoc = XDocument.Load(path);
            IEnumerable<XElement> Newword = xdoc.Descendants("Word");

            ObservableCollection<WordOfDictionary> result = new ObservableCollection<WordOfDictionary>();
            foreach (var w in Newword)
            {
                if (w.Attributes("Date").Count()==0)
                {
                    result.Add(new WordOfDictionary()
                    {
                        PracticeDay = "Not seen",
                        FistCharacter = w.Attribute("ThisWord").Value.FirstCharacter(),
                        WordDic = w.Attribute("ThisWord").Value
                    });
                }
                else
                {
                    result.Add(new WordOfDictionary()
                    {
                        PracticeDay = Calendar.SubtractDate(DateTime.Parse(w.Attribute("Date").Value)).ToString(),
                        FistCharacter = w.Attribute("ThisWord").Value.FirstCharacter(),
                        WordDic = w.Attribute("ThisWord").Value
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Move word from a dictionary to another one
        /// </summary>
        /// <param name="list"></param>
        /// <param name="from"></param>
        /// <param name="target"></param>
        /// <param name="moveIndex"></param>
        public static void Move(ref ListDictionary list, int from, int target, params int[] moveIndex)
        {
            moveIndex.SortDecending();

            #region Change Data
            //get path

            string path_from = Link.PathOfDictionary(list.ListDictionaryInfo[from]);
            string path_target = Link.PathOfDictionary(list.ListDictionaryInfo[target]);

            //get data
            XDocument xdoc_from = XDocument.Load(path_from);
            IEnumerable<XElement> from_elements = xdoc_from.Descendants("Word");


            //move data
            XDocument xdoc_target = XDocument.Load(path_target);
            foreach (int i in moveIndex)
            {
                xdoc_target.Root.Add(from_elements.ElementAt(i));
                from_elements.ElementAt(i).Remove();
            }

            //save file
            xdoc_from.Save(path_from);
            xdoc_target.Save(path_target);
            #endregion

            #region Change Infomation
            list.ListDictionaryInfo[from].NumberOfWord = (int.Parse(list.ListDictionaryInfo[from].NumberOfWord) - moveIndex.Length).ToString();
            list.ListDictionaryInfo[target].NumberOfWord = (int.Parse(list.ListDictionaryInfo[target].NumberOfWord) + moveIndex.Length).ToString();
            #endregion
        }

        /// <summary>
        /// Turn of not seen of a word
        /// </summary>
        /// <param name="di">name of dictionary</param>
        /// <param name="word">word</param>
        public static void TurnOffNotSeen(DictionaryInfo di, string word)
        {
            XDocument xdoc = XDocument.Load(Link.PathOfDictionary(di));
            IEnumerable<XElement> elements = xdoc.Descendants("Word").Where(x => x.Attribute("ThisWord").Value == word);
            foreach (XElement x in elements)
            {
                x.SetAttributeValue("Date", DateTime.Now.Date);
            }
            xdoc.Save(Link.PathOfDictionary(di));
        }

        /// <summary>
        /// Get a fist word that "ThisWord" property == input
        /// </summary>
        /// <param name="di"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public static WordData Query(DictionaryInfo di, string word)
        {
            string path = Link.PathOfDictionary(di);

            XDocument xdoc = XDocument.Load(path);
            XElement queryElement = xdoc.Root.Elements().Where(w => w.Attribute("ThisWord").Value == word).FirstOrDefault();

            return ConvertFromXElement(queryElement);            
        }

        /// <summary>
        /// Convert Xelement word to WordData
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static WordData ConvertFromXElement(XElement word)
        {
            if (word.Attributes("Date").Count() > 0 && word.Attribute("Date").Value != "")
            {
                return new WordData()
                {
                    Date = DateTime.Parse(word.Attribute("Date").Value),
                    ThisWord = word.Attribute("ThisWord").Value,
                    RelatedWord = word.Attribute("RelatedWords").Value
                };
            }
            else {
                return new WordData()
                {
                    Date = DateTime.Now,
                    ThisWord = word.Attribute("ThisWord").Value,
                    RelatedWord = word.Attribute("RelatedWords").Value
                };
            }

        }

        /// <summary>
        /// Convert WordData to Array of String that contain all words(Including relatedword) and not duplicate
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        public static string[] ConvertToArrayWord(DictionaryInfo di)
        {
            string path = Link.PathOfDictionary(di);
            var result = new List<string>();

            foreach (var w in XDocument.Load(path).Descendants("Word"))
            {
                result.Add(w.Attribute("ThisWord").Value);
                result.AddRange(w.Attribute("RelatedWord").Value.Split(',').ToList());
            }

            StringProcessing.RemoveDuplication(result);

            return result.ToArray();
        }

        /// <summary>
        /// Convert WordData to Array of String that contain all words(Including relatedword) and not duplicate
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        public static string[] ConvertToArrayWord(XDocument xdoc)
        {
            var result = new List<string>();

            foreach (var w in xdoc.Descendants("Word"))
            {
                result.Add(w.Attribute("ThisWord").Value);
                List<string> related = w.Attribute("RelatedWords").Value.Split(',').ToList();
                if (related.Count > 0)
                    result.AddRange(related);
            }

            StringProcessing.RemoveDuplication(result);

            return result.ToArray();
        }
        
        /// <summary>
        /// Import dictionary from path dictionary folder
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pathDictionaryFolder"></param>
        public static DictionaryInfo Import(string name, string pathDictionaryFolder)
        {
            //add Info
            string pathDictionary = string.Format(@"{0}\{1}.xml",pathDictionaryFolder,System.IO.Path.GetFileNameWithoutExtension(pathDictionaryFolder));
            XDocument xdoc_dic = XDocument.Load(pathDictionary);
            int words = xdoc_dic.Descendants("Word").Count();
            //copy word data
            xdoc_dic.Save(Link.PathOfDictionary(name));

            return new DictionaryInfo()
            {
                NameOfDictionary = name,
                NumberOfWord = words.ToString()                
            };
        }
    }

    public class CurrentWord : WordData
    {
        private string[] _linkSpeak;
        public string[] LinkSpeak { get => _linkSpeak; set => _linkSpeak = value; }
        public string SourceImage { get => _sourceImage; set => _sourceImage = value; }
        public string[] Pron { get => pron; set => pron = value; }

        private string[] pron;
        private string _sourceImage;
    }
}
