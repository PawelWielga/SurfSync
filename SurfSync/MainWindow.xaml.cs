using SurfSync.Browser;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace SurfSync;

public partial class MainWindow : Window
{
    private readonly IEnumerable<IBrowserService> _browserServices;

    public MainWindow(IEnumerable<IBrowserService> browserServices)
    {
        InitializeComponent();

        _browserServices = browserServices;

        foreach (IBrowserService browserService in browserServices)
            browserService.MainWindow = this;

        MainFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;

        MainFrame.Content = new HomePage(this, _browserServices);
    }

    private void Label_MouseDown(object sender, MouseButtonEventArgs e)
    {
        Close();
    }
}