using BaseBookDownloader;
using CefSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace BookDownloadFormApp
{
    public partial class WndContextData: BaseWndContextData
    {

    }
    partial class WindowsFormChrome: IBaseMainWindow
    {
        WndContextData datacontext = new WndContextData();

        private void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading == false)
            {
                Debug.WriteLine("Browser_LoadingStateChanged : Load Completed ....");
            }
            else
            {
                Debug.WriteLine("Browser_LoadingStateChanged : Still loading ....");
            }
        }

        void Browser_FrameLoadComplete(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            Debug.WriteLine("Browser_FrameLoadComplete : " + e.Url.ToString() + " ...");
            if (e.Frame.IsMain)
            {
                try
                {
                    datacontext.PageLoaded = true;
                    this.UpdateAnalysisPageButton();
                    if (datacontext.RefreshCount > 0 )
                    {
                        Debug.Assert(true);
                    }
                    if (datacontext.DictDownloadStatus.ContainsKey(e.Url.ToString()))
                    {
                        DownloadStatus status = datacontext.DictDownloadStatus[e.Url.ToString()];
                        UpdateStatusMsg(datacontext, "Finished Page download : " + e.Url.ToString() + " ...", (int)((100.0 / DownloadStatus.MaxPageToDownload * (status.PageNum - 1 + 0.5))));
                        status.DownloadFinished = true;
                        status.FinishTime = DateTime.Now;
                    }
                    else
                    {
                        UpdateStatusMsg(datacontext, "Finished Page download : " + e.Url.ToString() + " ...", 50);
                        new Thread(() => datacontext.WaitAndLaunchAnalsys(this, e.Url.ToString(), false, null)).Start();

                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }
    }
}
