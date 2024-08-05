using BaseBookDownloader;

namespace BookDownloadFormApp
{
    public partial class WndContextData : BaseWndContextData
    {

    }

#if ENABLE_CHROME
    partial class WindowsFormChrome : IBaseMainWindow
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

        public void LoadHtmlString(string strHtml, string url, BaseWndContextData? datacontext = null)
        {
            //this.Dispatcher.Invoke(() => { webBrowser.lo(strHtml, url); });
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
                txtHtml.Text = strBody.Replace("\r","").Replace("\n","\r\n").Replace("\r\n\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n");
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

        public string GetLogContents(BaseWndContextData? datacontext = null){
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
                webBrowser.LoadUrl(strURL);
            });
        }

        public void Back(BaseWndContextData datacontext)
        {
            this.Invoke(() =>
            {
                webBrowser.Back();
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
            bool bRet = false;
            this.Invoke(() => {
                bRet = webBrowser == null || webBrowser.IsLoading == true;
            });
            return !bRet;
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
#endif
}
