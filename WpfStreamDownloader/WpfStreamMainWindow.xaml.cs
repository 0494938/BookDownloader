using System.Text;
using System.Windows;

namespace WpfStreamDownloader
{

    public partial class WpfStreamMainWindow : Window
    {
        public WpfStreamMainWindow()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //FFmpegBinariesHelper.RegisterFFmpegBinaries();
            //DynamicallyLoadedBindings.Initialize();

            InitializeComponent();
            InitBrowser();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            webBrowser.Dispose();
        }

    }
}