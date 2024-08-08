using BaseBookDownloader;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WpfIEBookDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
    public class _DocContents
    {
        public string sHtml = "";
    }
    
    public partial class WPFMainWindow : Window, IBaseMainWindow
    {
        public void HideScriptErrors(WebBrowser wb, bool hide)
        {
            var fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            var objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null)
            {
                wb.Loaded += (o, s) => HideScriptErrors(wb, hide); //In case we are to early
                return;
            }
            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }

        private void OnBookTypeSelectChagned(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (txtInitURL != null)
            {
                NovelTypeChangeEvent(App.Current.MainWindow.DataContext as WndContextData, cmbNovelType.SelectedIndex);
            }
        }

        private void OnMainWindowActivated(object sender, EventArgs e)
        {
            Debug.WriteLine("OnMainWindowActivated invoked...");
            if (txtInitURL != null)
            {
                NovelTypeChangeEvent(App.Current.MainWindow.DataContext as WndContextData, cmbNovelType.SelectedIndex);
            }
        }

        private void OnMainWindowClosing(object sender, CancelEventArgs e)
        {
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            Debug.WriteLine("OnMainWindowClosing invoked...");
            datacontext.UnloadPgm = true;
        }

        private void OnMainWindowUnloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnMainWindowUnloaded invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            datacontext.UnloadPgm = true;
            webBrowser.Dispose();
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

        private void OnSyncFromBrowser(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnSyncFromBrowser invoked...");
            txtCurURL.Text = webBrowser.Source.ToString();
        }

        public static void StartUrlOnWebBrowser(string strUrl)
        {
            if (!string.IsNullOrEmpty(strUrl.Trim()))
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
            txtCurURL.Text = webBrowser.Source.ToString();
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

        private void txtInitURL_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Debug.WriteLine("txtInitURL_PreviewKeyDown, SystemKey: " + e.SystemKey + ", Key:" + e.Key);
            btnInitURL.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private void txtCurURL_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Debug.WriteLine("txtCurURL_PreviewKeyDown, SystemKey: " + e.SystemKey + ", Key:" + e.Key);
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                btnCurURL.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void txtNextUrl_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Debug.WriteLine("txtNextUrl_PreviewKeyDown, SystemKey: " + e.SystemKey + ", Key:" + e.Key);
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                btnNextPage.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if (datacontext != null)
            {
                BaseBookDownload.WPF.WndSetting wndSetting = new BaseBookDownload.WPF.WndSetting(datacontext)
                {
                    Owner = this
                };
                wndSetting.ShowDialog();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Debug.WriteLine("Window_Closed invoked...");
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
            MainFrameWebLoadCompleted(sender, (sender as Microsoft.Web.WebView2.Wpf.WebView2)?.Source?.ToString() ?? "");
        }

        private void CoreWebView2_WebMessageReceived(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            //throw new NotImplementedException();
        }

    }
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
}