using System.Windows;

namespace FirefoxProfileLauncher;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainFrame.Content = new HomePage(this);
    }

    private void Label_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        Close();
    }
}