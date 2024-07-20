using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    [ComVisible(true)]
    public partial class WindowsForm : Form
    {
        public WindowsForm()
        {
            InitializeComponent();
        }

        private void btnBrowser_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtURL.Text.Trim()))
            {
                webBrowser.Navigate(txtURL.Text.Trim());
            }
        }

        private void webBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            Debug.WriteLine("webBrowser_Navigated --- " + e.Url.ToString());
        }

        private void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            Debug.WriteLine("webBrowser_Navigating >>> " + e.Url.ToString());
        }

        private void WindowsForm_Load(object sender, EventArgs e)
        {
            webBrowser.AllowWebBrowserDrop = false;
            webBrowser.IsWebBrowserContextMenuEnabled = false;
            webBrowser.WebBrowserShortcutsEnabled = false;
            webBrowser.ObjectForScripting = this;
            // Uncomment the following line when you are finished debugging.
            webBrowser.ScriptErrorsSuppressed = true;
            webBrowser.DocumentText =
            "<html>\n" +
            "  <head>\n" +
            "    <script>\n" +
            "    function test(message) { alert(message); }\n" +
            "    </script></head>\n" +
            "  <body>\n" +
            "    <button onclick=\"window.external.Test('called from script code')\">\n" +
            "     call client code from script code</button>\n" +
            "  </body>\n" +
            "</html>";
        }

        public void Test(String message)
        {
            MessageBox.Show(message, "client code");
        }
    }

}
