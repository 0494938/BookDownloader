using BaseBookDownloader;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls.Primitives;

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

        private void OnCheckPrettyHtmlChanged(object sender, RoutedEventArgs e)
        {
            if (chkboxPrettyHtml.IsChecked == false)
                GetBrowserDocAndPutToCtrl();
            else
                GetBrowserDocAndPrettyToCtrl();
        }

        private void btnAnalysisCurURL_Click(object sender, RoutedEventArgs e)
        {
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if ((datacontext != null))
            {
                datacontext.DictDownloadStatus.Clear();
                datacontext.DictHandledContentsForDupCheck.Clear();
                AnalysisURL(webBrowser.Address.ToString(), false);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if (datacontext != null)
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\zdhe\\batchdownload\\1.0", false);
                datacontext.FileTempPath = (registryKey?.GetValue("FileTempPath") as string) ?? "";
                datacontext.FileSavePath = (registryKey?.GetValue("FileSavePath") as string) ?? "";
                registryKey?.Close();
            }

            if (cmbNovelType.SelectedIndex == -1)
            {
                NovelTypeChangeEvent(App.Current.MainWindow.DataContext as WndContextData, 0);
            }
            else
            {
                NovelTypeChangeEvent(App.Current.MainWindow.DataContext as WndContextData, cmbNovelType.SelectedIndex);
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnMainWindowUnloaded invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if (datacontext != null) { datacontext.UnloadPgm = true; }
            webBrowser.Dispose();
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

        private void Window_Closed(object sender, EventArgs e)
        {
            Debug.WriteLine("Window_Closed invoked...");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Debug.WriteLine("Window_Closing invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if (datacontext != null) { datacontext.UnloadPgm = true; }
        }
    }
#pragma warning restore CA1416 // プラットフォームの互換性を検証
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
#pragma warning restore CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
}