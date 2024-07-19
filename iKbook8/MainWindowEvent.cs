using HtmlAgilityPack;
using MSHTML;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

        private void OnBookTypeSelectChagned(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (txtInitURL != null)
            {
                NovelTyhpeChangeToIndex(cmbNovelType.SelectedIndex);
            }
        }

        private void OnMainWindowActivated(object sender, EventArgs e)
        {
            Debug.WriteLine("OnMainWindowActivated invoked...");
            if (txtInitURL != null)
            {
                NovelTyhpeChangeToIndex(cmbNovelType.SelectedIndex);
            }
        }


#pragma warning disable SYSLIB0001 // 型またはメンバーが旧型式です
        private void OnTestDecoding(object sender, RoutedEventArgs e)
        {
            Encoding[] arrAllEncodings = new[] {
                            Encoding.GetEncoding(932), Encoding.GetEncoding(10001), Encoding.GetEncoding(20290), Encoding.GetEncoding(20932), Encoding.GetEncoding(50220), Encoding.GetEncoding(50221), Encoding.GetEncoding(50222), Encoding.GetEncoding(51932),
                            Encoding.UTF8, Encoding.UTF7, Encoding.Unicode, Encoding.ASCII, Encoding.Latin1, Encoding.Default, Encoding.BigEndianUnicode, Encoding.UTF32, Encoding.GetEncoding(1200),Encoding.GetEncoding(1201),Encoding.GetEncoding(12000),Encoding.GetEncoding(12001),Encoding.GetEncoding(20127),Encoding.GetEncoding(28591),
                            Encoding.GetEncoding(936), Encoding.GetEncoding(950), Encoding.GetEncoding(20936),Encoding.GetEncoding(50227),Encoding.GetEncoding(51936),Encoding.GetEncoding(52936),Encoding.GetEncoding(54936),
                        };

            Encoding[] arrEncodings = arrAllEncodings.DistinctBy(t => t.CodePage).ToArray();
            
            string sTest = "马晓光听到有人在说话";
            byte[] bytes1 = Encoding.Unicode.GetBytes(sTest);
            for (int i = 0; i < arrEncodings.Length; i++)
            {
                string result1 = arrEncodings[i].GetString(bytes1);
                Debug.Print("<" + sTest + "> as " + arrEncodings[i].EncodingName +
                    " ->   <" + result1 + ">");
            }

        }

        private void OnTestEncoding(object sender, RoutedEventArgs e)
        {
            Encoding[] arrAllEncodings = new[] {
                            Encoding.GetEncoding(932), Encoding.GetEncoding(10001), Encoding.GetEncoding(20290), Encoding.GetEncoding(20932), Encoding.GetEncoding(50220), Encoding.GetEncoding(50221), Encoding.GetEncoding(50222), Encoding.GetEncoding(51932),
                            Encoding.UTF8, Encoding.UTF7, Encoding.Unicode, Encoding.ASCII, Encoding.Latin1, Encoding.Default, Encoding.BigEndianUnicode, Encoding.UTF32, Encoding.GetEncoding(1200),Encoding.GetEncoding(1201),Encoding.GetEncoding(12000),Encoding.GetEncoding(12001),Encoding.GetEncoding(20127),Encoding.GetEncoding(28591),
                            Encoding.GetEncoding(936), Encoding.GetEncoding(950), Encoding.GetEncoding(20936),Encoding.GetEncoding(50227),Encoding.GetEncoding(51936),Encoding.GetEncoding(52936),Encoding.GetEncoding(54936),
                        };

            Encoding[] arrEncodings = arrAllEncodings.DistinctBy(t => t.CodePage).ToArray();

            IHTMLDocument2? hTMLDocument2 = webBrowser.Document as IHTMLDocument2;
            IHTMLElement? elemBody = hTMLDocument2?.body as IHTMLElement;
            if (elemBody != null)
            {
                string? strBody = elemBody?.outerHTML ?? "";

                HtmlDocument html = new HtmlDocument();
                html.LoadHtml(strBody);
                HtmlNode body = html.DocumentNode.ChildNodes["BODY"];

                HtmlNodeCollection topDivs = body.SelectNodes(".//div[@class='muye-reader-content noselect']");
                if (topDivs.Count() > 0) {
                    HtmlNode topDiv = topDivs.First();

                    IEnumerable<HtmlNode> contentDivs = topDiv.Descendants().Where(n => n?.Name == "div");
                    if(contentDivs.Count() > 0)
                    {
                        HtmlNode contentDiv = contentDivs.First();

                        string sTestText = "";
                        int nMaxP = 5;
                        int nPs = 0;
                        foreach (HtmlNode pNode in contentDiv.ChildNodes)
                        {
                            if (pNode.Name == "p") { 
                                if (nPs <= nMaxP && !string.IsNullOrEmpty(pNode.InnerText.Trim()))
                                {
                                    sTestText += pNode.InnerText.Trim();
                                    nPs++;
                                }
                            }
                        }

                        FileStream outStream = new FileStream("test.txt", FileMode.Create, FileAccess.Write, FileShare.Read);

                        byte[] bytes = Encoding.Unicode.GetBytes(sTestText);
                        outStream.Write(bytes);
                        outStream.Close();

                        for (int i = 0; i < arrEncodings.Length; i++)
                            for (int j = 0; j < arrEncodings.Length; j++)
                            {
                                if (i != j)
                                {
                                    byte[] bytes1 = arrEncodings[i].GetBytes(sTestText);
                                    string result1 = arrEncodings[j].GetString(bytes1);
                                    Debug.Print(" " + arrEncodings[i].EncodingName + 
                                        " -> " + arrEncodings[j].EncodingName +
                                        " : " + result1);
                                }
                            }
                    }

                }

            }
        }
#pragma warning restore SYSLIB0001 // 型またはメンバーが旧型式です
    }
}