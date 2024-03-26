using System.Windows;
using System.Windows.Controls;

namespace FirefoxProfileLauncher;

public partial class HomePage : Page
{
    private string _firefoxBaseUrl = "C:\\Program Files\\Mozilla Firefox\\firefox.exe";
    private string _runProfileArgument = "-P";
    private string _userProfileName1 = "Pawełek";
    private string _userProfileName2 = "Ewelinka";

    public HomePage()
    {
        InitializeComponent();
    }

    private void Button_Click1(object sender, RoutedEventArgs e)
    {
        System.Diagnostics.Process.Start(_firefoxBaseUrl, _runProfileArgument + " " + _userProfileName1);
    }


    private void Button_Click2(object sender, RoutedEventArgs e)
    {
        System.Diagnostics.Process.Start(_firefoxBaseUrl, _runProfileArgument + " " + _userProfileName2);
    }
}