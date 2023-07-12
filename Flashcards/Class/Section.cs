using System.Collections.Generic;

namespace Flashcards.Class
{
    public class Section
    {
        private string header;
        List<Content> contents = new List<Content>();

        public string Header { get => header; set => header = value; }
        public List<Content> Contents { get => contents; set => contents = value; }

        public static bool IsNullSection(Section s)
        {
            foreach (Content c in s.Contents)
            {
                if (!string.IsNullOrEmpty(c.Cont.ToString().Trim()))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
