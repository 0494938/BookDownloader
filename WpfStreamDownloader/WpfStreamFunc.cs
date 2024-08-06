using BaseBookDownloader;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace WpfStreamDownloader
{
    public class _DocContents
    {
        public string sHtml = "";
    }

    public partial class WpfStreamMainWindow : Window
    {
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
        private void AnalysisURL(string strUrl, bool bWaitOptoin = true)
        {
            Debug.WriteLine("btnAnalysisCurURL_Click invoked...");
            WndContextData? datacontext = null;
            string strFileNameBase = "";
            this.Dispatcher.Invoke(() => {
                datacontext = App.Current.MainWindow.DataContext as WndContextData;
                strFileNameBase = "HtmlDownload_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            });
            StreamWriter? html = null;
            try {
                html = new StreamWriter(
                       File.Open(datacontext.FileTempPath + strFileNameBase + ".html",
                       FileMode.Create,
                       FileAccess.ReadWrite,
                       FileShare.Read),
                       Encoding.UTF8
                    );
            }
            catch (Exception ) {
                return;
            }

            Debug.Assert(datacontext != null);
            bool bOutputHtml = false;
            this.Dispatcher.Invoke(() => { return bOutputHtml = chkShowHtml.IsChecked ?? false; });
            string? strHtml = GetWebDocHtmlSource(strUrl, bWaitOptoin);
            if (!string.IsNullOrEmpty(strHtml?.Trim()))
            {
                ;
                html.Write(strHtml);
                html.Flush();
                html.Close();

                string strPrettyHtml = PrettyPrintUtil.PrettyPrintHtml(strHtml, false, false, false);

                StreamWriter htmlPretty = new StreamWriter(
                    File.Open(datacontext.FileTempPath + strFileNameBase + "_pretty.html",
                    FileMode.Create,
                    FileAccess.ReadWrite,
                    FileShare.Read),
                    Encoding.UTF8
                 );
                htmlPretty.Write(strPrettyHtml);
                htmlPretty.Flush();
                htmlPretty.Close();
                if (bOutputHtml)
                {
                    this.Dispatcher.Invoke(() => {
                        txtWebContents.Text = strPrettyHtml;
                    });
                }
                //analysis of youtube
                if (IsYouTubeSite(strUrl))
                    new Thread(() => datacontext.AnalysisHtmlThreadFunc4YouTube(this, strUrl, strHtml)).Start();
                else if (IsPornHubSite(strUrl))
                    new Thread(() => datacontext.AnalysisHtmlThreadFunc4PornHub(this, strUrl, strHtml)).Start();
                else if (IsRedPornSite(strUrl))
                    new Thread(() => datacontext.AnalysisHtmlThreadFunc4RedPorn(this, strUrl, strHtml)).Start();
                else  //analysis as Novel by default
                    datacontext.AnalysisHtml4Nolvel(this, bWaitOptoin, strUrl, strHtml);
            }
        }

        static string pornHubMatch = "https://[a-zA-Z0-9]+\\.pornhub\\.com/";
        static bool IsPornHubSite(string strUrl)
        {
            //Match match = ex.Match(strUrl);
            bool bMatch= Regex.IsMatch(strUrl, pornHubMatch);
            if(!bMatch)
                bMatch = Regex.IsMatch(strUrl, "https://[a-zA-Z0-0\\-]+\\.phncdn\\.com/");
            return bMatch;
        }

        static string redPornMatch = "https://([a-zA-Z0-9]+\\.)?redporn\\.porn/";
        static bool IsRedPornSite(string strUrl)
        {
            //Match match = ex.Match(strUrl);
            return Regex.IsMatch(strUrl, redPornMatch);
        }

        static bool IsYouTubeSite(string strUrl)
        {
            //return Regex.IsMatch(strUrl, redPornMatch);
            return strUrl.StartsWith("https://www.youtube.com/");
        }

        public string? GetWebDocHtmlSource(string strUrl, bool bWaitOptoin = true, BaseWndContextData? datacontext = null)
        {
            _DocContents doc = new _DocContents();

            string html = "";
            this.Dispatcher.Invoke(() =>
            {
                if (webBrowser != null && webBrowser.CoreWebView2 != null && webBrowser.IsLoaded == true)
                {
                    webBrowser.ExecuteScriptAsync("document.documentElement.outerHTML;").ContinueWith(taskHtml =>
                    {
                        doc.sHtml = taskHtml.Result;
                    });
                }
            });
            int nMaxRetry = 5 * 20;
            int nRetry = 0;
            while (nRetry < nMaxRetry && string.IsNullOrEmpty(doc.sHtml))
            {
                nRetry++;
                Thread.Sleep(200);
            }
            html = doc.sHtml;

            html = Regex.Unescape(html);
            html = html.Remove(0, 1);
            html = html.Remove(html.Length - 1, 1);
            return html;
        }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
    }
}