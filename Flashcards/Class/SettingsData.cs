using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Flashcards.Class
{
    public class SettingsData
    {
        List<string> idioms = new List<string>();
        List<string> quotes = new List<string>();

        public List<string> Quotes { get => quotes; set => quotes = value; }
        public List<string> Idioms { get => idioms; set => idioms = value; }

        public static void Save(SettingsData data)
        {
            string idioms = "";
            string quotes = "";
            foreach (var s in data.Idioms)
            {
                idioms += (s + ",");
            }
            foreach (var s in data.Quotes)
            {
                quotes += (s + ",");
            }

            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("SettingsData",
                    new XAttribute("Idioms", idioms),
                    new XAttribute("Quotes", quotes)));
            
            xdoc.Save(Link.PathtoSettingsData);
        }

        public static SettingsData Load()
        {
            SettingsData result = new SettingsData();

            XDocument xdoc = XDocument.Load(Link.PathtoSettingsData);
            XElement settingsData = xdoc.Root;

            result.Idioms = settingsData.Attribute("Idioms").Value.Split(',').ToList();
            result.Quotes = settingsData.Attribute("Quotes").Value.Split(',').ToList();

            result.Idioms.RemoveAt(result.Idioms.Count - 1);
            result.Quotes.RemoveAt(result.Quotes.Count - 1);

            return result;
        }
    }
}
