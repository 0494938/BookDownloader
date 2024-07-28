using BaseBookDownloader;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;

namespace WpfBookDownloader
{
#pragma warning disable CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
#pragma warning disable CA1416 // プラットフォームの互換性を検証
    public partial class WindowsWPFChrome : Window, IBaseMainWindow
    {
        void Browser_FrameLoadComplete(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            Debug.WriteLine("Browser_FrameLoadComplete : " + e.Url.ToString() + " ...");
            WndContextData? datacontext = null;
            bool bIsLoaded = false;
            this.Dispatcher.Invoke(() => {
                datacontext = App.Current.MainWindow.DataContext as WndContextData;
                bIsLoaded = webBrowser.IsLoaded;

            }); 
            if (e.Frame.IsMain)
            {
                try
                {
                    datacontext.PageLoaded = true;
                    //btnAnalysisCurURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                    this.UpdateAnalysisPageButton();
                    if (datacontext.RefreshCount > 0)
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
                        //AnalysisURL(e.Url.ToString());

                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }


        private void MainFrameWebLoadCompleted(object sender, string strURL /*NavigationEventArgs e1*/)
        {
            Debug.WriteLine("----------------------------------------------------------------------------- MainFrameWebLoadCompleted invoked -----------------------------------------------------------------------------");
            WndContextData? datacontext = null;
            bool bIsLoaded = false;
            this.Dispatcher.Invoke(() => { 
                datacontext = App.Current.MainWindow.DataContext as WndContextData;
                bIsLoaded = webBrowser.IsLoaded;

            });
            
            if ((datacontext != null))
            {
                if (webBrowser== null || bIsLoaded == false)
                    return;

                if (bIsLoaded  != true)
                    return;

                if (strURL == datacontext.PgmNaviUrl)
                {
                    try
                    {
                        datacontext.PageLoaded = true;
                              // btnAnalysisCurURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                        if (datacontext.DictDownloadStatus.ContainsKey(strURL))
                        {
                            DownloadStatus status = datacontext.DictDownloadStatus[strURL];
                            UpdateStatusMsg(datacontext, strURL + " : Finished Page download ...", (int)((100.0 / DownloadStatus.MaxPageToDownload * (status.PageNum - 1 + 0.5))));
                            status.DownloadFinished = true;
                            status.FinishTime = DateTime.Now;
                        }
                        else
                        {
                            UpdateStatusMsg(datacontext, strURL + " : Finished Page download ...", 50);
                            AnalysisURL(strURL);
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
#pragma warning restore CA1416 // プラットフォームの互換性を検証
#pragma warning restore CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
}