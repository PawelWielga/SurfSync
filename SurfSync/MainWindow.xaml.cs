using SurfSync.Browser;
using SurfSync.Pages;
using SurfSync.Theme;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Navigation;

namespace SurfSync;

public partial class MainWindow : Window
{
    private const int DwmaUseImmersiveDarkMode = 20;
    private const int DwmaUseImmersiveDarkModeBefore20H1 = 19;

    private const double HomeMinWidthValue = 340;
    private const double HomeMinHeightValue = 250;
    private const double HomeMaxWidthValue = 1200;
    private const double HomeMaxHeightValue = 900;
    private const double HomeCardWidth = 106;
    private const double HomeCardHeight = 104;
    private const double HomeWidthPadding = 100;
    private const double HomeHeightPadding = 82;
    private const int HomeMaxColumns = 5;

    private const double SettingsMinWidthValue = 760;
    private const double SettingsMinHeightValue = 560;

    public IReadOnlyList<IBrowserService> BrowserServices { get; }

    public MainWindow(IEnumerable<IBrowserService> browserServices)
    {
        InitializeComponent();

        SourceInitialized += MainWindow_SourceInitialized;
        Closed += MainWindow_Closed;
        ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;

        BrowserServices = browserServices.ToList();

        foreach (IBrowserService browserService in BrowserServices)
            browserService.MainWindow = this;

        MainFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
        MainFrame.Content = new HomePage(this, BrowserServices);
    }

    private void MainWindow_SourceInitialized(object sender, EventArgs e)
    {
        ApplyNativeTitleBarTheme(ThemeManager.IsLightThemeEnabled);
    }

    private void ThemeManager_ThemeChanged(bool isLightTheme)
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.Invoke(() => ApplyNativeTitleBarTheme(isLightTheme));
            return;
        }

        ApplyNativeTitleBarTheme(isLightTheme);
    }

    private void MainWindow_Closed(object sender, EventArgs e)
    {
        ThemeManager.ThemeChanged -= ThemeManager_ThemeChanged;
    }

    private void OptionsButton_Click(object sender, RoutedEventArgs e)
    {
        OpenOptionsPage();
    }

    public void OpenOptionsPage()
    {
        ApplyWindowConstraints(SettingsMinWidthValue, SettingsMinHeightValue);
        ResizeWindowKeepingCenter(Math.Max(Width, SettingsMinWidthValue), Math.Max(Height, SettingsMinHeightValue));

        if (MainFrame.Content is SettingsPage)
            return;

        MainFrame.Content = new SettingsPage(this);
    }

    public void AdjustWindowToProfiles(int profileCount)
    {
        if (WindowState != WindowState.Normal)
            return;

        ApplyWindowConstraints(HomeMinWidthValue, HomeMinHeightValue);

        var safeProfileCount = Math.Max(profileCount, 1);
        var columns = Math.Clamp((int)Math.Ceiling(Math.Sqrt(safeProfileCount)), 1, HomeMaxColumns);
        var rows = (int)Math.Ceiling(safeProfileCount / (double)columns);

        var desiredWidth = (columns * HomeCardWidth) + HomeWidthPadding;
        var desiredHeight = (rows * HomeCardHeight) + HomeHeightPadding;

        var workArea = SystemParameters.WorkArea;
        var maxWidth = Math.Min(HomeMaxWidthValue, Math.Max(HomeMinWidthValue, workArea.Width - 40));
        var maxHeight = Math.Min(HomeMaxHeightValue, Math.Max(HomeMinHeightValue, workArea.Height - 40));

        var targetWidth = Math.Clamp(desiredWidth, HomeMinWidthValue, maxWidth);
        var targetHeight = Math.Clamp(desiredHeight, HomeMinHeightValue, maxHeight);

        ResizeWindowKeepingCenter(targetWidth, targetHeight);
    }

    private void ApplyWindowConstraints(double minWidth, double minHeight)
    {
        MinWidth = minWidth;
        MinHeight = minHeight;
    }

    private void ResizeWindowKeepingCenter(double targetWidth, double targetHeight)
    {
        if (!IsLoaded)
            return;

        var workArea = SystemParameters.WorkArea;
        var centerX = Left + (Width / 2);
        var centerY = Top + (Height / 2);

        Width = targetWidth;
        Height = targetHeight;

        Left = Math.Clamp(centerX - (targetWidth / 2), workArea.Left, workArea.Right - targetWidth);
        Top = Math.Clamp(centerY - (targetHeight / 2), workArea.Top, workArea.Bottom - targetHeight);
    }

    private void ApplyNativeTitleBarTheme(bool isLightTheme)
    {
        var hwnd = new WindowInteropHelper(this).Handle;
        if (hwnd == IntPtr.Zero)
            return;

        var useDarkTitleBar = isLightTheme ? 0 : 1;
        _ = DwmSetWindowAttribute(
            hwnd,
            DwmaUseImmersiveDarkMode,
            ref useDarkTitleBar,
            Marshal.SizeOf<int>());

        _ = DwmSetWindowAttribute(
            hwnd,
            DwmaUseImmersiveDarkModeBefore20H1,
            ref useDarkTitleBar,
            Marshal.SizeOf<int>());
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(
        IntPtr hwnd,
        int attribute,
        ref int attributeValue,
        int attributeSize);
}
