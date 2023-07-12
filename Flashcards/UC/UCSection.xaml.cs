using System.Windows.Controls;
using Flashcards.Class;

namespace Flashcards.UC
{
    /// <summary>
    /// Interaction logic for UCSection.xaml
    /// </summary>
    public partial class UCSection : UserControl
    {
        private Section sec;
        public Section Sec { get => sec;
            set {
                sec = value;
                txt_header.Text = sec.Header;
                txt_header.ToolTip = sec.Header.TranslateString("EN", "VI").EncodeTransform();
                foreach (Content cont in sec.Contents)
                {
                    if (!string.IsNullOrEmpty(cont.Cont.ToString().Trim()))
                    {
                        UCContent uc = new UCContent();
                        uc.Cont = cont;
                        stk_Content.Children.Add(uc);
                    }
                }
            }
        }

        public UCSection()
        {
            InitializeComponent();
        }

        
    }
}
