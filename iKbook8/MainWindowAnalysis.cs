using System.Windows;

namespace BookDownloader
{
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class WPFMainWindow : Window, IBaseMainWindow
    {
        static SHDocVw.WebBrowser? GetWebBrowserPtr(System.Windows.Controls.WebBrowser webBrowser)
        {
            var serviceProvider = (IServiceProvider)webBrowser.Document;
            if (serviceProvider != null)
            {
                Guid serviceGuid = new Guid("0002DF05-0000-0000-C000-000000000046");
                Guid iid = typeof(SHDocVw.WebBrowser).GUID;
                var webBrowserPtr = (SHDocVw.WebBrowser)serviceProvider
                    .QueryService(ref serviceGuid, ref iid);
                return webBrowserPtr;
            }
            return (SHDocVw.WebBrowser?) null;
        }

        private void btnAnalysisCurURL_Click(object sender, RoutedEventArgs e)
        {
            dictDownloadStatus.Clear();
            AnalysisURL(webBrowser.Source.ToString(), false);
        }

        
    }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
}