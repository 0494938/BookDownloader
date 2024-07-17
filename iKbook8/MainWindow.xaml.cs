using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
//using HtmlElement = System.Windows.Forms.HtmlElement;

namespace iKbook8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //bool bWebPageLoadFinished = false;
        public MainWindow()
        {
            InitializeComponent();
            //this.OnClosing += OnMainWindowClosing;
            //this.OnActivated += OnMainWindowActivated;
        }

        private void MainFrameWebLoaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("MainFrameWebLoaded invoked...");
        }

        private void btnAnalysisCurURL_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("btnAnalysisCurURL_Click invoked...");
            if (webBrowser == null || webBrowser.Document == null)
                return;

            //dynamic document = webBrowser.Document;

            //if (document.readyState != "complete")
            //    return;

            /*
            HtmlElement elem;

            if (webBrowser.Document != null)
            {
                //CodeForm cf = new CodeForm();
                HtmlElementCollection elems = webBrowser.Document.GetElementsByTagName("HTML");
                if (elems.Count == 1)
                {
                    elem = elems[0];
                    cf.Code = elem.OuterHtml;
                    cf.Show();
                }
            }
            */

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
                }
            }


        }

        private void webBrowser1_NewWindow2(ref object ppDisp, ref bool Cancel)
        {
            // Handle the event.  
            Cancel = true;
        }



        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("btnNextPage_Click invoked...");
        }
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
    internal interface IServiceProvider
    {
        [return: MarshalAs(UnmanagedType.IUnknown)]
        object QueryService(ref Guid guidService, ref Guid riid);
    }

}