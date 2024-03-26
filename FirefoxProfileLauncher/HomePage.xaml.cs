using System.Windows;
using System.Windows.Controls;

namespace FirefoxProfileLauncher;

public partial class HomePage : Page
{
    private string _firefoxBaseUrl = "C:\\Program Files\\Mozilla Firefox\\firefox.exe";
    private string _runProfileArgument = "-P";
    private string _userProfileName1 = "Pawełek";
    private string _userProfileName2 = "Ewelinka";


    public HomePage(MainWindow mainWindow)
    {
        MainWindow = mainWindow;
        InitializeComponent();
    }

    public MainWindow MainWindow { get; }

    private void Button_Click1(object sender, RoutedEventArgs e)
    {
        System.Diagnostics.Process.Start(_firefoxBaseUrl, _runProfileArgument + " " + _userProfileName1);
        MainWindow.Close();
    }


    private void Button_Click2(object sender, RoutedEventArgs e)
    {
        System.Diagnostics.Process.Start(_firefoxBaseUrl, _runProfileArgument + " " + _userProfileName2);
        MainWindow.Close();
    }
}