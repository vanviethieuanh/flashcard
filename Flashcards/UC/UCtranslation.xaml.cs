using System.Windows.Controls;
using Flashcards.Class;
using System.ComponentModel;

namespace Flashcards.UC
{
    /// <summary>
    /// Interaction logic for UCtranslation.xaml
    /// </summary>
    public partial class UCtranslation : UserControl, INotifyPropertyChanged
    {
        Translation translation;
        public Translation Translation {
            get { return translation; }
            set
            {
                translation = value;
                DataContext = Translation;
                OnPropertyChanged("Translation");
                if (Translation.ExampleSentences.Length < 1)
                    txt_ex.Visibility = System.Windows.Visibility.Hidden;
                foreach (string s in Translation.ExampleSentences)
                {
                    Label lbl_ex = new Label();
                    lbl_ex.Content = new AccessText() {Text = s};
                    stk_ex.Children.Add(lbl_ex);
                }
            }
        }

        public UCtranslation()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string newName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(newName));
            }
        }
    }
}
