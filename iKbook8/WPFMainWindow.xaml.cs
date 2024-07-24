using BaseBookDownload;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
//using HtmlElement = System.Windows.Forms.HtmlElement;

namespace BookDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class WPFMainWindow : Window, IBaseMainWindow
    {
        public WPFMainWindow()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            InitializeComponent();
        }

        private void MainFrameWebLoaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("MainFrameWebLoaded invoked...");
        }

        private void PagesPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
        private static readonly Regex _regex = new Regex("[^0-9]+");

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void PreviewPagesTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnMainWindowLoaded invoked...");
            HideScriptErrors(webBrowser, true);
        }

        private void OnSyncFromBrowser(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnSyncFromBrowser invoked...");
            txtCurURL.Text = webBrowser.Source.ToString();
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