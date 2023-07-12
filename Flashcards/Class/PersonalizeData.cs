using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Flashcards.Class
{
    public class PersonalizeData
    {
        private string backgoundPath;
        private byte indexMainColor;
        private byte blur;

        public string BackgoundPath { get => backgoundPath; set => backgoundPath = value; }
        public byte IndexMainColor { get => indexMainColor; set => indexMainColor = value; }
        public byte Blur { get => blur; set => blur = value; }

        public static void Save(PersonalizeData data)
        {
            XDocument xdoc = new XDocument(new XDeclaration("1.0","utf-8","yes"),
                new XElement("Personalize",
                    new XAttribute("Path",data.BackgoundPath),
                    new XAttribute("Color",data.IndexMainColor),
                    new XAttribute("Blur",data.Blur)));
            xdoc.Save(Link.PathtoPersonalize);
        }

        public static void Save(string path,double indexColor,double blur)
        {
            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("Personalize",
                    new XAttribute("Path", path),
                    new XAttribute("Color", indexColor),
                    new XAttribute("Blur", blur)));
            xdoc.Save(Link.PathtoPersonalize);
        }

        public static PersonalizeData Load()
        {
            PersonalizeData result = new PersonalizeData();
            XDocument xdoc = XDocument.Load(Link.PathtoPersonalize);

            if (xdoc.Root.Attributes("Path").Count() > 0)
                result.BackgoundPath = xdoc.Root.Attribute("Path").Value;
            
            result.Blur = byte.Parse(xdoc.Root.Attribute("Blur").Value);
            result.IndexMainColor = byte.Parse(xdoc.Root.Attribute("Color").Value);

            return result;
        }
    }
}
