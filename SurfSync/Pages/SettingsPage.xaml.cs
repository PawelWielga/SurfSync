using SurfSync.Browser;
using SurfSync.Config;
using SurfSync.Enums;
using SurfSync.Logging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SurfSync.Pages;

public partial class SettingsPage : Page
{
    public MainWindow MainWindow { get; }

    private readonly FirefoxService _firefoxService;
    private readonly Dictionary<CheckBox, string> _firefoxProfileVisibilityCheckBoxes = new();
    private readonly Dictionary<string, ProfileColorSelection> _firefoxProfileColorSelections = new(StringComparer.OrdinalIgnoreCase);

    private static readonly IReadOnlyList<ColorChoice> ColorChoices = new List<ColorChoice>
    {
        new("Domyslny (motyw)", string.Empty),
        new("Niebieski", "#FF3A8DFF"),
        new("Granatowy", "#FF1E3A8A"),
        new("Turkusowy", "#FF0EA5A0"),
        new("Zielony", "#FF16A34A"),
        new("Limonkowy", "#FF65A30D"),
        new("Pomaranczowy", "#FFEA580C"),
        new("Czerwony", "#FFDC2626"),
        new("Rozowy", "#FFDB2777"),
        new("Fioletowy", "#FF7C3AED"),
        new("Szary", "#FF6B7280"),
        new("Bialy", "#FFFFFFFF"),
        new("Czarny", "#FF111111")
    };

    public SettingsPage(MainWindow mainWindow)
    {
        InitializeComponent();

        MainWindow = mainWindow;
        _firefoxService = MainWindow.BrowserServices.OfType<FirefoxService>().FirstOrDefault();

        LoadBrowserSelection();
        LoadFirefoxProfileSections();
    }

    private void LoadBrowserSelection()
    {
        var visibleBrowsers = ConfigReader.GetVisibleBrowsers().ToHashSet();

        FirefoxCheckBox.IsChecked = visibleBrowsers.Contains(BrowserType.firefox);
        ChromeCheckBox.IsChecked = visibleBrowsers.Contains(BrowserType.chrome);
        EdgeCheckBox.IsChecked = visibleBrowsers.Contains(BrowserType.edge);
        OperaCheckBox.IsChecked = visibleBrowsers.Contains(BrowserType.opera);
        BraveCheckBox.IsChecked = visibleBrowsers.Contains(BrowserType.brave);
    }

    private void LoadFirefoxProfileSections(
        IEnumerable<string> hiddenProfilesOverride = null,
        IEnumerable<ProfileVisualPreference> visualPreferencesOverride = null)
    {
        if (!TryGetFirefoxProfiles(out var profiles, out var noProfilesMessage))
        {
            ShowNoFirefoxProfilesMessage(noProfilesMessage);
            ShowNoFirefoxColorProfilesMessage(noProfilesMessage);
            SaveFirefoxProfilesVisibilityButton.IsEnabled = false;
            SaveFirefoxProfileColorsButton.IsEnabled = false;
            return;
        }

        LoadFirefoxProfileVisibilitySelection(profiles, hiddenProfilesOverride);
        LoadFirefoxProfileColorSelection(profiles, visualPreferencesOverride);
    }

    private bool TryGetFirefoxProfiles(out List<(string name, bool isDefault)> profiles, out string noProfilesMessage)
    {
        profiles = new List<(string name, bool isDefault)>();

        if (_firefoxService is null)
        {
            noProfilesMessage = "Nie znaleziono serwisu Firefox.";
            return false;
        }

        try
        {
            profiles = _firefoxService.GetProfiles()
                .Where(profile => !string.IsNullOrWhiteSpace(profile.Name))
                .GroupBy(profile => profile.Name.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(group =>
                {
                    var firstProfile = group.First();
                    return (firstProfile.Name.Trim(), group.Any(profile => profile.Default));
                })
                .OrderBy(profile => profile.Item1, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
        catch (Exception ex)
        {
            ErrorLogger.LogException(ex, "SettingsPage.TryGetFirefoxProfiles");
            noProfilesMessage = "Nie udalo sie odczytac profili Firefox.";
            return false;
        }

        if (profiles.Count == 0)
        {
            noProfilesMessage = "Brak profili Firefox do wyswietlenia.";
            return false;
        }

        noProfilesMessage = string.Empty;
        return true;
    }

    private void LoadFirefoxProfileVisibilitySelection(
        IEnumerable<(string name, bool isDefault)> profiles,
        IEnumerable<string> hiddenProfilesOverride = null)
    {
        var hiddenFirefoxProfiles = (hiddenProfilesOverride ?? ConfigReader.GetHiddenFirefoxProfiles())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        _firefoxProfileVisibilityCheckBoxes.Clear();
        FirefoxProfilesVisibilityContainer.Children.Clear();

        foreach (var profile in profiles)
        {
            var checkBox = new CheckBox
            {
                Content = profile.isDefault ? $"{profile.name} (default)" : profile.name,
                IsChecked = !hiddenFirefoxProfiles.Contains(profile.name)
            };

            checkBox.SetResourceReference(StyleProperty, "ModernCheckBoxStyle");
            _firefoxProfileVisibilityCheckBoxes[checkBox] = profile.name;
            FirefoxProfilesVisibilityContainer.Children.Add(checkBox);
        }

        NoFirefoxProfilesMessage.Visibility = Visibility.Collapsed;
        SaveFirefoxProfilesVisibilityButton.IsEnabled = true;
    }

    private void LoadFirefoxProfileColorSelection(
        IEnumerable<(string name, bool isDefault)> profiles,
        IEnumerable<ProfileVisualPreference> visualPreferencesOverride = null)
    {
        var visualPreferences = (visualPreferencesOverride ?? ConfigReader.GetProfileVisualPreferences())
            .Where(preference => preference.browserType == BrowserType.firefox)
            .ToDictionary(preference => preference.profileName.Trim(), StringComparer.OrdinalIgnoreCase);

        _firefoxProfileColorSelections.Clear();
        FirefoxProfileColorsContainer.Children.Clear();

        foreach (var profile in profiles)
        {
            visualPreferences.TryGetValue(profile.name, out var visualPreference);

            var rowGrid = new Grid
            {
                Margin = new Thickness(0, 0, 0, 8)
            };

            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(240) });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });

            var profileLabel = new TextBlock
            {
                Text = profile.isDefault ? $"{profile.name} (default)" : profile.name,
                VerticalAlignment = VerticalAlignment.Center
            };
            profileLabel.SetResourceReference(ForegroundProperty, "TextPrimaryBrush");
            profileLabel.SetResourceReference(StyleProperty, "BodyTextStyle");
            Grid.SetColumn(profileLabel, 0);

            var circleColorComboBox = CreateColorChoiceComboBox(visualPreference?.circleColor);
            Grid.SetColumn(circleColorComboBox, 1);

            var textColorComboBox = CreateColorChoiceComboBox(visualPreference?.textColor);
            Grid.SetColumn(textColorComboBox, 2);

            rowGrid.Children.Add(profileLabel);
            rowGrid.Children.Add(circleColorComboBox);
            rowGrid.Children.Add(textColorComboBox);

            FirefoxProfileColorsContainer.Children.Add(rowGrid);
            _firefoxProfileColorSelections[profile.name] = new ProfileColorSelection(circleColorComboBox, textColorComboBox);
        }

        NoFirefoxProfileColorsMessage.Visibility = Visibility.Collapsed;
        SaveFirefoxProfileColorsButton.IsEnabled = true;
    }

    private ComboBox CreateColorChoiceComboBox(string selectedColor)
    {
        return new ComboBox
        {
            ItemsSource = ColorChoices,
            DisplayMemberPath = nameof(ColorChoice.Label),
            SelectedValuePath = nameof(ColorChoice.Value),
            SelectedValue = NormalizeColorSelection(selectedColor),
            Margin = new Thickness(0, 0, 10, 0),
            MinWidth = 180,
            Height = 30,
            VerticalContentAlignment = VerticalAlignment.Center
        };
    }

    private void SaveBrowsersButton_Click(object sender, RoutedEventArgs e)
    {
        SaveBrowserSelection();
        SetStatus("Zapisano ustawienia widocznych przegladarek.", false);
    }

    private void SaveFirefoxProfilesVisibilityButton_Click(object sender, RoutedEventArgs e)
    {
        SaveFirefoxProfileVisibilitySelection();
        SetStatus("Zapisano widocznosc profili Firefox.", false);
    }

    private void SaveFirefoxProfileColorsButton_Click(object sender, RoutedEventArgs e)
    {
        SaveFirefoxProfileColorSelection();
        SetStatus("Zapisano kolory profili Firefox.", false);
    }

    private void AddFirefoxProfileButton_Click(object sender, RoutedEventArgs e)
    {
        var profileName = FirefoxProfileNameTextBox.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(profileName))
        {
            SetStatus("Podaj nazwe nowego profilu Firefox.", true);
            return;
        }

        if (_firefoxService is null)
        {
            SetStatus("Nie znaleziono serwisu Firefox.", true);
            return;
        }

        try
        {
            _firefoxService.CreateProfile(profileName);
            FirefoxProfileNameTextBox.Text = string.Empty;

            var hiddenProfiles = GetHiddenFirefoxProfilesFromSelection();
            var visualPreferences = GetProfileVisualPreferencesFromSelection();
            LoadFirefoxProfileSections(hiddenProfiles, visualPreferences);

            SetStatus($"Utworzono profil Firefox: {profileName}", false);
        }
        catch (Exception ex)
        {
            ErrorLogger.LogException(ex, "SettingsPage.AddFirefoxProfile");
            SetStatus($"Nie udalo sie utworzyc profilu Firefox: {ex.Message}", true);
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        SaveBrowserSelection();
        SaveFirefoxProfileVisibilitySelection();
        SaveFirefoxProfileColorSelection();
        MainWindow.MainFrame.Content = new HomePage(MainWindow, MainWindow.BrowserServices);
    }

    private void SaveBrowserSelection()
    {
        var selectedBrowsers = new List<BrowserType>();

        if (FirefoxCheckBox.IsChecked == true)
            selectedBrowsers.Add(BrowserType.firefox);

        if (ChromeCheckBox.IsChecked == true)
            selectedBrowsers.Add(BrowserType.chrome);

        if (EdgeCheckBox.IsChecked == true)
            selectedBrowsers.Add(BrowserType.edge);

        if (OperaCheckBox.IsChecked == true)
            selectedBrowsers.Add(BrowserType.opera);

        if (BraveCheckBox.IsChecked == true)
            selectedBrowsers.Add(BrowserType.brave);

        ConfigReader.SetVisibleBrowsers(selectedBrowsers);
    }

    private void SaveFirefoxProfileVisibilitySelection()
    {
        var hiddenProfiles = GetHiddenFirefoxProfilesFromSelection();
        ConfigReader.SetHiddenFirefoxProfiles(hiddenProfiles);
    }

    private void SaveFirefoxProfileColorSelection()
    {
        var currentProfileNames = _firefoxProfileColorSelections.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var selectedProfileVisualPreferences = GetProfileVisualPreferencesFromSelection();

        var allPreferences = ConfigReader.GetProfileVisualPreferences().ToList();
        allPreferences.RemoveAll(preference =>
            preference.browserType == BrowserType.firefox
            && currentProfileNames.Contains(preference.profileName?.Trim() ?? string.Empty));

        allPreferences.AddRange(selectedProfileVisualPreferences);
        ConfigReader.SetProfileVisualPreferences(allPreferences);
    }

    private List<string> GetHiddenFirefoxProfilesFromSelection()
    {
        if (_firefoxProfileVisibilityCheckBoxes.Count == 0)
            return ConfigReader.GetHiddenFirefoxProfiles().ToList();

        return _firefoxProfileVisibilityCheckBoxes
            .Where(pair => pair.Key.IsChecked != true)
            .Select(pair => pair.Value)
            .ToList();
    }

    private List<ProfileVisualPreference> GetProfileVisualPreferencesFromSelection()
    {
        return _firefoxProfileColorSelections
            .Select(pair => new ProfileVisualPreference
            {
                browserType = BrowserType.firefox,
                profileName = pair.Key,
                circleColor = NormalizeColorSelection(pair.Value.CircleColorComboBox.SelectedValue as string),
                textColor = NormalizeColorSelection(pair.Value.TextColorComboBox.SelectedValue as string)
            })
            .Where(preference => !string.IsNullOrWhiteSpace(preference.circleColor) || !string.IsNullOrWhiteSpace(preference.textColor))
            .ToList();
    }

    private void ShowNoFirefoxProfilesMessage(string message)
    {
        _firefoxProfileVisibilityCheckBoxes.Clear();
        FirefoxProfilesVisibilityContainer.Children.Clear();
        NoFirefoxProfilesMessage.Text = message;
        NoFirefoxProfilesMessage.Visibility = Visibility.Visible;
    }

    private void ShowNoFirefoxColorProfilesMessage(string message)
    {
        _firefoxProfileColorSelections.Clear();
        FirefoxProfileColorsContainer.Children.Clear();
        NoFirefoxProfileColorsMessage.Text = message;
        NoFirefoxProfileColorsMessage.Visibility = Visibility.Visible;
    }

    private static string NormalizeColorSelection(string color)
    {
        return string.IsNullOrWhiteSpace(color) ? string.Empty : color.Trim();
    }

    private void SetStatus(string message, bool isError)
    {
        var resourceKey = isError ? "ErrorBrush" : "SuccessBrush";
        StatusTextBlock.Foreground = (Brush)(Application.Current.TryFindResource(resourceKey)
            ?? Application.Current.TryFindResource("TextSecondaryBrush")
            ?? Brushes.Gray);

        StatusTextBlock.Text = message;
    }

    private sealed record ColorChoice(string Label, string Value);

    private sealed record ProfileColorSelection(ComboBox CircleColorComboBox, ComboBox TextColorComboBox);
}
