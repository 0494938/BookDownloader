using BaseBookDownloader;
using CefSharp;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace WpfBookDownloader
{
#pragma warning disable CA1416 // プラットフォームの互換性を検証
#pragma warning disable IDE0019 // パターン マッチングを使用します
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
    public partial class WindowsWPFChrome : Window, IBaseMainWindow
    {
        private void AnalysisURL(string strUrl, bool bWaitOptoin = true)
        {
            Debug.WriteLine("btnAnalysisCurURL_Click invoked...");
            WndContextData? datacontext = null;
            this.Dispatcher.Invoke(() =>
            {
                datacontext = App.Current.MainWindow.DataContext as WndContextData;
            });
            Debug.Assert(datacontext != null);

            string? strBody = GetWebDocHtmlBody(strUrl, bWaitOptoin);
            if (!string.IsNullOrEmpty(strBody?.Trim()))
            {
                txtWebContents.Text = strBody;
                datacontext.AnalysisHtmlBody(this, bWaitOptoin, strUrl, strBody);
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

        public void Back(BaseWndContextData datacontext)
        {
            datacontext.PageLoaded = false;
            datacontext.NextLinkAnalysized = false;
            webBrowser.Dispatcher.Invoke(() =>
            {
                btnNextPage.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                webBrowser.Back();
            });
        }

    }
#pragma warning restore CA1416 // プラットフォームの互換性を検証
#pragma warning restore CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning restore IDE0019 // パターン マッチングを使用します
}