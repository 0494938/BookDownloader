using BaseBookDownload;
using HtmlAgilityPack;
using MSHTML;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace BookDownloader
{
    public partial class WPFMainWindow : Window, IBaseMainWindow
    {
        public static string DumpString(byte[] bytes)
        {
            return string.Concat(bytes.Select(b => "0x" + b.ToString("X2") + ", ").ToArray());

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

#if false
            string sTest = "马晓光听到有人在说话";
            byte[] bytes1 = Encoding.Unicode.GetBytes(sTest);
            for (int i = 0; i < arrEncodings.Length; i++)
            {
                string result1 = arrEncodings[i].GetString(bytes1);
                Debug.Print("<" + sTest + "> as " + arrEncodings[i].EncodingName + " ->   <" + result1 + ">");
            }
#else
            byte[] bytes = new byte[] {
                0xee, 0x92, 0x8c, 0xe6, 0x99, 0x93, 0xee, 0x93, 0xa2, 0xee, 0x92, 0xb4, 0xee, 0x92, 0xa0, 0xee,
                0x92, 0x80, 0xee, 0x93, 0x82, 0xee, 0x8f, 0xa9, 0xee, 0x91, 0x8a, 0xee, 0x90, 0xbb, 0xef, 0xbc,
                0x8c, 0xe5, 0x8a, 0xaa, 0xee, 0x93, 0xbd, 0xee, 0x93, 0xb3, 0xee, 0x94, 0x8b, 0xe7, 0x9d, 0x81,
                0xee, 0x94, 0xa8, 0xee, 0x91, 0xb7, 0xee, 0x91, 0xb7, 0xef, 0xbc, 0x8c, 0xee, 0x91, 0xb8, 0xee,
                0x93, 0xa7, 0xee, 0x94, 0xb4, 0xee, 0x93, 0xb3, 0xee, 0x94, 0xa8, 0xe7, 0x9a, 0xae, 0xe6, 0xb2,
                0x89, 0xee, 0x94, 0xb3, 0xee, 0x94, 0xa9, 0xee, 0x92, 0x90, 0xef, 0xbc, 0x8c, 0xee, 0x94, 0xa9,
                0xe8, 0xae, 0xba, 0xee, 0x93, 0x8d, 0xee, 0x90, 0xb9, 0xe7, 0x9d, 0x81, 0xee, 0x94, 0xaa, 0xee,
                0x93, 0xbf, 0xee, 0x94, 0xa8, 0xe3, 0x80, 0x82
            };

            string result1 = Encoding.UTF8.GetString(bytes);
            string? decoded = DotDecodingUtil.DecodeDitStr(result1);

            Debug.WriteLine("Converted String <" + decoded?.ToString() + ">  =>  ");

            string sTest = "马晓光听到有人在说话";
            byte[] bytesTest = Encoding.UTF8.GetBytes(sTest);
            Debug.WriteLine("<" + sTest + ">  =>  " + DumpString(bytesTest));
            foreach (char c in sTest)
            {
                byte[] bytes1 = Encoding.UTF8.GetBytes(c.ToString());
                Debug.WriteLine("<" + c + ">  =>  " + DumpString(bytes1));
            }
#endif

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
                HtmlNode? body = BaseBookNovelContent.GetHtmlBody(html);

                HtmlNodeCollection? topDivs = body?.SelectNodes(".//div[@class='muye-reader-content noselect']");
                if (topDivs?.Count() > 0)
                {
                    HtmlNode topDiv = topDivs.First();

                    IEnumerable<HtmlNode> contentDivs = topDiv.Descendants().Where(n => n?.Name == "div");
                    if (contentDivs.Count() > 0)
                    {
                        HtmlNode contentDiv = contentDivs.First();

                        string sTestText = "";
                        int nMaxP = 5;
                        int nPs = 0;
                        foreach (HtmlNode pNode in contentDiv.ChildNodes)
                        {
                            if (pNode.Name == "p")
                            {
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