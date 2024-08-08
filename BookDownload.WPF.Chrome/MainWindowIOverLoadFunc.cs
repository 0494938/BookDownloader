using BaseBookDownloader;
using CefSharp;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace WpfBookDownloader
{
#pragma warning disable CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
#pragma warning disable CA1416 // プラットフォームの互換性を検証
    public partial class WindowsWPFChrome : Window, IBaseMainWindow
    {
        public void NovelTypeChangeEvent(BaseWndContextData datacontext, int nIndex)
        {
            if (txtInitURL != null && datacontext != null)
            {
                //Debug.WriteLine("Select Combox Index : " + cmbNovelType.SelectedIndex);
                UpdateStatusMsg(App.Current.MainWindow.DataContext as WndContextData, "Select Combox Index : " + cmbNovelType.SelectedIndex + " " + ((BatchQueryNovelContents)cmbNovelType.SelectedIndex).ToCode(), -1);
                if (txtInitURL != null)
                {
                    Debug.WriteLine("Select Combox Index : " + cmbNovelType.SelectedIndex);
                    txtInitURL.Text = BaseWndContextData.GetDefaultUrlByIdx(cmbNovelType.SelectedIndex);

                    if (string.IsNullOrEmpty(txtCurURL.Text.Trim()))
                    {
                        txtCurURL.Text = txtInitURL.Text;
                    }
                }
            }
            if ((datacontext != null))
            {
                datacontext.SelectedIdx = nIndex;

                //btnDownloadYouTube.GetBindingExpression(Button.VisibilityProperty).UpdateTarget();
                //btnDownloadPornHub.GetBindingExpression(Button.VisibilityProperty).UpdateTarget();
            }
            UpdateInitPageButton();
            UpdateNextPageButton();
            UpdateAutoDownloadPageButton();
        }

        public string GetNovelName(BaseWndContextData? datacontext = null)
        {
            try
            {
                string sNovelName= "";
                this.Dispatcher.Invoke(() => { sNovelName = txtBookName.Text.Trim(); });
                return sNovelName;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public bool isWebBrowserEmpty(BaseWndContextData? datacontext = null)
        {
            try
            {
                bool bEmpty = false;
                this.Dispatcher.Invoke(() => { bEmpty = webBrowser == null || webBrowser.IsLoading == true; });
                return bEmpty;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool isWebPageLoadComplete(string strURL, BaseWndContextData? datacontext = null)
        {
            try
            {
                bool bComplete = false;
                this.Dispatcher.Invoke(() => { bComplete = webBrowser != null && webBrowser.IsLoaded == true; });
                return bComplete;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void UpdateNextPageButton(BaseWndContextData? datacontext = null)
        {
            this.Dispatcher.Invoke(() =>
            {
                btnNextPage.GetBindingExpression(Button.IsEnabledProperty)?.UpdateTarget();
            });
        }

        public void UpdateInitPageButton(BaseWndContextData? datacontext = null)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.btnInitURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
            });
        }

        public void UpdateAutoDownloadPageButton(BaseWndContextData? datacontext = null)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.btnAutoDownload.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
            });
        }
        public void UpdateAnalysisPageButton(BaseWndContextData? datacontext = null)
        {
        }
        public void UpdateAnalysizedContents(string? strContents, BaseWndContextData? datacontext = null)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtAnalysizedContents.Text = strContents;
            });
        }

        public string GetLogContents(BaseWndContextData? datacontext = null)
        {
            string strLog = "";
            this.Dispatcher.Invoke(() => {
                strLog = txtLog.Text.Replace("\r", "").Replace("\n", "\r\n");
            });
            return strLog;
        }

        public void UpdateNovelName(string sNovelName, BaseWndContextData? datacontext = null)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.txtBookName.Text = sNovelName;
            });
        }

        public void UpdateAggragatedContents(string strContents, BaseWndContextData? datacontext = null)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtAggregatedContents.Text += strContents;
                txtAggregatedContents.ScrollToEnd();
            });
        }

        public void UpdateAggragatedContentsWithLimit(string strContents, BaseWndContextData? datacontext = null)
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

        public void UpdateWebBodyOuterHtml(string? strBody, BaseWndContextData? datacontext = null)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (chkboxPrettyHtml.IsChecked == false)
                    txtWebContents.Text = strBody?.Replace("\r\n\r\n\r\n", "\r\n")?.Replace("\r\n\r\n", "\r\n");
                else
                    GetBrowserDocAndPrettyToCtrl();
            });
        }

        public void UpdateNextUrl(string url, BaseWndContextData? datacontext = null)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtNextUrl.Text = url;
            });
        }

        public void UpdateInitUrl(string url, BaseWndContextData? datacontext = null)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.txtInitURL.Text = url;
            });
        }

        public void UpdateCurUrl(string url, BaseWndContextData? datacontext = null)
        {
            if (!string.IsNullOrEmpty(url))
            {
                this.Dispatcher.Invoke(() =>
                {
                    txtCurURL.Text = url;
                });
            }
        }

        public void RefreshPage(BaseWndContextData? datacontext = null)
        {
            this.Dispatcher.Invoke(() => {
                webBrowser.Reload();
            });
        }

        public string? GetWebDocHtmlSource(string strUrl, bool bWaitOptoin = true, BaseWndContextData? datacontext = null)
        {
            _DocContents doc = new _DocContents();
            string? strBody = null;
            GetWebDocHtml(doc);
            int nMaxRetry = 5 * 20;
            int nRetry = 0;
            while (nRetry  < nMaxRetry && string.IsNullOrEmpty(doc.sHtml) && !datacontext.UnloadPgm)
            {
                nRetry++;
                Thread.Sleep(200);
            }
            strBody = doc.sHtml;
            if (strBody.Length > 0)
            {
                if (strBody.Length < 200)
                    Debug.Assert(false);
            }
            //else
            //{
            //    //Debug.Assert(false);
            //    //this reload should not happen...
            //    //this.RefreshPage();
            //}
            return strBody;
        }

        public void LoadHtmlString(string strHtml, string url, BaseWndContextData? datacontext = null)
        {
            this.Dispatcher.Invoke(() => { webBrowser.LoadHtml(strHtml, url); });
        }

        public void UpdateChapterMsg(BaseWndContextData datacontext, string msg, int value)
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

                    txtChapter.AppendText(msg + ((value >= 0) ? ("(" + value + "%)") : "") + "\r\n");
                    txtChapter.CaretIndex = txtLog.Text.Length;
                    txtChapter.ScrollToEnd();

                }
                if (value >= 0)
                    txtProgress.GetBindingExpression(ProgressBar.ValueProperty).UpdateTarget();
            });
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
            DownloadStatus.WriteDbgLnLog(msg);
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

        public string BatchDownloadNotified(BaseWndContextData datacontext, DownloadStatus status, string sDownloadFileName)
        {
            string strMsgAreaLog = "";
            this.Dispatcher.Invoke(() =>
            {
                this.UpdateInitPageButton();
                this.UpdateAutoDownloadPageButton();

                UpdateStatusMsg(datacontext, "Flush Log to file: " + sDownloadFileName + ".log", -1);
                if (!string.IsNullOrEmpty(status?.NextUrl))
                    txtInitURL.Text = status.NextUrl;

                strMsgAreaLog = txtLog.Text;
                MessageBox.Show(this, "Batch download finished...", "Web Novel Downloader", MessageBoxButton.OK);
            });
            return strMsgAreaLog;
        }

        public bool DownloadFile(BaseWndContextData? datacontext, string sDownloadURL, string sTitle, bool bForceDownload = false)
        {
            throw new NotImplementedException();
        }

        public bool DownloadFile(BaseWndContextData? datacontext, System.Collections.Generic.Dictionary<string, string> dictUrls, string sTitle, bool bForceDownload = false)
        {
            throw new NotImplementedException();
        }
    }
#pragma warning restore CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
#pragma warning restore CA1416 // プラットフォームの互換性を検証
}