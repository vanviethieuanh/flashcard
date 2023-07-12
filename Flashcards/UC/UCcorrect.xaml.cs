using System.Windows.Controls;
using Flashcards.Class;

namespace Flashcards.UC
{
    /// <summary>
    /// Interaction logic for UCcorrect.xaml
    /// </summary>
    public partial class UCcorrect : UserControl
    {
        public UCcorrect(Question ques)
        {
            InitializeComponent();
            txt_Ques.Text = ques.Ques;
            txt_Ans.Text = ques.Answers[0];
        }
    }
}
