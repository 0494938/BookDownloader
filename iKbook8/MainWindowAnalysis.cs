using MSHTML;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace BookDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static SHDocVw.WebBrowser? GetWebBrowserPtr(System.Windows.Controls.WebBrowser webBrowser)
        {
            var serviceProvider = (IServiceProvider)webBrowser.Document;
            if (serviceProvider != null)
            {
                Guid serviceGuid = new Guid("0002DF05-0000-0000-C000-000000000046");
                Guid iid = typeof(SHDocVw.WebBrowser).GUID;
                var webBrowserPtr = (SHDocVw.WebBrowser)serviceProvider
                    .QueryService(ref serviceGuid, ref iid);
                return webBrowserPtr;
            }
            return (SHDocVw.WebBrowser?) null;
        }

        private void btnAnalysisCurURL_Click(object sender, RoutedEventArgs e)
        {
            AnalysisCurURL(webBrowser.Source.ToString());
        }
        
        private void AnalysisCurURL(string strUrl)
        {
            Debug.WriteLine("btnAnalysisCurURL_Click invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            Debug.Assert(datacontext!=null);

            if (webBrowser == null || webBrowser.Document == null)
                return;

            var serviceProvider = (IServiceProvider)webBrowser.Document;
            if (serviceProvider != null)
            {
                Guid serviceGuid = new Guid("0002DF05-0000-0000-C000-000000000046");
                Guid iid = typeof(SHDocVw.WebBrowser).GUID;
                var webBrowserPtr = (SHDocVw.WebBrowser)serviceProvider
                    .QueryService(ref serviceGuid, ref iid);
                if (webBrowserPtr != null)
                {
                    webBrowserPtr.NewWindow2 += webBrowser1_NewWindow2;
                    webBrowserPtr.NewWindow3+= webBrowser1_NewWindow3;
                }
            }

            IHTMLDocument2? hTMLDocument2 = webBrowser.Document as IHTMLDocument2;
            IHTMLElement? body = hTMLDocument2?.body as IHTMLElement;
            string? strBody = body?.outerHTML ?? "";
            txtWebContents.Text = body?.outerHTML;
            AnalysisHtmlBody( datacontext, strUrl, strBody);
        }

        private void webBrowser1_NewWindow2(ref object ppDisp, ref bool Cancel)
        {
            // Handle the event.  
            Cancel = true;
        }

        private void webBrowser1_NewWindow3(ref object ppDisp, ref bool Cancel, uint dwFlags,  string bstrUrlContext,  string bstrUrl)
        {
            // Handle the event.  
            Cancel = true;
        }

        public void UpdateStatusMsg(WndContextData datacontext, string msg, int value)
        {

            Debug.WriteLine(msg);
            datacontext.StartBarMsg = msg;
            datacontext.ProcessBarValue = value;
            txtStatus.Dispatcher.Invoke(() =>
            {
                txtStatus.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
                if (!string.IsNullOrEmpty(msg))
                {
                    txtLog.AppendText(msg + "(" + value + "%)\r\n");
                    txtLog.CaretIndex = txtLog.Text.Length;
                    txtLog.ScrollToEnd();
                }
                txtProgress.GetBindingExpression(ProgressBar.ValueProperty).UpdateTarget();
            });
        }

        public void UpdateStatusProgress(WndContextData datacontext, int value)
        {
            datacontext.ProcessBarValue = value;
            //txtProgress.GetBindingExpression(ProgressBar.ValueProperty).UpdateTarget();
            txtProgress.Dispatcher.Invoke(() =>
            {
                txtProgress.GetBindingExpression(ProgressBar.ValueProperty).UpdateTarget();
            });
        }

    }
}