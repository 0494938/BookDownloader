using CefSharp;
using System;
using System.Diagnostics;
using System.Windows;

namespace BookDownloaderWpf
{
#pragma warning disable CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
    public partial class WindowsWPFChrome : Window
    {
        public WindowsWPFChrome()
        {
            InitializeComponent();
            InitBrowser();
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
            StartUrlOnWebBrowser(txtCurURL.Text.Trim());
        }

        private void PreviewPagesTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
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

        private void OnSyncFromBrowser(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnSyncFromBrowser invoked...");
            txtCurURL.Text = webBrowser.Address.ToString();
        }

        public void InitBrowser()
        {
            WndContextData? datacontext = App.Current.MainWindow.DataContext as WndContextData;
                              //webBrowser.LoadingStateChanged += Browser_LoadingStateChanged;
            webBrowser.FrameLoadStart += (sender, args) =>
            {
                //MainFrame has started to load, too early to access the DOM, you can add event listeners for DOMContentLoaded etc.
                string sFrameName="", strUrl="";
                bool bIsLoading = false;
                this.Dispatcher.Invoke(() => {
                    sFrameName = args.Frame.Name;
                    bIsLoading = webBrowser.IsLoading;
                    strUrl = webBrowser.GetMainFrame().Url;
                });
                Debug.WriteLine("browser.FrameLoadStart[Frame=" + (string.IsNullOrEmpty(sFrameName) ? "#NONAME" : sFrameName) + "] entered with (IsLoading = " + bIsLoading + ")...");
                UpdateStatusMsg(datacontext, "Start Frame Load : " + args.Url.ToString() + " ...", 0);

                if (args.Frame.IsMain)
                {
                    //const string script = "document.addEventListener('DOMContentLoaded', function(){ alert('DomLoaded'); });";
                    //args.Frame.ExecuteJavaScriptAsync(script);
                }
            };
            webBrowser.FrameLoadEnd += new EventHandler<CefSharp.FrameLoadEndEventArgs>(Browser_FrameLoadComplete);
            //webBrowser.AddressChanged += new EventHandler<CefSharp.AddressChangedEventArgs>(Browser_AddressChanged);
            //webBrowser.IsBrowserInitializedChanged += new EventHandler(Browser_IsBrowserInitializedChanged);
            //webBrowser.JavascriptMessageReceived += new EventHandler<CefSharp.JavascriptMessageReceivedEventArgs>(Browser_JavascriptMessageReceived);
            //webBrowser.LocationChanged += new EventHandler(Browser_LocationChanged);
            //webBrowser.RegionChanged += new EventHandler(Browser_RegionChanged);

            //webBrowser.LoadError += new EventHandler<CefSharp.LoadErrorEventArgs>(Browser_LoadError);
            //webBrowser.TitleChanged += new EventHandler<CefSharp.TitleChangedEventArgs>(Browser_TitleChanged);

            //webBrowser.ControlAdded += new ControlEventHandler(Browser_ControlAdded);
            //webBrowser.ControlRemoved += new ControlEventHandler(Browser_ControlRemoved);
            //webBrowser.BindingContextChanged += new EventHandler(Browser_BindingContextChanged);
            webBrowser.LoadingStateChanged += WebBrowser_LoadingStateChanged;
            webBrowser.Loaded += WebBrowser_Loaded;
            webBrowser.LoadError += WebBrowser_LoadError;
            webBrowser.ManipulationCompleted += WebBrowser_ManipulationCompleted;
            webBrowser.ManipulationStarting += WebBrowser_ManipulationStarting;
            webBrowser.ManipulationStarted += WebBrowser_ManipulationStarted;
            webBrowser.RequestBringIntoView += WebBrowser_RequestBringIntoView;
            webBrowser.JavascriptMessageReceived += WebBrowser_JavascriptMessageReceived;
            webBrowser.LayoutUpdated += WebBrowser_LayoutUpdated;
            webBrowser.LoadError += WebBrowser_LoadError;
            webBrowser.Unloaded += WebBrowser_Unloaded;
            webBrowser.TitleChanged += WebBrowser_TitleChanged;
        }

    }
#pragma warning restore CS8632 // '#nullable' 注釈コンテキスト内のコードでのみ、Null 許容参照型の注釈を使用する必要があります。
}
