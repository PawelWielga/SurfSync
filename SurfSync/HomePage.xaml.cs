using SurfSync.Browser;
using SurfSync.Browsers;
using SurfSync.Components;
using SurfSync.Enums;
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
        foreach (IBrowserService browserService in BrowserServices)
        {
            var profiles = browserService.GetProfiles();
            foreach (var profile in profiles)
            {
                ProfilesContainer.Children.Add(new UserProfileComponent(profile, browserService.OpenBrowserWithProfile));
            }
        }
    }

    private void Image_MouseDownAsync(object sender, MouseButtonEventArgs e) 
    {
        //_browserService.OpenBrowserProfileSettings(); 
    }
}