using BaseBookDownloader;

namespace BookDownloadFormApp
{
    public partial class WndContextData: BaseWndContextData
    {

    }
#if ENABLE_CHROME
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
                    //btnAnalysisCurURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
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
                        //Thread thread = new Thread(() => datacontext.WaitAndLaunchAnalsysi(this, e.Url.ToString(), false, null));
                        //thread.Start();
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
#endif
}
