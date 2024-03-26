using System.Windows;

namespace FirefoxProfileLauncher;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainFrame.Content = new HomePage();
    }
}