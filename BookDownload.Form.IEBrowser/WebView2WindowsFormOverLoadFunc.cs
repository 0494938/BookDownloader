using BaseBookDownloader;
using System;
using System.Diagnostics;

namespace BookDownloadFormApp
{
#pragma warning disable CS8632 
    public partial class WndContextData : BaseWndContextData
    {

    }
    
    partial class WindowsFormWebView2 : IBaseMainWindow
    {
        public void UpdateChapterMsg(BaseWndContextData datacontext, string msg, int value)
        {

        }

        public void UpdateStatusMsg(BaseWndContextData datacontext, string msg, int value)
        {
            this.Invoke(() =>
            {
                txtStatus.Text = msg;
                txtLog.AppendText(msg.TrimEnd(new char[] { '\r', '\n', ' ', '\t' }) + "\r\n");
                if(value >=0) 
                    txtProgress.Value = value;
            });
            Debug.WriteLine(msg);
            DownloadStatus.WriteDbgLnLog(msg);
        }

        public void UpdateStatusProgress(BaseWndContextData datacontext, int value)
        {
            if(value >= 0)
            {
                this.Invoke(() =>
                {
                    txtProgress.Value = value;
                });

            }
        }

        public void LoadHtmlString(string strHtml, string sHtmlContent, BaseWndContextData? datacontext = null)
        {
            this.Invoke(() => { 
                webBrowser.CoreWebView2.NavigateToString(sHtmlContent);
            });
        }

        public void UpdateNextPageButton(BaseWndContextData? datacontext = null)
        {
        }

        public void UpdateInitPageButton(BaseWndContextData? datacontext = null)
        {
        }
        public void UpdateAnalysisPageButton(BaseWndContextData? datacontext = null)
        {
        }

        public void UpdateAutoDownloadPageButton(BaseWndContextData? datacontext = null)
        {
        }

        public void UpdateNextUrl(string url, BaseWndContextData? datacontext = null)
        {
            this.Invoke(() => {
                txtNextUrl.Text = url;
            });
        }

        public void UpdateCurUrl(string url, BaseWndContextData? datacontext = null)
        {
        }
        public string GetNovelName(BaseWndContextData? datacontext = null)
        {
            return "";
        }
        public void UpdateWebBodyOuterHtml(string strBody, BaseWndContextData? datacontext = null)
        {
            this.Invoke(() => {
                //txtHtml.Text = strBody.Replace("\r","").Replace("\n","\r\n").Replace("\r\n\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n");
                txtHtml.Text = strBody.Replace("\r", "").Replace("\n", "\r\n").Replace("\r\n\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n");
            });
        }

        public void UpdateInitUrl(string url, BaseWndContextData? datacontext = null)
        {
            this.Invoke(() => {
                this.txtInitURL.Text = url;
            });
        }

        public void UpdateNovelName(string sNovelName, BaseWndContextData? datacontext = null)
        {
            //this.Invoke(() => {
            //    this.txtBookName.Text = sNovelName;
            //});
        }

        public void UpdateAnalysizedContents(string strContents, BaseWndContextData? datacontext = null)
        {
            this.Invoke(() => {
                txtContent.Text = strContents.Replace("\r", "").Replace("\n", "\r\n");
            });
        }

        public void UpdateAggragatedContents(string strContents, BaseWndContextData? datacontext = null)
        {
        }

        public void UpdateAggragatedContentsWithLimit(string strContents, BaseWndContextData? datacontext = null)
        {
        }

        public string GetLogContents(BaseWndContextData? datacontext = null)
        {
            string strLog="";
            this.Invoke(() => {
                strLog= txtLog.Text.Replace("\r", "").Replace("\n", "\r\n");
            });
            return strLog;
        }

        public void LoadUiUrl(BaseWndContextData datacontext, string strURL)
        {
            this.Invoke(() =>
            {
                datacontext.PageLoaded = false;
                datacontext.NextLinkAnalysized = false;
                datacontext.PgmNaviUrl = strURL;
                webBrowser.CoreWebView2.Navigate(strURL);
            });
        }

        public void Back(BaseWndContextData datacontext)
        {
            this.Invoke(() =>
            {
                webBrowser.CoreWebView2.GoBack();
            });
        }


        public bool isWebBrowserEmpty(BaseWndContextData? datacontext = null)
        {
            try
            {
                bool bRet = false;
                this.Invoke(() => {
                    bRet = webBrowser == null;//|| browser.IsLoading == true; 
                });
                return bRet;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool isWebPageLoadComplete(string strURL, BaseWndContextData? datacontext = null)
        {
            bool bRetNotReady = false;
            this.Invoke(() => {
                bRetNotReady = webBrowser == null /*|| webBrowser.CoreWebView2. == true*/;
            });
            return !bRetNotReady;
        }
        public string BatchDownloadNotified(BaseWndContextData datacontext, DownloadStatus status, string sDownloadFileName)
        {
            string strMsgAreaLog = "";
            this.Invoke(() =>
            {
                this.UpdateInitPageButton();
                this.UpdateAutoDownloadPageButton();

                UpdateStatusMsg(datacontext, "Flush Log to file: " + sDownloadFileName + ".log", -1);
                if (!string.IsNullOrEmpty(status?.NextUrl))
                    txtInitURL.Text = status.NextUrl;

                strMsgAreaLog = txtLog.Text;
                //MessageBox.Show(this, "Batch download finished...", "Web Novel Downloader", MessageBoxButton.OK);
            });
            return strMsgAreaLog;
        }

    }
#pragma warning restore CS8632 
}
