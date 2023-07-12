using System.Windows.Controls;
using Flashcards.Class;

namespace Flashcards.UC
{
    /// <summary>
    /// Interaction logic for UCincorrect.xaml
    /// </summary>
    public partial class UCincorrect : UserControl
    {
        public UCincorrect(Question ques)
        {
            InitializeComponent();
            txt_Ques.Text = ques.Ques;
            txt_correctAns.Text = ques.Answers[0];
            txt_inCAns.Text = ques.Answers[ques.IncorrectIndex];
        }
    }
}
