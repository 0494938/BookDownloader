using MSHTML;
using System.Diagnostics;
using System.Windows;

namespace iKbook8
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
            //IHTMLDocument? hTMLDocument = webBrowser.Document as IHTMLDocument;

            IHTMLElement? body = hTMLDocument2?.body as IHTMLElement;
            string? strBody = body?.outerHTML ?? "";
            //string? strHtml = hTMLDocument2.boday
            txtWebContents.Text = body?.outerHTML;
            AnalysisHtmlBody(ref datacontext, ref strBody);
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

        /*
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HtmlDocument Document {
            get {
                object objDoc = this.AxIWebBrowser2.Document;
                if (objDoc != null) {
                    // Document is not necessarily an IHTMLDocument, it might be an office document as well.
                    UnsafeNativeMethods.IHTMLDocument2 iHTMLDocument2 = null;
                    try {
                        iHTMLDocument2 = objDoc as UnsafeNativeMethods.IHTMLDocument2;
                    } 
                    catch (InvalidCastException) { 
                    }
                    if (iHTMLDocument2 != null) {
                        UnsafeNativeMethods.IHTMLLocation iHTMLLocation = iHTMLDocument2.GetLocation();
                        if (iHTMLLocation != null) {
                            string href = iHTMLLocation.GetHref();
                            if (!string.IsNullOrEmpty(href))
                            {
                                Uri url = new Uri(href);
                                WebBrowser.EnsureUrlConnectPermission(url);  // Security check
                                return new HtmlDocument (ShimManager, iHTMLDocument2 as UnsafeNativeMethods.IHTMLDocument);
                            }
                        }
                    }
                }
                return null;
            }
        }
        */
    }
}