using BaseBookDownload;
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

namespace WindowsFormsApp
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
                Debug.WriteLine("Load Completed ....");
            }
            else
            {
                Debug.WriteLine("still loading ....");
            }
        }

        void Browser_FrameLoadComplete(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            Debug.WriteLine("Browser_FrameLoadComplete ...");
            if (e.Frame.IsMain)
            {
                if (e.Url.ToString() == datacontext.PgmNaviUrl)
                {
                    try
                    {
                        datacontext.PageLoaded = true;
                        //btnAnalysisCurURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                        this.UpdateAnalysisPageButton();
                        if (dictDownloadStatus.ContainsKey(e.Url.ToString()))
                        {
                            DownloadStatus status = dictDownloadStatus[e.Url.ToString()];
                            UpdateStatusMsg(datacontext, e.Url.ToString() + " : Finished Page download ...", (int)((100.0 / DownloadStatus.ThreadMax * (status.ThreadNum - 1 + 0.5))));
                            status.DownloadFinished = true;
                            status.FinishTime = DateTime.Now;
                        }
                        else
                        {
                            UpdateStatusMsg(datacontext, e.Url.ToString() + " : Finished Page download ...", 50);
                            Thread thread = new Thread(() => WaitAndLaunchAnalsysi(datacontext, this, e.Url.ToString(), false, null));
                            thread.Start();
                            //AnalysisURL(e.Url.ToString());

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
}
