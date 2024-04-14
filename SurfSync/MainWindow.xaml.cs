using SurfSync.Browser;
using System.Windows;
using System.Windows.Input;

namespace SurfSync;

public partial class MainWindow : Window
{
    private readonly IBrowserService _browserService;

    public MainWindow(IBrowserService browserService)
    {
        InitializeComponent();

        _browserService = browserService;
        MainFrame.Content = new HomePage(this, _browserService);
    }

    private void Label_MouseDown(object sender, MouseButtonEventArgs e)
    {
        Close();
    }
}