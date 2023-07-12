using System.Windows.Controls;
using Flashcards.Class;
using System.ComponentModel;
using System.Windows;

namespace Flashcards.UC
{
    /// <summary>
    /// Interaction logic for UCword.xaml
    /// </summary>
    public partial class UCword : UserControl, INotifyPropertyChanged
    {
        private Word word;
        public Word Word {
            get => word;
            set
            {
                word = value;
                DataContext = Word;
                OnPropertyChanged("Word");

                stk_listTrans.Children.Clear();
                if (Word.Pron == "")
                    lbl_pron.Padding = new Thickness(0);
               
                foreach (Translation t in Word.Translations)
                {
                    UCtranslation UCtran = new UCtranslation();
                    UCtran.Translation = t;
                    stk_listTrans.Children.Add(UCtran);
                }
            }
        }
        
        public UCword()
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
