using CefSharp;
using CefSharp.WinForms;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    [ComVisible(true)]
    public partial class WindowsFormChrome : Form
    {
        public WindowsFormChrome()
        {
            InitializeComponent();
            InitBrowser();
        }

        private void btnBrowser_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtURL.Text.Trim()))
            {
                browser.LoadUrl(txtURL.Text.Trim());
            }
        }

       

        private void WindowsForm_Load(object sender, EventArgs e)
        {
        }

        public void Test(String message)
        {
            MessageBox.Show(message, "client code");
        }

        public ChromiumWebBrowser browser;

        public void InitBrowser()
        {
            Cef.Initialize(new CefSettings());
            browser = new ChromiumWebBrowser("www.google.com");
            //this.Controls.Add(browser);
            this.splitContainer1.Panel2.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;
            browser.LoadingStateChanged += Browser_LoadingStateChanged;
            browser.FrameLoadStart += (sender, args) =>
            {
                //MainFrame has started to load, too early to access the DOM, you can add event listeners for DOMContentLoaded etc.
                if (args.Frame.IsMain)
                {
                    const string script = "document.addEventListener('DOMContentLoaded', function(){ alert('DomLoaded'); });";

                    args.Frame.ExecuteJavaScriptAsync(script);
                }
            };
            browser.FrameLoadEnd += new EventHandler<CefSharp.FrameLoadEndEventArgs>(Browser_FrameLoadComplete);
        }


        private void splitContainer1_Panel2_Validated(object sender, EventArgs e)
        {

        }
        private void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading == false)
            {
                Debug.WriteLine("Load Completed ....");
                //e.Browser.ViewSource();
                //Debug.WriteLine();
            }
            else
            {
                Debug.WriteLine("still loading ....");
            }
        }
    }

}
