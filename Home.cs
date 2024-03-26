namespace FirefoxProfileLauncher
{
    public partial class Home : Form
    {
        private string _firefoxBaseUrl = "C:\\Program Files\\Mozilla Firefox\\firefox.exe";
        private string _runProfileArgument = "-P";
        private string _userProfileName1 = "Pawel";
        private string _userProfileName2 = "Inka";

        public Home()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(_firefoxBaseUrl, _runProfileArgument + " " + _userProfileName1);
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(_firefoxBaseUrl, _runProfileArgument + " " + _userProfileName2);
            Application.Exit();
        }
    }
}
