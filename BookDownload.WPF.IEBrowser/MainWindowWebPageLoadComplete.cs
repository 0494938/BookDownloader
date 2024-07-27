using BaseBookDownloader;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WebBrowser = System.Windows.Controls.WebBrowser;

namespace WpfIEBookDownloader
{
    public partial class WPFMainWindow : Window, IBaseMainWindow
    {

        private void MainFrameWebLoadCompleted(object? sender, string strUri)
        {
            Debug.WriteLine("----------------------------------------------------------------------------- MainFrameWebLoadCompleted invoked -----------------------------------------------------------------------------");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext != null))
            {
#if false
                var browser = sender as WebBrowser;
                if (browser == null || browser.Document == null)
                    return;

                dynamic document = browser.Document;

                SHDocVw.WebBrowser? webBrowserPtr = GetWebBrowserPtr(webBrowser);
                Debug.WriteLine(strUri.ToString() + " : Status <" + webBrowserPtr?.ReadyState.ToString() + ">");
                if (webBrowserPtr?.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                    return;
                
                    if(webBrowser.IsLoaded == false)
                    return;
#else
                if (webBrowser == null || webBrowser.CoreWebView2 == null || webBrowser.IsLoaded != true)
                    return;
#endif

                if (strUri.ToString() == datacontext.PgmNaviUrl)
                {
                    try
                    {
                        datacontext.PageLoaded = true;
                        // btnAnalysisCurURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                        if (datacontext.DictDownloadStatus.ContainsKey(strUri))
                        {
                            DownloadStatus status = datacontext.DictDownloadStatus[strUri];
                            UpdateStatusMsg(datacontext, strUri + " : Finished Page download ...", (int)((100.0 / DownloadStatus.MaxPageToDownload * (status.PageNum - 1 + 0.5))));
                            status.DownloadFinished = true;
                            status.FinishTime = DateTime.Now;
                        }
                        else
                        {
                            UpdateStatusMsg(datacontext, strUri + " : Finished Page download ...", 50);
                            new Thread(() =>  AnalysisURL(strUri)).Start();
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