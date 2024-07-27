using BaseBookDownloader;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
//using HtmlElement = System.Windows.Forms.HtmlElement;

namespace WpfIEBookDownloader
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
            InitBrowser();
        }

        private void WebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("WebBrowser_Loaded invoked...");
            //throw new NotImplementedException();
        }

        private void WebBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            Debug.WriteLine("WebBrowser_LoadCompleted invoked...");
            MainFrameWebLoadCompleted(sender, e.Uri.ToString());
        }

        private void WebBrowser_NavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            Debug.WriteLine("WebBrowser_NavigationCompleted invoked...");
            MainFrameWebLoadCompleted(sender, (sender as Microsoft.Web.WebView2.Wpf.WebView2)?.Source?.ToString()??"");
        }
        private void CoreWebView2_WebMessageReceived(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            //throw new NotImplementedException();
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
            //HideScriptErrors(webBrowser, true);
        }

        private void OnSyncFromBrowser(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnSyncFromBrowser invoked...");
            txtCurURL.Text = webBrowser.Source.ToString();
        }

        public static void StartUrlOnWebBrowser(string strUrl)
        {
            if(!string.IsNullOrEmpty(strUrl.Trim()))
                //Process.Start("explorer", strUrl.Trim());  
                Process.Start(new ProcessStartInfo { FileName = strUrl.Trim(), UseShellExecute = true });
        }

        private void OnLoadInBrowser(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnLoadInBrowser invoked...");
            StartUrlOnWebBrowser(txtInitURL.Text.Trim());
        }

        private void btnLaunchNextUrlOnWeb_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("btnLaunchNextUrlOnWeb_Click invoked...");
            StartUrlOnWebBrowser(txtNextUrl.Text.Trim());
        }

        private void OnBtnRefreshPage(object sender, RoutedEventArgs e)
        {
            webBrowser.Reload();
        }

        private void btnLaunchCurUrlOnWeb_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("btnLaunchNextUrlOnWeb_Click invoked...");
            StartUrlOnWebBrowser(txtCurURL.Text.Trim());

        }

        private void OnCheckPrettyHtmlChanged(object sender, RoutedEventArgs e)
        {
            if (webBrowser.CoreWebView2 == null)
                return;
            if (chkboxPrettyHtml.IsChecked == false)
                GetBrowserDocAndPutToCtrl();
            else
                GetBrowserDocAndPrettyToCtrl();

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