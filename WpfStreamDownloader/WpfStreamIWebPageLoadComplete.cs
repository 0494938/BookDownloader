using BaseBookDownloader;
using System.Diagnostics;
using System.Windows;

namespace WpfStreamDownloader
{

    public partial class WpfStreamMainWindow : Window
    {
        private void MainFrameWebLoadCompleted(object? sender, string strUri)
        {
            Debug.WriteLine("----------------------------------------------------------------------------- MainFrameWebLoadCompleted invoked -----------------------------------------------------------------------------");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext != null))
            {
                if (webBrowser == null || webBrowser.CoreWebView2 == null || webBrowser.IsLoaded != true)
                    return;

                if (strUri.ToString() == datacontext.PgmNaviUrl)
                {
                    try
                    {
                        datacontext.PageLoaded = true;
                        // btnAnalysisCurURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                        if (datacontext.DictDownloadStatus.ContainsKey(strUri) )
                        {
                            DownloadStatus status = datacontext.DictDownloadStatus[strUri];
                            if (status.DownloadFinished == false)
                            {
                                UpdateStatusMsg(datacontext, strUri + " : Finished Page download ...", (int)((100.0 / DownloadStatus.MaxPageToDownload * (status.PageNum - 1 + 0.5))));
                                status.DownloadFinished = true;
                                status.FinishTime = DateTime.Now;
                            }
                        }
                        else
                        {
                            UpdateStatusMsg(datacontext, strUri + " : Finished Page download ...", 50);
                            new Thread(() => AnalysisURL(strUri)).Start();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
                else if (IsYouTubeSite(strUri))
                {
                    datacontext.PageLoaded = true; UpdateStatusMsg(datacontext, strUri + " : Finished Page download ...", 50);
                    new Thread(() => AnalysisURL(strUri)).Start();
                }
                else if (IsPornHubSite(strUri))
                {
                    datacontext.PageLoaded = true; UpdateStatusMsg(datacontext, strUri + " : Finished Page download ...", 50);
                    new Thread(() => AnalysisURL(strUri)).Start();
                }
                else if (strUri.StartsWith("https://redporn.porn/"))
                {
                    datacontext.PageLoaded = true; UpdateStatusMsg(datacontext, strUri + " : Finished Page download ...", 50);
                    new Thread(() => AnalysisURL(strUri)).Start();
                }

                if (!string.IsNullOrEmpty(webBrowser.CoreWebView2.Source.ToString()))
                {
                    this.UpdateCurUrl(webBrowser.Source.ToString());
                }
            }
        }
    }
}