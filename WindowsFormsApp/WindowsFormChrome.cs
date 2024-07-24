using BaseBookDownload;
using CefSharp;
using CefSharp.BrowserSubprocess;
using CefSharp.WinForms;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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

        private void WindowsForm_Load(object sender, EventArgs e)
        {
            //cmbNovelType.SelectedIndex = cmbNovelType.FindString("4 无线电子书");
            cmbNovelType.SelectedIndex = 4;

            //browser.LoadUrl(cmbNovelType.Text.Trim());
            btnInitURL.PerformClick();  
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
            browser = new ChromiumWebBrowser(
                //txtInitURL.Text.Trim()
            );
            //this.Controls.Add(browser);
            this.scBottom.Panel1.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;
            browser.LoadingStateChanged += Browser_LoadingStateChanged;
            browser.FrameLoadStart += (sender, args) =>
            {
                //MainFrame has started to load, too early to access the DOM, you can add event listeners for DOMContentLoaded etc.
                Debug.WriteLine("browser.FrameLoadStart[Frame=" + (string.IsNullOrEmpty(args.Frame.Name)?"#NONAME": args.Frame.Name) + "] entered with (IsLoading = " + browser.IsLoading + ")...");
                UpdateStatusMsg(datacontext, "Start Frame Load : " + args.Url.ToString() + " ...", 0);

                if (args.Frame.IsMain)
                {
                    //const string script = "document.addEventListener('DOMContentLoaded', function(){ alert('DomLoaded'); });";
                    //args.Frame.ExecuteJavaScriptAsync(script);
                }
            };
            browser.FrameLoadEnd += new EventHandler<CefSharp.FrameLoadEndEventArgs>(Browser_FrameLoadComplete);
            browser.AddressChanged += new EventHandler<CefSharp.AddressChangedEventArgs>(Browser_AddressChanged);
            browser.IsBrowserInitializedChanged += new EventHandler(Browser_IsBrowserInitializedChanged);
            browser.JavascriptMessageReceived += new EventHandler<CefSharp.JavascriptMessageReceivedEventArgs>(Browser_JavascriptMessageReceived);
            browser.LocationChanged += new EventHandler(Browser_LocationChanged);
            browser.RegionChanged += new EventHandler(Browser_RegionChanged);

            browser.LoadError += new EventHandler<CefSharp.LoadErrorEventArgs>(Browser_LoadError);
            browser.TitleChanged+= new EventHandler<CefSharp.TitleChangedEventArgs>(Browser_TitleChanged);

            browser.ControlAdded += new ControlEventHandler(Browser_ControlAdded);
            browser.ControlRemoved += new ControlEventHandler(Browser_ControlRemoved);
            browser.BindingContextChanged += new EventHandler(Browser_BindingContextChanged);
        }


        private void splitContainer1_Panel2_Validated(object sender, EventArgs e)
        {

        }

        private void panelTop_Resize(object sender, EventArgs e)
        {
            txtLog.Height = panelTop.Height - txtLog.Top - 5;
            txtLog.Width = panelTop.Width - txtLog.Left * 2;
            txtInitURL.Width = panelTop.Width - txtInitURL.Left - 20 - 122;
            txtNextUrl.Width = panelTop.Width - txtNextUrl.Left - 20 - 122;
            btnAutoDownload.Left = txtInitURL.Left + txtInitURL.Width + 12;
            txtPages.Left = txtInitURL.Left + txtInitURL.Width + 12;
        }

        private void scResult_Panel1_Resize(object sender, EventArgs e)
        {
            txtHtml.Width = scResult.Panel1.Width - 2 * txtHtml.Top;
            txtContent.Width = scResult.Panel1.Width - 2 * txtContent.Top;
        }

        private void cmbNovelType_SelectedIndexChanged(object sender, EventArgs e)
        {
            NovelTypeChangeEvent(datacontext, cmbNovelType.SelectedIndex);
        }


        private void txtPages_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txtPages_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(txtPages.Text, "[^0-9]"))
            {
                txtPages.Text = txtPages.Text.Remove(txtPages.Text.Length - 1);
            }
        }

        private void flowLayoutPanel1_Resize(object sender, EventArgs e)
        {
            txtProgress.Left = statusPanel.Width - (txtProgress.Top + txtProgress.Width) - 5;
            txtStatus.Width  = statusPanel.Width - txtStatus.Left - 5;
        }
    }
}
