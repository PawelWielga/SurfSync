using SurfSync.Browser;
using SurfSync.Components;
using SurfSync.Pages;
using System.Windows.Controls;
using System.Windows.Input;

namespace SurfSync;

public partial class HomePage : Page
{
    private MainWindow MainWindow { get; init; }
    private IEnumerable<IBrowserService> BrowserServices { get; set; } 

    public HomePage(MainWindow mainWindow, IEnumerable<IBrowserService> browserServices)
    {
        MainWindow = mainWindow;
        BrowserServices = browserServices;

        InitializeComponent();

        PrepareProfiles();
    }

    private void PrepareProfiles()
    {
        foreach (var browserService in BrowserServices)
        {
            var profiles = browserService.GetProfiles();
            foreach (var profile in profiles)
            {
                ProfilesContainer.Children.Add(new UserProfileComponent(profile, browserService.OpenBrowserWithProfile));
            }
        }
    }

    private void SettingsButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        MainWindow.MainFrame.Content = new SettingsPage(MainWindow);
    }
}