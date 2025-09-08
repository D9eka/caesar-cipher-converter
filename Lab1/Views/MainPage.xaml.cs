using Lab1.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Lab1
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = new MainViewModel(this);
        }
    }
}