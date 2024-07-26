using BaseBookDownload;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace BookDownloaderWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
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
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            Debug.WriteLine("OnMainWindowClosing invoked...");
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
    }
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
}