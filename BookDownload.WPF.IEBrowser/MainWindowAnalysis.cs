using BaseBookDownloader;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace WpfIEBookDownloader
{
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
    public partial class WPFMainWindow : Window, IBaseMainWindow
    {
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "consistent naming")]
        private static readonly Guid SID_SWebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");

        static SHDocVw.WebBrowser? GetWebBrowserPtr(System.Windows.Controls.WebBrowser webBrowser)
        {
            var serviceProvider = (IServiceProvider)webBrowser.Document;
            if (serviceProvider != null)
            {
                Guid iid = typeof(SHDocVw.WebBrowser).GUID;
                var webBrowserPtr = (SHDocVw.WebBrowser)serviceProvider
                    .QueryService(SID_SWebBrowserApp, ref iid);
                return webBrowserPtr;
            }
            return (SHDocVw.WebBrowser?) null;
        }

        private void btnAnalysisCurURL_Click(object sender, RoutedEventArgs e)
        {
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext != null))
            {
                datacontext.DictDownloadStatus.Clear();
                AnalysisURL(webBrowser.Source.ToString(), false);
            }
        }

        public void LoadUiUrl(BaseWndContextData datacontext, string strURL)
        {
            webBrowser.Dispatcher.Invoke(() =>
            {
                datacontext.PageLoaded = false;
                datacontext.NextLinkAnalysized = false;
                //btnAnalysisCurURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                btnNextPage.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                datacontext.PgmNaviUrl = strURL;
                webBrowser.Navigate(strURL);
            });
        }

        
        public void UpdateNovelName(string sNovelName)
        {
            //this.Invoke(() => {
            //    this.txtBookName.Text = sNovelName;
            //});
        }

    }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
}