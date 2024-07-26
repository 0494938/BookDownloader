using BaseBookDownload;
using CefSharp;
using MSHTML;
using System;
using System.Diagnostics;
using System.Security.Policy;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace BookDownloaderWpf
{
#pragma warning disable CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
#pragma warning disable CA1416 // プラットフォームの互換性を検証
    class _DocContents
    {
        public string sHtml = "";
    }

    public partial class WindowsWPFChrome : Window, IBaseMainWindow
    {
        public void UpdateNextPageButton() {
            this.Dispatcher.Invoke(() =>
            {
                btnNextPage.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
            });
        }

        public void UpdateInitPageButton()
        {
            this.Dispatcher.Invoke(() =>
            {
                this.btnInitURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
            });
        }

        public void UpdateAutoDownloadPageButton()
        {
            this.Dispatcher.Invoke(() =>
            {
                this.btnAutoDownload.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
            });
        }
        public void UpdateAnalysisPageButton()
        {
        }
        public void UpdateAnalysizedContents(string ? strContents)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtAnalysizedContents.Text = strContents;
            });
        }

        public string GetLogContents()
        {
            string strLog = "";
            this.Dispatcher.Invoke(() => {
                strLog = txtLog.Text.Replace("\r", "").Replace("\n", "\r\n");
            });
            return strLog;
        }

        public void UpdateAggragatedContents(string strContents)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtAggregatedContents.Text += strContents;
                txtAggregatedContents.ScrollToEnd();
            });
        }
        
        public void UpdateAggragatedContentsWithLimit(string strContents)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (txtAggregatedContents.Text.Length > 1024 * 64)
                    txtAggregatedContents.Text = strContents;
                else
                    txtAggregatedContents.Text += strContents;
                txtAggregatedContents.ScrollToEnd();
            });
        }

        public void UpdateWebBodyOuterHtml(string? strBody)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtWebContents.Text = strBody.Replace("\r","").Replace("\n","\r\n").Replace("\r\n\r\n\r\n","\r\n").Replace("\r\n\r\n", "\r\n");
            });
        }

        public void UpdateNextUrl(string url)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtNextUrl.Text = url;
            });
        }

        public void UpdateInitUrl(string url)
        {
            this.Dispatcher.Invoke(() => 
            { 
                this.txtInitURL.Text = url;
            });
        }

        public void UpdateCurUrl(string url)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtCurURL.Text = url;
            });
        }

        public void RefreshPage()
        {
            this.Dispatcher.Invoke(() => {
                webBrowser.Reload();
            });
        }

        public string? GetWebDocHtmlBody(string strUrl, bool bWaitOptoin = true)
        {
            _DocContents doc = new _DocContents();
            string? strBody = null;
            GetWebDocHtml(doc);
            this.Dispatcher.Invoke(() => { strBody = doc.sHtml; });
            while (string.IsNullOrEmpty(doc.sHtml))
            {
                Thread.Sleep(200);
                GetWebDocHtml(doc);
                this.Dispatcher.Invoke(() => {
                    strBody = doc.sHtml;
                    if (strBody.Length > 0 && strBody.Length < 200)
                        Debug.Assert(false);
                });
            }
            return strBody;
        }

        private void GetWebDocHtml(_DocContents doc)
        {
            bool isLoading = false;
            this.Dispatcher.Invoke(() => {
                isLoading = webBrowser.IsLoading;
            });
            if (webBrowser == null || isLoading == true)
                return;

            this.Dispatcher.Invoke(() => {
                webBrowser.GetSourceAsync().ContinueWith(taskHtml =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        doc.sHtml = taskHtml.Result;
                    });
                });
            });
        }

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

        public void UpdateStatusMsg(BaseWndContextData datacontext, string msg, int value)
        {

            Debug.WriteLine(msg);
            datacontext.StartBarMsg = msg;
            if (value >= 0)
                datacontext.ProcessBarValue = value;
            txtStatus.Dispatcher.Invoke(() =>
            {
                txtStatus.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
                if (!string.IsNullOrEmpty(msg))
                {
                    txtLog.AppendText(msg + ((value >= 0) ? ("(" + value + "%)") : "") + "\r\n");
                    txtLog.CaretIndex = txtLog.Text.Length;
                    txtLog.ScrollToEnd();
                }
                if (value >= 0)
                    txtProgress.GetBindingExpression(ProgressBar.ValueProperty).UpdateTarget();
            });
        }

        public void UpdateStatusProgress(BaseWndContextData datacontext, int value)
        {
            datacontext.ProcessBarValue = value;
            //txtProgress.GetBindingExpression(ProgressBar.ValueProperty).UpdateTarget();
            txtProgress.Dispatcher.Invoke(() =>
            {
                txtProgress.GetBindingExpression(ProgressBar.ValueProperty).UpdateTarget();
            });
        }
    }
#pragma warning restore CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
#pragma warning restore CA1416 // プラットフォームの互換性を検証
}