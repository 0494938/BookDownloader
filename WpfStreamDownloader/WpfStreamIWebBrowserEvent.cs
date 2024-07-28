using Microsoft.Web.WebView2.WinForms;
using System.Diagnostics;
using System.IO;
using System.Text;
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
            //throw new NotImplementedException();
        }

        private void CoreWebView2_WebMessageReceived(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            //throw new NotImplementedException();
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
                    Debug.WriteLine("CoreWebView2.WebResourceResponseReceived -- response to redirect. status code: " + statusCode + ", Request Url: " + e.Response.Headers.GetHeader("Location"));
                }
                if (e.Request.Uri.ToString() == "https://www.youtube.com/watch?v=0pPPXeXKdfg")
                {
                    Debug.Assert(true);
                    string strUrl = e.Request.Uri.ToString();
                    e.Response.GetContentAsync().ContinueWith(t => {
                        StreamReader reader = new StreamReader(t.Result);
                        string html = reader.ReadToEnd();
                        //html = html.Replace("<html", "<HTML");
                        //byte[] byteArray = Encoding.UTF8.GetBytes(html);
                        //e.Request.Content = new MemoryStream(byteArray);
                        if (strUrl == "https://www.youtube.com/watch?v=0pPPXeXKdfg")
                        {
                            Debug.Assert(true);
                            //datacontext.WaitAndLaunchAnalsys
                        }
                    });
                }
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
            if (e.Request.Uri.ToString() == "https://www.youtube.com/watch?v=0pPPXeXKdfg")
            {
                Debug.Assert(true);
            }
        }
    }
}