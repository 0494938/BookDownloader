using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace WpfStreamDownloader
{

    public partial class WpfStreamMainWindow : Window
    {
        private void WebBrowser_NavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            Debug.WriteLine("WebBrowser_NavigationCompleted invoked...");
            MainFrameWebLoadCompleted(sender, (sender as Microsoft.Web.WebView2.Wpf.WebView2)?.Source?.ToString() ?? "");

            if (sender is WebView2 webView2) {
                //webView2.CoreWebView2
                Debug.Assert(true);
            }
        }

        private void WebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("WebBrowser_Loaded invoked...");
        }

        private void WebBrowser_Unloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("WebBrowser_Unloaded invoked...");
        }

        private void CoreWebView2_WebMessageReceived(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            Debug.WriteLine("CoreWebView2_WebMessageReceived invoked..." +  e.ToString());
        }

        private void CoreWebView2_FrameNavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            Debug.WriteLine("CoreWebView2_FrameNavigationCompleted invoked... Success:" + e.IsSuccess + ", " + e.ToString());
        }

        private void CoreWebView2_FrameNavigationStarting(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            Debug.WriteLine("CoreWebView2_FrameNavigationStarting invoked... IsRedirected:" + e.IsRedirected + ", Uri:" + e.Uri.ToString());
        }

        CoreWebView2Frame? frame=null;
        private void CoreWebView2_FrameCreated(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2FrameCreatedEventArgs e)
        {
            Debug.WriteLine("CoreWebView2_FrameCreated invoked... " + e.ToString());
            frame = e.Frame;
        }


        private void CoreWebView2_WebResourceResponseReceived(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if (e.Response != null && datacontext !=null)
            {
                int statusCode = e.Response.StatusCode;
                //string date = e.Response.Headers.GetHeader("date");
                //if(e.Request.Uri.ToString().StartsWith("https://www.youtube.com/"))
                Debug.WriteLine("CoreWebView2.WebResourceResponseReceived triggered. status code: " + statusCode + ", Request Url: " + e.Request.Uri.ToString());
                if(statusCode >= 300 && statusCode < 400)
                {
                    Debug.WriteLine("CoreWebView2.WebResourceResponseReceived -- response to redirect. status code: " + statusCode + ", Request Url: " + (e.Response.Headers.ToList().Where(n => n.Key == "Location")?.FirstOrDefault().Value ?? "0"));
                }
                if (e.Request.Uri.ToString().StartsWith("https://www.youtube.com/"))
                {
                    UpdateStreamMsg(datacontext, e.Request.Method + ", " + e.Response.StatusCode + ", " + (e.Response.Headers.ToList().Where(n => n.Key == "content-length")?.FirstOrDefault().Value??"0") + " : " + e.Request.Uri, -1);
                    Debug.Assert(true);
                    //string strUrl = e.Request.Uri.ToString();
                    //e.Response.GetContentAsync().ContinueWith(t => {
                    //    StreamReader reader = new StreamReader(t.Result);
                    //    string html = reader.ReadToEnd();
                    //    //html = html.Replace("<html", "<HTML");
                    //    //byte[] byteArray = Encoding.UTF8.GetBytes(html);
                    //    //e.Request.Content = new MemoryStream(byteArray);
                    //    if (strUrl.StartsWith("https://www.youtube.com/"))
                    //    {
                    //        Debug.Assert(true);
                    //        //datacontext.WaitAndLaunchAnalsys
                    //    }
                    //});
                }else if (e.Request.Uri.ToString().Contains(".phncdn.com/hls/videos"))
                {
                    UpdateStreamMsg(datacontext, e.Request.Method + ", " + e.Response.StatusCode + ", " + (e.Response.Headers.ToList().Where(n => n.Key == "content-length")?.FirstOrDefault().Value ?? "0") + " : " + e.Request.Uri , -1);
                }
                else if (e.Request.Uri.ToString().StartsWith("https://etahub.com/events"))
                {
                    UpdateStreamMsg(datacontext, e.Request.Method + ", " + e.Response.StatusCode + ", " + (e.Response.Headers.ToList().Where(n => n.Key == "content-length")?.FirstOrDefault().Value ?? "0") + " : " + e.Request.Uri, -1);
                }
            }
            else
            {
                Debug.Assert(false);
            }
        }

        private void CoreWebView2_WebResourceRequested(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebResourceRequestedEventArgs e)
        {
            Debug.WriteLine("CoreWebView2.WebResourceRequested triggered: " + e.Request.Uri.ToString());

            if (e.Request == null)
            {
                System.Diagnostics.Trace.WriteLine("CoreWebView2_WebResourceRequested :: RESP NULL");
                return;
            }

            if (e.Response == null || e.Response.Content == null)
            {
                System.Diagnostics.Trace.WriteLine("CoreWebView2_WebResourceRequested :: Content NULL");
                return;
            }

            //System.Diagnostics.Trace.WriteLine("GO REPLACE");
            StreamReader reader = new StreamReader(e.Request.Content);
            string html = reader.ReadToEnd();
            //html = html.Replace("<html", "<HTML");
            //byte[] byteArray = Encoding.UTF8.GetBytes(html);
            //e.Request.Content = new MemoryStream(byteArray);
            if (e.Request.Uri.ToString().StartsWith("https://www.youtube.com/"))
            {
                Debug.Assert(true);
            }
        }

        private void CoreWebView2_WindowCloseRequested(object? sender, object e)
        {
            Debug.WriteLine("CoreWebView2_WindowCloseRequested invoked... " + e.ToString());
        }

        private void CoreWebView2_SourceChanged(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs e)
        {
            Debug.WriteLine("CoreWebView2_SourceChanged invoked... IsNewDocument:" + e.IsNewDocument + ", " + e.ToString());
        }

        private void CoreWebView2_ServerCertificateErrorDetected(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2ServerCertificateErrorDetectedEventArgs e)
        {
            Debug.WriteLine("CoreWebView2_ServerCertificateErrorDetected invoked... RequestUri:" + e.RequestUri + ", ErrorStatus:" + e.ErrorStatus + ", Action:" + e.Action + ", " + e.ToString());
        }

        private void CoreWebView2_ScriptDialogOpening(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2ScriptDialogOpeningEventArgs e)
        {
            Debug.WriteLine("CoreWebView2_ScriptDialogOpening invoked... Message:" + e.Message + ", Uri:" + e.Uri.ToString() + ", ResultText:" + e.ResultText + ", DefaultText:" + e.DefaultText + ", " + e.ToString());
        }

        private void CoreWebView2_ProcessFailed(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2ProcessFailedEventArgs e)
        {
            Debug.WriteLine("CoreWebView2_ProcessFailed invoked... ProcessFailedKind:" + e.ProcessFailedKind + ", FailureSourceModulePath" + e.FailureSourceModulePath + ", ProcessDescription" + e.ProcessDescription + ", " + e.ToString());
        }

        private void CoreWebView2_PermissionRequested(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2PermissionRequestedEventArgs e)
        {
            Debug.WriteLine("CoreWebView2_PermissionRequested invoked... State:" + e.State + ", Uri:" + e.Uri + ", PermissionKind:" + e.PermissionKind + ", IsUserInitiated:" + e.IsUserInitiated + ", " + e.ToString());
        }

        private void CoreWebView2_NewWindowRequested(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs e)
        {
            Debug.WriteLine("CoreWebView2_NewWindowRequested invoked... Name:" + e.Name + ", Uri:" + e.Uri + ", NewWindow:" + e.NewWindow + ", WindowFeatures:" + e.WindowFeatures + ", " + e.ToString());
            e.Handled = true;
        }

        private void CoreWebView2_NavigationStarting(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            Debug.WriteLine("CoreWebView2_NavigationStarting invoked... NavigationKind:" + e.NavigationKind + ", Uri:" + e.Uri + ", NavigationId:" + e.NavigationId + ", Cancel:" + e.Cancel + ", AdditionalAllowedFrameAncestors:" + e.AdditionalAllowedFrameAncestors + ", " + e.ToString());
        }

        private void CoreWebView2_LaunchingExternalUriScheme(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2LaunchingExternalUriSchemeEventArgs e)
        {
            Debug.WriteLine("CoreWebView2_LaunchingExternalUriScheme invoked... InitiatingOrigin:" + e.InitiatingOrigin + ", Uri:" + e.Uri + ", IsUserInitiated:" + e.IsUserInitiated + ", " + e.ToString());
        }

        private void CoreWebView2_IsMutedChanged(object? sender, object e)
        {
            Debug.WriteLine("CoreWebView2_IsMutedChanged invoked... " + e.ToString());
        }

        private void CoreWebView2_IsDocumentPlayingAudioChanged(object? sender, object e)
        {
            Debug.WriteLine("CoreWebView2_IsDocumentPlayingAudioChanged invoked... Success:" + e.ToString());
        }

        private void CoreWebView2_FaviconChanged(object? sender, object e)
        {
            Debug.WriteLine("CoreWebView2_FaviconChanged invoked... Success:" + e.ToString());
        }

        private void CoreWebView2_DownloadStarting(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2DownloadStartingEventArgs e)
        {
            Debug.WriteLine("CoreWebView2_DownloadStarting invoked... Cancel:" + e.Cancel + ", Handled:" + e.Handled + ", GetDeferral:" + e.GetDeferral() + ", " + e.ToString());
        }

        private void CoreWebView2_DOMContentLoaded(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2DOMContentLoadedEventArgs e)
        {
            Debug.WriteLine("CoreWebView2_DOMContentLoaded invoked... NavigationId:" + e.NavigationId + ", " + e.ToString());
        }

        private void CoreWebView2_ContextMenuRequested(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2ContextMenuRequestedEventArgs e)
        {
            Debug.WriteLine("CoreWebView2_ContextMenuRequested invoked... MenuItems:" + e.MenuItems + ", Handled:" + e.Handled + ", ContextMenuTarget:" + e.ContextMenuTarget + ", " + e.ToString());
        }

        private void CoreWebView2_ContentLoading(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2ContentLoadingEventArgs e)
        {
            Debug.WriteLine("CoreWebView2_ContentLoading invoked... IsErrorPage:" + e.IsErrorPage + ", NavigationId:" + e.NavigationId + ", " + e.ToString());
            Debug.Assert(!e.IsErrorPage);
        }

        private void CoreWebView2_ClientCertificateRequested(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2ClientCertificateRequestedEventArgs e)
        {
            Debug.WriteLine("CoreWebView2_ClientCertificateRequested invoked... AllowedCertificateAuthorities:" + e.AllowedCertificateAuthorities + ", MutuallyTrustedCertificates:" + e.MutuallyTrustedCertificates + ", Cancel:" + e.Cancel + ", Handled:" + e.Handled + ", " + e.ToString());
        }

        private void CoreWebView2_BasicAuthenticationRequested(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2BasicAuthenticationRequestedEventArgs e)
        {
            Debug.WriteLine("CoreWebView2_BasicAuthenticationRequested invoked... Challenge:" + e.Challenge + ", Uri:" + e.Uri + ", Cancel:" + e.Cancel + ", " + e.ToString());
        }

        private void chkMuteWeb_Checked(object sender, RoutedEventArgs e)
        {
            if (webBrowser.CoreWebView2 != null)
            {
                webBrowser.CoreWebView2.IsMuted = true;
            }
        }

        private void chkMuteWeb_Unchecked(object sender, RoutedEventArgs e)
        {
            webBrowser.CoreWebView2.IsMuted = false;
        }
    }
}