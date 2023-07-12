using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Flashcards.Class
{
    public class ListDictionary
    {
        private List<DictionaryInfo> listDictionaryInfo;
        public List<DictionaryInfo> ListDictionaryInfo { get => listDictionaryInfo; set => listDictionaryInfo = value; }

        public static ListDictionary Load()
        {
            XDocument xdoc = XDocument.Load(Link.PathtoListDictionary);
            IEnumerable<DictionaryInfo> list = xdoc.Descendants("DictionaryInfo").Select(x => new DictionaryInfo()
            {
                NumberOfWord = x.Attribute("NumberOfWord").Value,
                NameOfDictionary = x.Attribute("NameOfDictionary").Value
            });

            return new ListDictionary() { ListDictionaryInfo = list.ToList() };
        }

        public static void Save(string path, ListDictionary data)
        {
            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                                new XElement("ListDictionaryInfo",
                                    from di in data.ListDictionaryInfo
                                    select new XElement("DictionaryInfo",
                                            new XAttribute("NumberOfWord", di.NumberOfWord),
                                            new XAttribute("NameOfDictionary", di.NameOfDictionary)
                                                    )));
            xdoc.Save(path);
        }

        public static void Rename(ref ListDictionary visuallist, int index, string newname)
        {
            DictionaryInfo di = visuallist.ListDictionaryInfo[index];

            File.Move(Link.PathOfDictionary(di),
                          Link.PathOfDictionary(new DictionaryInfo() { NameOfDictionary = newname }));

            //change infomation
            di.NameOfDictionary = newname;
            visuallist.ListDictionaryInfo[index] = di;
        }

        public static void Augments(DictionaryInfo dictionaryInfo, int number)
        {
            XDocument xdoc = XDocument.Load(Link.PathtoListDictionary);

            xdoc.Root.Elements("DictionaryInfo").Where(di => di.Element("NameOfDictionary").Value == dictionaryInfo.NameOfDictionary).FirstOrDefault().SetAttributeValue("NumberOfWord", int.Parse(dictionaryInfo.NumberOfWord) + number);

            xdoc.Save(Link.PathtoListDictionary);
        }

        public static void CreateDictionary(ref ListDictionary visualListDic, string name)
        {
            DictionaryInfo di = new DictionaryInfo() { NameOfDictionary = name, NumberOfWord = "0" };
            visualListDic.ListDictionaryInfo.Add(di);

            string path = Link.PathOfDictionary(di);
            XDocument xdoc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("NewVocab")
                );

            xdoc.Save(path);
        }

        public static void DeleteDic(ref ListDictionary visualListDic, int index)
        {
            DictionaryInfo di = visualListDic.ListDictionaryInfo[index];
            string path = Link.PathOfDictionary(di);

            visualListDic.ListDictionaryInfo.RemoveAt(index);
            File.Delete(path);

        }

        public static void ExportToXMLData(DictionaryInfo di, string pathFolder, string savefilename)
        {
            XDocument xdoc = XDocument.Load(Link.PathOfDictionary(di));
            foreach (var e in xdoc.Root.Elements("Word"))
            {
                e.SetAttributeValue("Date", null);
            }

            XDocument data = Word.CreateDoc(WordData.ConvertToArrayWord(xdoc));
            Directory.CreateDirectory(pathFolder + @"\Data");
            data.Save(pathFolder + @"\Data\" + "data.xml");
            xdoc.Save(pathFolder +@"\"+ savefilename);
        }

        public static void ExportToHTML(DictionaryInfo di, string destFileName)
        {

        }

        public static int CountWordXML(string DictionaryPath)
        {
            return XDocument.Load(DictionaryPath).Descendants("Word").Count();
        }

        public static DictionaryInfo GetDictionaryInfoFromHTML(string DictionaryPath)
        {
            throw new Exception();
        }
        
        public static bool ExistedDictionary(string name, ListDictionary listdictionary)
        {
            foreach (var item in listdictionary.ListDictionaryInfo)
            {
                if (name == item.NameOfDictionary)
                {
                    return true;
                }
            }
            return false;
        }

        public static List<Tuple<string,List<double>>> GetMonthProgress()
        {
            List<Tuple<string, List<double>>> result = new List<Tuple<string, List<double>>>();

            foreach (var filepath in Directory.GetFiles(Link.PathOfDictionaryFolder))
            {
                IEnumerable<XElement> words = XDocument.Load(filepath).Descendants("Word");
                List<double> progress = new List<double>() {0,0,0,0};
                
                foreach (var w in words)
                {
                    if (w.Attributes("Date").Count() == 0)
                    {
                        continue;
                    }
                    else {
                        int days = Calendar.SubtractDate(DateTime.Parse(w.Attribute("Date").Value));
                        int weeks = days / 7;

                        if (weeks < 4)
                        {
                            progress[weeks]++;
                        }
                    }
                }
                progress.Reverse();

                Tuple<string, List<double>> dictionary = new Tuple<string, List<double>>(Path.GetFileNameWithoutExtension(filepath), progress);
                result.Add(dictionary);
            }

            return result;
        }
        public static List<Tuple<string, List<double>>> GetYearProgress()
        {
            List<Tuple<string, List<double>>> result = new List<Tuple<string, List<double>>>();

            foreach (var filepath in Directory.GetFiles(Link.PathOfDictionaryFolder))
            {
                IEnumerable<XElement> words = XDocument.Load(filepath).Descendants("Word");
                List<double> progress = new List<double>() { 0, 0, 0, 0 ,0,0,0,0,0,0,0,0};

                foreach (var w in words)
                {
                    if (w.Attributes("Date").Count() == 0)
                    {
                        continue;
                    }
                    else
                    {
                        int days = Calendar.SubtractDate(DateTime.Parse(w.Attribute("Date").Value));
                        int weeks = days / 30;

                        if (weeks < 12)
                        {
                            progress[weeks]++;
                        }
                    }
                }
                progress.Reverse();

                Tuple<string, List<double>> dictionary = new Tuple<string, List<double>>(Path.GetFileNameWithoutExtension(filepath), progress);
                result.Add(dictionary);
            }

            return result;
        }
    }
}
