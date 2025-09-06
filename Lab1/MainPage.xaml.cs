using Lab1.Cipher;
using Microsoft.UI.Xaml.Controls;

namespace Lab1
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = new ViewModel();
        }
    }
}