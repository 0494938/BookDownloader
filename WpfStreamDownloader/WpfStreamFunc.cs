using BaseBookDownloader;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfStreamDownloader
{
    public class WndContextData : BaseWndContextData
    {
        public Visibility EnabledDbgButtons { get; set; } =
#if DEBUG
#if false
            Visibility.Visible;
#else
            Visibility.Hidden;
#endif
#else
            Visibility.Hidden;
#endif
    }

    public class _DocContents
    {
        public string sHtml = "";
    }

    public partial class WpfStreamMainWindow : Window
    {
        private void AnalysisURL(string strUrl, bool bWaitOptoin = true)
        {
            Debug.WriteLine("btnAnalysisCurURL_Click invoked...");
            WndContextData? datacontext = null;
            string strFileNameBase = "";
            this.Dispatcher.Invoke(() => {
                datacontext = App.Current.MainWindow.DataContext as WndContextData;
                strFileNameBase = AppDomain.CurrentDomain.BaseDirectory + "HtmlDownload_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            });

            Debug.Assert(datacontext != null);

            string? strHtml = GetWebDocHtmlSource(strUrl, bWaitOptoin);
            if (!string.IsNullOrEmpty(strHtml?.Trim()))
            {
                StreamWriter html = new StreamWriter(
                   File.Open(strFileNameBase + ".html",
                   FileMode.Create,
                   FileAccess.ReadWrite,
                   FileShare.Read),
                   Encoding.UTF8
                );
                html.Write(strHtml);
                html.Flush();
                html.Close();

                string strPrettyHtml = PrettyPrintUtil.PrettyPrintHtml(strHtml, false, false, false);

                StreamWriter htmlPretty = new StreamWriter(
                    File.Open(strFileNameBase + "_pretty.html",
                    FileMode.Create,
                    FileAccess.ReadWrite,
                    FileShare.Read),
                    Encoding.UTF8
                 );
                htmlPretty.Write(strPrettyHtml);
                htmlPretty.Flush();
                htmlPretty.Close();

                this.Dispatcher.Invoke(() => {
                    txtWebContents.Text = strPrettyHtml;
                });
                //analysis of youtube
                if(strUrl.StartsWith("https://www.youtube.com", StringComparison.InvariantCultureIgnoreCase))
                    new Thread(() => datacontext.AnalysisHtmlThreadFunc4YouTube(this, strUrl, strHtml)).Start();
                else  //analysis as Novel by default
                    datacontext.AnalysisHtml4Nolvel(this, bWaitOptoin, strUrl, strHtml);
            }
        }

        public string? GetWebDocHtmlSource(string strUrl, bool bWaitOptoin = true)
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
    }
}