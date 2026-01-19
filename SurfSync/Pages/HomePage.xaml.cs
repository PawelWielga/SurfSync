using SurfSync.Browser;
using SurfSync.Components;
using SurfSync.Models;
using SurfSync.Logging;
using SurfSync.Pages;
using System.Diagnostics;
using System.Windows;
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

        Loaded += HomePage_Loaded;
    }

    private async void HomePage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= HomePage_Loaded;
        try
        {
            await PrepareProfilesAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load profiles: {ex}");
            ErrorLogger.LogException(ex, "HomePage.Loaded");
        }
    }

    private async Task PrepareProfilesAsync()
    {
        var profiles = await Task.Run(() =>
        {
            var items = new List<(Profile profile, Action<Profile> openAction)>();
            foreach (var browserService in BrowserServices)
            {
                try
                {
                    var browserProfiles = browserService.GetProfiles();
                    foreach (var profile in browserProfiles)
                    {
                        items.Add((profile, browserService.OpenBrowserWithProfile));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to load {browserService.BrowserType} profiles: {ex}");
                    ErrorLogger.LogException(ex, $"Load profiles: {browserService.BrowserType}");
                }
            }

            return items;
        });

        foreach (var item in profiles)
        {
            ProfilesContainer.Children.Add(new UserProfileComponent(item.profile, item.openAction));
        }
    }

    private void SettingsButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        MainWindow.MainFrame.Content = new SettingsPage(MainWindow);
    }
}
