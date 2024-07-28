using BaseBookDownloader;
using System.Diagnostics;
using System.Text;
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

    public partial class WpfStreamMainWindow : Window
    {
        public async void InitBrowser()
        {
            await webBrowser.EnsureCoreWebView2Async(null);
            webBrowser.NavigationCompleted += WebBrowser_NavigationCompleted;
            webBrowser.Loaded += WebBrowser_Loaded;
            webBrowser.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
            //webBrowser.CoreWebView2.AddWebResourceRequestedFilter("https://www.youtube.com/*",
            //    Microsoft.Web.WebView2.Core.CoreWebView2WebResourceContext.All);

            webBrowser.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
            webBrowser.CoreWebView2.WebResourceResponseReceived += CoreWebView2_WebResourceResponseReceived; ;

            await webBrowser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.postMessage(window.document.URL);");
            await webBrowser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.addEventListener(\'message\', event => alert(event.data));");
        }


    }
}