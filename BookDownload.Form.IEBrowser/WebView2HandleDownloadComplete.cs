using BaseBookDownloader;
using System;
using System.Diagnostics;
using System.Threading;

namespace BookDownloadFormApp
{
    public partial class WndContextData : BaseWndContextData
    {

    }

    partial class WindowsFormWebView2 : IBaseMainWindow
    {
        WndContextData datacontext = new WndContextData();

              
        private void WebBrowser_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            Debug.WriteLine("WebBrowser_NavigationCompleted invoked...");
            Browser_FrameLoadComplete(sender, (sender as Microsoft.Web.WebView2.WinForms.WebView2)?.Source?.ToString() ?? "");
        }

        void Browser_FrameLoadComplete(object sender, string strUrl)
        {
            Debug.WriteLine("Browser_FrameLoadComplete : " + strUrl + " ...");
            try
            {
                datacontext.PageLoaded = true;
                this.UpdateAnalysisPageButton();
                if (datacontext.RefreshCount > 0 )
                {
                    Debug.Assert(true);
                }
                if (datacontext.DictDownloadStatus.ContainsKey(strUrl))
                {
                    DownloadStatus status = datacontext.DictDownloadStatus[strUrl];
                    UpdateStatusMsg(datacontext, "Finished Page download : " + strUrl + " ...", (int)((100.0 / DownloadStatus.MaxPageToDownload * (status.PageNum - 1 + 0.5))));
                    status.DownloadFinished = true;
                    status.FinishTime = DateTime.Now;
                }
                else
                {
                    UpdateStatusMsg(datacontext, "Finished Page download : " + strUrl + " ...", 50);
                    //Thread thread = new Thread(() => datacontext.WaitAndLaunchAnalsysi(this, e.Url.ToString(), false, null));
                    //thread.Start();
                    new Thread(() => datacontext.WaitAndLaunchAnalsys(this, strUrl, false, null)).Start();

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
