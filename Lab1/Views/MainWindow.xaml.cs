using Microsoft.UI.Xaml;

namespace Lab1
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.Content = new MainPage();
        }
    }
}