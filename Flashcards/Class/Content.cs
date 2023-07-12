using System.Text;

namespace Flashcards.Class
{
    public class Content
    {
        string header;
        StringBuilder cont = new StringBuilder();

        public string Header { get => header; set => header = value; }
        public StringBuilder Cont { get => cont; set => cont = value; }
    }
}
