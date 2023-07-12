using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace Flashcards.Class
{
    public class Translation
    {
        string description;
        string[] exampleSentences;
        string trans;

        public string Description { get => description; set => description = value; }
        public string[] ExampleSentences { get => exampleSentences; set => exampleSentences = value; }
        public string Trans { get => trans; set => trans = value; }

        public static List<XElement> CreateXElement(params Translation[] translations)
        {
            List<XElement> result = new List<XElement>();

            foreach (Translation t in translations)
            {
                if (t.ExampleSentences.Length > 0)
                {
                    XElement e = new XElement("Translation",
                        new XAttribute("Description", t.Description),
                        new XAttribute("Trans", t.Trans),
                        new XElement("ExampleSentences",
                            from ex in t.ExampleSentences
                            select new XElement("Ex", ex)
                            )
                    );

                    result.Add(e);
                }
                else {
                    XElement e = new XElement("Translation",
                        new XAttribute("Description", t.Description),
                        new XAttribute("Trans", t.Trans),
                        new XElement("ExampleSentences", null)
                    );
                }

            }

            return result;
        }

        public static IEnumerable<Translation> GetTranslation(XElement word)
        {
            IEnumerable<Translation> result = from t in word.Descendants("Translation")
                                              select new Translation()
                                              {
                                                  Description = t.Attribute("Description").Value,
                                                  Trans = t.Attribute("Trans").Value,
                                                  ExampleSentences = (from ex in t.Elements("Ex") select ex.Value).ToArray()
                                              };
            return result;
        }
    }
}
