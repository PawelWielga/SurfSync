using SurfSync.Browser;
using SurfSync.Browsers;
using System.Windows;
using System.Windows.Input;

namespace SurfSync;

public partial class MainWindow : Window
{
    private readonly BrowserResolver _browserResolver;
    private readonly IEnumerable<IBrowserService> _browserServices;

    public MainWindow(BrowserResolver browserResolver, IEnumerable<IBrowserService> browserServices)
    {
        InitializeComponent();

        _browserResolver = browserResolver;
        _browserServices = browserServices;

        foreach (IBrowserService browserService in browserServices)
            browserService.MainWindow = this;

        MainFrame.Content = new HomePage(this, _browserServices);
    }

    private void Label_MouseDown(object sender, MouseButtonEventArgs e)
    {
        Close();
    }
}