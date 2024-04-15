using System.Windows.Controls;
using System.Windows.Input;

namespace SurfSync.Pages;

public partial class SettingsPage : Page
{
    public MainWindow MainWindow { get; }

    public SettingsPage(MainWindow mainWindow)
    {
        InitializeComponent();
        MainWindow = mainWindow;
    }


    private void BackButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        MainWindow.MainFrame.NavigationService.GoBack();
    }
}
