using BaseBookDownload;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace BookDownloaderWpf
{
#pragma warning disable CA1416 // プラットフォームの互換性を検証
    public partial class WindowsWPFChrome : Window, IBaseMainWindow
    {
        private void MainFrameWebLoadCompleted(object sender, NavigationEventArgs e)
        {
            Debug.WriteLine("----------------------------------------------------------------------------- MainFrameWebLoadCompleted invoked -----------------------------------------------------------------------------");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext != null))
            {
                if (webBrowser== null || webBrowser.IsLoaded == false)
                    return;

                if (webBrowser.IsLoaded != true)
                    return;

                if (e.Uri.ToString() == datacontext.PgmNaviUrl)
                {
                    try
                    {
                        datacontext.PageLoaded = true;
                              // btnAnalysisCurURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                        if (datacontext.DictDownloadStatus.ContainsKey(e.Uri.ToString()))
                        {
                            DownloadStatus status = datacontext.DictDownloadStatus[e.Uri.ToString()];
                            UpdateStatusMsg(datacontext, e.Uri.ToString() + " : Finished Page download ...", (int)((100.0 / DownloadStatus.MaxPageToDownload * (status.PageNum - 1 + 0.5))));
                            status.DownloadFinished = true;
                            status.FinishTime = DateTime.Now;
                        }
                        else
                        {
                            UpdateStatusMsg(datacontext, e.Uri.ToString() + " : Finished Page download ...", 50);
                            AnalysisURL(e.Uri.ToString());
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
}