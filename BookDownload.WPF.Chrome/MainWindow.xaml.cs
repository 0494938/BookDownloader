using Microsoft.Win32;
using System.Diagnostics;
using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfBookDownloader
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Unloaded += MainWindow_Unloaded;
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnMainWindowUnloaded invoked...");
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            datacontext.UnloadPgm = true;
            webBrowser.Dispose();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnMainWindowLoaded invoked...");
            //HideScriptErrors(webBrowser, true);
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
            if (datacontext != null)
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\zdhe\\batchdownload\\1.0", false);
                datacontext.FileTempPath = (registryKey?.GetValue("FileTempPath") as string) ?? "";
                datacontext.FileSavePath = (registryKey?.GetValue("FileSavePath") as string) ?? "";
                registryKey?.Close();
            }
        }
    }
}
