using BaseBookDownloader;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace WpfBookDownloader
{
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CA1416 // プラットフォームの互換性を検証
#pragma warning disable CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class WindowsWPFChrome : Window, IBaseMainWindow
    {
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
            Debug.WriteLine("OnMainWindowClosing invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            datacontext.UnloadPgm = true;
        }

        private void OnMainWindowUnloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnMainWindowUnloaded invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            datacontext.UnloadPgm = true;
        }

        private static readonly Regex _regex = new Regex("[^0-9]+");

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void WebBrowser_TitleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine("WebBrowser_TitleChanged invoked("+e.NewValue+")...");

        }

        private void WebBrowser_Unloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("WebBrowser_Unloaded invoked...");
        }

        private void WebBrowser_LayoutUpdated(object sender, EventArgs e)
        {
            //Debug.WriteLine("WebBrowser_LayoutUpdated invoked...");
        }

        private void WebBrowser_JavascriptMessageReceived(object sender, CefSharp.JavascriptMessageReceivedEventArgs e)
        {
            Debug.WriteLine("WebBrowser_JavascriptMessageReceived("+e.ToString()+ ") invoked("+e.Message+")...");
        }

        private void WebBrowser_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            Debug.WriteLine("WebBrowser_RequestBringIntoView invoked("+e.ToString()+")...");
        }

        private void WebBrowser_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            Debug.WriteLine("WebBrowser_ManipulationStarted invoked("+e.ToString()+")...");
        }

        private void WebBrowser_ManipulationStarting(object sender, System.Windows.Input.ManipulationStartingEventArgs e)
        {
            Debug.WriteLine("WebBrowser_ManipulationStarting invoked("+e.ToString()+")...");
        }

        private void WebBrowser_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            Debug.WriteLine("WebBrowser_ManipulationCompleted invoked("+e.ToString()+")...");
        }

        private void WebBrowser_LoadError(object sender, CefSharp.LoadErrorEventArgs e)
        {
            WndContextData? datacontext = null;
            this.Dispatcher.Invoke(() =>
            {
                datacontext = App.Current.MainWindow.DataContext as WndContextData;
            });
            if(datacontext!=null)
                UpdateStatusMsg(datacontext, "WebBrowser_LoadError invoked(" + e.ErrorText+ ")...", -1);
        }

        private void WebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("WebBrowser_Loaded invoked...");
        }

        private void OnMainWindowUnloaded(object sender, CefSharp.LoadingStateChangedEventArgs e)
        {
            Debug.WriteLine("OnMainWindowUnloaded invoked(" + e.ToString() + ")...");
        }

        private void WebBrowser_LoadingStateChanged(object sender, CefSharp.LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading == true || e.Browser.HasDocument != true) {
                Debug.WriteLine("WebBrowser_LoadingStateChanged invoked (IsLoading:" + e.IsLoading + ", Url:" +e.Browser.MainFrame.Url +  ")...");
            }
            else
            {
                //MainFrameWebLoadCompleted(sender, e.Browser.MainFrame.Url);
                Debug.WriteLine("WebBrowser_LoadingStateChanged invoked (IsLoading:" + e.IsLoading + ", Url:" + e.Browser.MainFrame.Url + ")...");
            }
        }

    }
#pragma warning restore CA1416 // プラットフォームの互換性を検証
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
#pragma warning restore CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
}