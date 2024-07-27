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
                AnalysisURL(webBrowser.Address.ToString(), false);
            }
        }

    }
#pragma warning restore CA1416 // プラットフォームの互換性を検証
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
#pragma warning restore CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
}