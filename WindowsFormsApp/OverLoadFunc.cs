using BaseBookDownload;
using CefSharp;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    public partial class WndContextData : BaseWndContextData
    {

    }

    partial class WindowsFormChrome : IBaseMainWindow
    {
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

        public void UpdateNextPageButton()
        {
        }

        public void UpdateInitPageButton()
        {
        }
        public void UpdateAnalysisPageButton()
        {
        }

        public void UpdateAutoDownloadPageButton()
        {
        }

        public void UpdateNextUrl(string url)
        {
            this.Invoke(() => {
                txtNextUrl.Text = url;
            });
        }

        public void UpdateCurUrl(string url)
        {
        }

        public void UpdateWebBodyOuterHtml(string strBody)
        {
            this.Invoke(() => { 
                txtHtml.Text = strBody.Replace("\r","").Replace("\n","\r\n"); 
            });
        }

        public void UpdateInitUrl(string url)
        {
            this.Invoke(() => {
                this.txtInitURL.Text = url;
            });
        }

        public void UpdateAnalysizedContents(string strContents)
        {
            this.Invoke(() => {
                txtContent.Text = strContents.Replace("\r", "").Replace("\n", "\r\n");
            });
        }

        public void UpdateAggragatedContents(string strContents)
        {
        }

        public void UpdateAggragatedContentsWithLimit(string strContents)
        {
        }

        public string GetLogContents(){
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
                browser.LoadUrl(strURL);
            });
        }

        public bool isWebBrowserEmpty()
        {
            try
            {
                bool bRet = false;
                this.Invoke(() => {
                    bRet = browser == null;//|| browser.IsLoading == true; 
                });
                return bRet;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool isWebPageLoadComplete(string strURL)
        {
            bool bRet = false;
            this.Invoke(() => {
                bRet = browser == null || browser.IsLoading == true;
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
}
