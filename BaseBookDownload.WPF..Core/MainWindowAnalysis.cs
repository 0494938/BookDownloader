using BaseBookDownload;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace BookDownloaderWpf
{
#pragma warning disable CA1416 // プラットフォームの互換性を検証
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
    public partial class WindowsWPFChrome : Window, IBaseMainWindow
    {
        private void btnAnalysisCurURL_Click(object sender, RoutedEventArgs e)
        {
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext != null))
            {
                datacontext.DictDownloadStatus.Clear();
                AnalysisURL(webBrowser.Address.ToString(), false);
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
                webBrowser.LoadUrl(strURL);
            });
        }
    }
#pragma warning restore CA1416 // プラットフォームの互換性を検証
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
}