using BaseBookDownload;
using CefSharp;
using CefSharp.BrowserSubprocess;
using CefSharp.WinForms;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    [ComVisible(true)]
    public partial class WindowsFormChrome : Form, IBaseMainWindow
    {
        public WindowsFormChrome()
        {
            InitializeComponent();
            InitBrowser();

            txtLog.Height = panelTop.Height - txtLog.Top - 5;
            txtLog.Width = panelTop.Width - txtLog.Left * 2;

        }

        private void btnBrowser_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtInitURL.Text.Trim()))
            {
                datacontext.PgmNaviUrl = txtInitURL.Text.Trim();
                browser.LoadUrl(txtInitURL.Text.Trim());
            }
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNextUrl.Text.Trim()))
            {
                datacontext.PgmNaviUrl = txtNextUrl.Text.Trim();
                browser.LoadUrl(txtNextUrl.Text.Trim());
            }

        }

        private void WindowsForm_Load(object sender, EventArgs e)
        {
            //cmbNovelType.SelectedText(cmbNovelType.Text);
            cmbNovelType.SelectedIndex = cmbNovelType.FindString("4 无线电子书");
        }

        public void Test(String message)
        {
            MessageBox.Show(message, "client code");
        }

        public ChromiumWebBrowser browser;

        public void InitBrowser()
        {
            Cef.Initialize(new CefSettings());
            datacontext.PgmNaviUrl = txtInitURL.Text.Trim();
            browser = new ChromiumWebBrowser(txtInitURL.Text.Trim());
            //this.Controls.Add(browser);
            this.scBottom.Panel1.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;
            browser.LoadingStateChanged += Browser_LoadingStateChanged;
            browser.FrameLoadStart += (sender, args) =>
            {
                //MainFrame has started to load, too early to access the DOM, you can add event listeners for DOMContentLoaded etc.
                if (args.Frame.IsMain)
                {
                    const string script = "document.addEventListener('DOMContentLoaded', function(){ alert('DomLoaded'); });";
                    args.Frame.ExecuteJavaScriptAsync(script);
                    Debug.WriteLine("browser.FrameLoadStart entered with (IsLoading = " + browser.IsLoading + ")...");
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

        private void panelTop_Resize(object sender, EventArgs e)
        {
            txtLog.Height = panelTop.Height - txtLog.Top - 5;
            txtLog.Width = panelTop.Width- txtLog.Left  *2 ;
        }

        private void scResult_Panel1_Resize(object sender, EventArgs e)
        {
            txtHtml.Width = scResult.Panel1.Width - 2 * txtHtml.Top;
            txtContent.Width = scResult.Panel1.Width - 2 * txtContent.Top;
        }

        private void cmbNovelType_SelectedIndexChanged(object sender, EventArgs e)
        {
            NovelTypeChangeEvent(cmbNovelType.SelectedIndex);
        }
    }

}
