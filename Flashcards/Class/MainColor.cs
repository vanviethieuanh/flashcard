using System.Windows.Media;

namespace Flashcards.Class
{
    public class MainColor
    {
        SolidColorBrush light;
        SolidColorBrush dark;

        public SolidColorBrush Light { get => light; set => light = value; }
        public SolidColorBrush Dark { get => dark; set => dark = value; }
    }
}
