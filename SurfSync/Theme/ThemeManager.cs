using Microsoft.Win32;
using System.Collections;
using System.Windows;
using System.Windows.Media;

namespace SurfSync.Theme;

public static class ThemeManager
{
    private const string PersonalizePath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
    private const string AppsUseLightThemeValue = "AppsUseLightTheme";

    public static event Action<bool> ThemeChanged;
    public static bool IsLightThemeEnabled { get; private set; } = true;

    public static void Initialize(Application app)
    {
        ApplyTheme(app);
        SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
    }

    public static void Shutdown()
    {
        SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
    }

    private static void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
    {
        if (Application.Current is null)
            return;

        if (e.Category is not UserPreferenceCategory.Color and not UserPreferenceCategory.General)
            return;

        Application.Current.Dispatcher.Invoke(() => ApplyTheme(Application.Current));
    }

    private static void ApplyTheme(Application app)
    {
        var isLightTheme = IsLightTheme();
        var palette = CreatePalette(isLightTheme);

        foreach (DictionaryEntry item in palette)
            app.Resources[item.Key] = item.Value;

        IsLightThemeEnabled = isLightTheme;
        ThemeChanged?.Invoke(isLightTheme);
    }

    private static bool IsLightTheme()
    {
        using var key = Registry.CurrentUser.OpenSubKey(PersonalizePath);
        var rawValue = key?.GetValue(AppsUseLightThemeValue);
        if (rawValue is int intValue)
            return intValue > 0;

        if (rawValue is string stringValue && int.TryParse(stringValue, out var parsedValue))
            return parsedValue > 0;

        return true;
    }

    private static ResourceDictionary CreatePalette(bool isLightTheme)
    {
        return isLightTheme ? CreateLightPalette() : CreateDarkPalette();
    }

    private static ResourceDictionary CreateLightPalette()
    {
        return new ResourceDictionary
        {
            ["AppBackgroundBrush"] = Brush("#FFF3F3F3"),
            ["SurfaceBrush"] = Brush("#FFFFFFFF"),
            ["SurfaceAltBrush"] = Brush("#FFF7F7F7"),
            ["SurfaceHoverBrush"] = Brush("#FFF3F3F3"),
            ["SurfacePressedBrush"] = Brush("#FFEDEDED"),
            ["BorderBrush"] = Brush("#FFDCDCDC"),
            ["TextPrimaryBrush"] = Brush("#FF1B1B1B"),
            ["TextSecondaryBrush"] = Brush("#FF5E5E5E"),
            ["TextDisabledBrush"] = Brush("#FF9E9E9E"),
            ["AccentBrush"] = Brush("#FF0067C0"),
            ["AccentHoverBrush"] = Brush("#FF005EAE"),
            ["AccentPressedBrush"] = Brush("#FF004B8C"),
            ["OnAccentBrush"] = Brush("#FFFFFFFF"),
            ["InputBackgroundBrush"] = Brush("#FFFFFFFF"),
            ["TagBackgroundBrush"] = Brush("#160067C0"),
            ["SuccessBrush"] = Brush("#FF1E7C45"),
            ["ErrorBrush"] = Brush("#FFC42B1C")
        };
    }

    private static ResourceDictionary CreateDarkPalette()
    {
        return new ResourceDictionary
        {
            ["AppBackgroundBrush"] = Brush("#FF202020"),
            ["SurfaceBrush"] = Brush("#FF2A2A2A"),
            ["SurfaceAltBrush"] = Brush("#FF323232"),
            ["SurfaceHoverBrush"] = Brush("#FF3A3A3A"),
            ["SurfacePressedBrush"] = Brush("#FF3F3F3F"),
            ["BorderBrush"] = Brush("#FF434343"),
            ["TextPrimaryBrush"] = Brush("#FFF6F6F6"),
            ["TextSecondaryBrush"] = Brush("#FFC7C7C7"),
            ["TextDisabledBrush"] = Brush("#FF8C8C8C"),
            ["AccentBrush"] = Brush("#FF4CC2FF"),
            ["AccentHoverBrush"] = Brush("#FF63CBFF"),
            ["AccentPressedBrush"] = Brush("#FF39A5DD"),
            ["OnAccentBrush"] = Brush("#FF111111"),
            ["InputBackgroundBrush"] = Brush("#FF1E1E1E"),
            ["TagBackgroundBrush"] = Brush("#264CC2FF"),
            ["SuccessBrush"] = Brush("#FF6CCB92"),
            ["ErrorBrush"] = Brush("#FFFF8A80")
        };
    }

    private static SolidColorBrush Brush(string colorHex)
    {
        var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorHex)!);
        brush.Freeze();
        return brush;
    }
}
