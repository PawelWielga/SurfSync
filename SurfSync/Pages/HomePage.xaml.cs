using SurfSync.Browser;
using SurfSync.Components;
using SurfSync.Config;
using SurfSync.Enums;
using SurfSync.Logging;
using SurfSync.Models;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SurfSync;

public partial class HomePage : Page
{
    private MainWindow MainWindow { get; }
    private IEnumerable<IBrowserService> BrowserServices { get; }

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
        var visibleBrowsers = ConfigReader.GetVisibleBrowsers().ToHashSet();
        var hiddenFirefoxProfiles = ConfigReader.GetHiddenFirefoxProfiles().ToHashSet(StringComparer.OrdinalIgnoreCase);
        var profileVisualPreferences = ConfigReader.GetProfileVisualPreferencesByKey();

        var profiles = await Task.Run(() =>
        {
            var items = new List<(Profile profile, Action<Profile> openAction)>();
            foreach (var browserService in BrowserServices)
            {
                if (!visibleBrowsers.Contains(browserService.BrowserType))
                    continue;

                try
                {
                    var browserProfiles = browserService.GetProfiles();
                    foreach (var profile in browserProfiles)
                    {
                        if (browserService.BrowserType == BrowserType.firefox
                            && hiddenFirefoxProfiles.Contains(profile?.Name?.Trim() ?? string.Empty))
                        {
                            continue;
                        }

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

        ProfilesContainer.Children.Clear();
        foreach (var item in profiles)
        {
            profileVisualPreferences.TryGetValue(
                ConfigReader.BuildProfileVisualKey(item.profile.BrowserType, item.profile.Name),
                out var profileVisualPreference);

            var circleBrush = TryCreateBrush(profileVisualPreference?.circleColor);
            var textBrush = TryCreateBrush(profileVisualPreference?.textColor);

            ProfilesContainer.Children.Add(new UserProfileComponent(item.profile, item.openAction, circleBrush, textBrush));
        }

        MainWindow.AdjustWindowToProfiles(profiles.Count);
    }

    private static Brush TryCreateBrush(string color)
    {
        if (string.IsNullOrWhiteSpace(color))
            return null;

        try
        {
            if (ColorConverter.ConvertFromString(color.Trim()) is Color parsedColor)
            {
                var brush = new SolidColorBrush(parsedColor);
                brush.Freeze();
                return brush;
            }
        }
        catch
        {
            // Ignore invalid color values from config.
        }

        return null;
    }
}
