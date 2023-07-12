using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Flashcards.Class
{
    public static class MainColors
    {
        public static ObservableCollection<MainColor> ListColor = new ObservableCollection<MainColor>()
        {
            new MainColor(){
                Light = (SolidColorBrush)(new BrushConverter().ConvertFrom("#2ecc71")),
                Dark = (SolidColorBrush)(new BrushConverter().ConvertFrom("#27ae60"))

            },
            new MainColor(){
                Light = (SolidColorBrush)(new BrushConverter().ConvertFrom("#1abc9c")),
                Dark = (SolidColorBrush)(new BrushConverter().ConvertFrom("#16a085"))
            },
            new MainColor(){
                Light = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3498db")),
                Dark = (SolidColorBrush)(new BrushConverter().ConvertFrom("#2980b9"))
            },
            new MainColor(){
                Light = (SolidColorBrush)(new BrushConverter().ConvertFrom("#9b59b6")),
                Dark = (SolidColorBrush)(new BrushConverter().ConvertFrom("#8e44ad"))
            },
            new MainColor(){
                Light = (SolidColorBrush)(new BrushConverter().ConvertFrom("#e74c3c")),
                Dark = (SolidColorBrush)(new BrushConverter().ConvertFrom("#c0392b"))
            },
            new MainColor(){
                Light = (SolidColorBrush)(new BrushConverter().ConvertFrom("#e67e22")),
                Dark = (SolidColorBrush)(new BrushConverter().ConvertFrom("#d35400"))
            },
            new MainColor(){
                Light = (SolidColorBrush)(new BrushConverter().ConvertFrom("#f1c40f")),
                Dark = (SolidColorBrush)(new BrushConverter().ConvertFrom("#f39c12"))
            }
        };
    }
}
