using SurfSync.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SurfSync.Components;

public partial class UserProfileComponent : UserControl, INotifyPropertyChanged
{
    public static readonly DependencyProperty ProfileProperty = DependencyProperty.Register(
        nameof(Profile),
        typeof(Profile),
        typeof(UserProfileComponent),
        new PropertyMetadata(null, ProfileChanged));

    private readonly Action<Profile> _onClickAction;
    private readonly Brush _customCircleBrush;
    private readonly Brush _customTextBrush;

    public Profile Profile
    {
        get => (Profile)GetValue(ProfileProperty);
        set => SetValue(ProfileProperty, value);
    }

    public string ProfileInitial => string.IsNullOrWhiteSpace(Profile?.Name)
        ? "?"
        : Profile.Name.Trim()[0].ToString().ToUpperInvariant();

    public event PropertyChangedEventHandler PropertyChanged;

    public UserProfileComponent(Profile profile, Action<Profile> onClickAction, Brush customCircleBrush = null, Brush customTextBrush = null)
    {
        Profile = profile;
        _onClickAction = onClickAction;
        _customCircleBrush = customCircleBrush;
        _customTextBrush = customTextBrush;

        InitializeComponent();
        ApplyCustomStyling();
    }

    private static void ProfileChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        var control = (UserProfileComponent)dependencyObject;
        control.OnPropertyChanged(nameof(ProfileInitial));
    }

    private void CardButton_Click(object sender, RoutedEventArgs e)
    {
        if (Profile is null)
            return;

        _onClickAction.Invoke(Profile);
    }

    private void ApplyCustomStyling()
    {
        if (_customCircleBrush is not null)
        {
            CircleBorder.Background = _customCircleBrush;
            ProfileInitialTextBlock.Foreground = GetContrastingTextBrush(_customCircleBrush);
        }

        if (_customTextBrush is not null)
        {
            ProfileNameTextBlock.Foreground = _customTextBrush;
        }
    }

    private static Brush GetContrastingTextBrush(Brush backgroundBrush)
    {
        if (backgroundBrush is not SolidColorBrush solidColorBrush)
            return Brushes.White;

        var color = solidColorBrush.Color;
        var luminance = (0.299 * color.R) + (0.587 * color.G) + (0.114 * color.B);
        return luminance >= 170 ? Brushes.Black : Brushes.White;
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
