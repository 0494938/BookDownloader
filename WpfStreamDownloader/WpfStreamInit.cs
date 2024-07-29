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
            webBrowser.Unloaded += WebBrowser_Unloaded;

            //webBrowser.CoreWebView2.AddWebResourceRequestedFilter("https://www.youtube.com/*",
            //    Microsoft.Web.WebView2.Core.CoreWebView2WebResourceContext.All);

            webBrowser.CoreWebView2.FrameCreated += CoreWebView2_FrameCreated;
            webBrowser.CoreWebView2.FrameNavigationStarting += CoreWebView2_FrameNavigationStarting;
            webBrowser.CoreWebView2.FrameNavigationCompleted += CoreWebView2_FrameNavigationCompleted;
            webBrowser.CoreWebView2.BasicAuthenticationRequested += CoreWebView2_BasicAuthenticationRequested;
            webBrowser.CoreWebView2.ClientCertificateRequested += CoreWebView2_ClientCertificateRequested;
            webBrowser.CoreWebView2.ContentLoading += CoreWebView2_ContentLoading;
            webBrowser.CoreWebView2.ContextMenuRequested += CoreWebView2_ContextMenuRequested;
            webBrowser.CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;
            webBrowser.CoreWebView2.DownloadStarting += CoreWebView2_DownloadStarting;
            webBrowser.CoreWebView2.FaviconChanged += CoreWebView2_FaviconChanged;
            webBrowser.CoreWebView2.IsDocumentPlayingAudioChanged += CoreWebView2_IsDocumentPlayingAudioChanged;
            webBrowser.CoreWebView2.IsMutedChanged += CoreWebView2_IsMutedChanged;
            webBrowser.CoreWebView2.LaunchingExternalUriScheme += CoreWebView2_LaunchingExternalUriScheme;
            webBrowser.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
            webBrowser.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
            webBrowser.CoreWebView2.PermissionRequested += CoreWebView2_PermissionRequested;
            webBrowser.CoreWebView2.ProcessFailed += CoreWebView2_ProcessFailed;
            webBrowser.CoreWebView2.ScriptDialogOpening += CoreWebView2_ScriptDialogOpening;
            webBrowser.CoreWebView2.ServerCertificateErrorDetected += CoreWebView2_ServerCertificateErrorDetected;
            webBrowser.CoreWebView2.SourceChanged += CoreWebView2_SourceChanged;
            webBrowser.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
            webBrowser.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
            webBrowser.CoreWebView2.WebResourceResponseReceived += CoreWebView2_WebResourceResponseReceived;
            webBrowser.CoreWebView2.WindowCloseRequested += CoreWebView2_WindowCloseRequested;

            webBrowser.CoreWebView2.IsMuted = true;

            await webBrowser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.postMessage(window.document.URL);");
            await webBrowser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.addEventListener(\'message\', event => alert(event.data));");
        }

    }
}