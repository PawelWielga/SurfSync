using SurfSync.Browser;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SurfSync.Components;

public partial class UserProfileComponent : UserControl
{
    public static readonly DependencyProperty ProfileProperty = DependencyProperty.Register("Profile", typeof(Profile), typeof(UserProfileComponent), null);
    public Profile Profile
    {
        get { return (Profile)GetValue(ProfileProperty); }
        set { SetValue(ProfileProperty, value); }
    }

    private Action<Profile> OnMouseDownAction;

    public UserProfileComponent(Profile profile, Action<Profile> onMouseDownAction)
    {
        Profile = profile;
        OnMouseDownAction = onMouseDownAction;

        InitializeComponent();
    }

    private void Border_MouseDown(object sender, MouseButtonEventArgs e) => OnMouseDownAction.Invoke(Profile);
}
