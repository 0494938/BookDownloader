using CefSharp;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp
{
    public class WndContextData: BaseWndContextData
    {

    }
    partial class WindowsFormChrome
    {
        void Browser_FrameLoadComplete(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            Debug.WriteLine("wb_FrameLoadEnd ....");
            if (e.Frame.IsMain)
            {
                //browser.ViewSource();
                browser.GetSourceAsync().ContinueWith(taskHtml =>
                {
                    var html = taskHtml.Result;
                    AnalysisURL(e.Uri.ToString());

                });
            }
        }

        private void AnalysisURL(string strUrl, bool bWaitOptoin = true)
        {
            Debug.WriteLine("btnAnalysisCurURL_Click invoked...");
            string? strBody = GetWebDocHtmlBody(strUrl, bWaitOptoin);
            if (!string.IsNullOrEmpty(strBody.Trim()))
            {
                WndContextData datacontext = new WndContextData();
                AnalysisHtmlBody(datacontext, bWaitOptoin, strUrl, strBody);
            }
        }
        public string? GetWebDocHtmlBody(string strUrl, bool bWaitOptoin = true)
        {

            bool bFailed = false;
            this.Dispatcher.Invoke(() =>
            {
                if (webBrowser == null || webBrowser.Document == null)
                    bFailed = true;
            });

            string? strBody = null;
            if (!bFailed)
            {
                this.Dispatcher.Invoke(() =>
                {
                    var serviceProvider = (IServiceProvider)webBrowser.Document;
                    if (serviceProvider != null)
                    {
                        Guid serviceGuid = new Guid("0002DF05-0000-0000-C000-000000000046");
                        Guid iid = typeof(SHDocVw.WebBrowser).GUID;
                        SHDocVw.WebBrowser? webBrowserPtr = serviceProvider
                            .QueryService(ref serviceGuid, ref iid) as SHDocVw.WebBrowser;
                        Debug.WriteLine(strUrl + " : Status <" + webBrowserPtr?.ReadyState.ToString() + ">");
                        if (webBrowserPtr != null)
                        {
                            webBrowserPtr.NewWindow2 += webBrowser1_NewWindow2;
                            webBrowserPtr.NewWindow3 += webBrowser1_NewWindow3;
                        }
                    }

                    IHTMLDocument2? hTMLDocument2 = webBrowser.Document as IHTMLDocument2;
                    IHTMLElement? body = hTMLDocument2?.body as IHTMLElement;
                    strBody = body?.outerHTML;
                });

            }
            return strBody;
        }

        public void AnalysisHtmlBody(WndContextData? datacontext, bool bWaitOption, string strURL, string strBody, bool bSilenceMode = false, DownloadStatus? status = null)
        {
            Debug.Assert(datacontext != null);
            try
            {
                Thread thread = new Thread(() => AnalysisHtmlBodyThreadFunc(datacontext, this, strURL, strBody, bSilenceMode, status));
                thread.Start();
            }
            catch (TaskCanceledException)
            {
                //ignore TaskCanceledException
            }
            //AnalysisHtmlBodyThreadFunc(datacontext, this, strURL, strBody, bSilenceMode, status);
        }
    }
}
