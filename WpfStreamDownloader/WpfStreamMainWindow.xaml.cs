using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using VideoLibrary;


namespace WpfStreamDownloader
{

    public partial class WpfStreamMainWindow : Window
    {
        public WpfStreamMainWindow()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            InitializeComponent();
            InitBrowser();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            webBrowser.Dispose();
        }
    }
}