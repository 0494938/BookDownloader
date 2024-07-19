using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WebBrowser = System.Windows.Controls.WebBrowser;

namespace BookDownloader
{
    public partial class MainWindow : Window
    {
        private void MainFrameWebLoadCompleted(object sender, NavigationEventArgs e)
        {
            Debug.WriteLine("----------------------------------------------------------------------------- MainFrameWebLoadCompleted invoked -----------------------------------------------------------------------------");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext != null))
            {
                var browser = sender as WebBrowser;

                if (browser == null || browser.Document == null)
                    return;

                dynamic document = browser.Document;

                SHDocVw.WebBrowser? webBrowserPtr = GetWebBrowserPtr(webBrowser);
                Debug.WriteLine(e.Uri.ToString() + " : Status <" + webBrowserPtr?.ReadyState.ToString() + ">");
                if (webBrowserPtr?.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                    return;

                //int nStep = 0;
                dynamic? script = null;
                try
                {
                    script = document.createElement("script");
                    script.type = @"text/javascript";
                    script.text = @"window.onerror = function(msg,url,line){return true;}";
                    document.head.appendChild(script);
                }
                catch (Exception)
                {
                }

                try
                {
                    datacontext.PageLoaded = true;
                    btnAnalysisCurURL.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
                    if (dictDownloadStatus.ContainsKey(e.Uri.ToString()))
                    {
                        DownloadStatus status = dictDownloadStatus[e.Uri.ToString()];
                        UpdateStatusMsg(datacontext, e.Uri.ToString() + " : Finished Page download ...", (int)((100.0 / DownloadStatus.ThreadMax * (status.ThreadNum - 1 + 0.5))));
                        status.DownloadFinished = true;
                        status.FinishTime = DateTime.Now;
                    }
                    else
                    {
                        UpdateStatusMsg(datacontext, e.Uri.ToString() + " : Finished Page download ...", 50);
                        AnalysisCurURL(e.Uri.ToString());
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