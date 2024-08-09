using BaseBookDownloader;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace WpfStreamDownloader
{
    public partial class WpfStreamMainWindow : Window
    {
#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
        public void BreakChapterAndConvertTextToMp3Thread(BaseWndContextData datacontext, string strNovelFilePath, string strMp3OutputPath)
        {
            Debug.WriteLine("covert " + strNovelFilePath + " to " + strMp3OutputPath + " ...");

            StringBuilder sbConvert=new StringBuilder();
            int nChapter = 0;
            const Int32 BufferSize = 2048;
            using (FileStream fileStream = File.OpenRead(strNovelFilePath))
            using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                this.UpdateStatusMsg(datacontext, "Analysize of Chapter in Novel file : " + strNovelFilePath, -1);
                String? sLine;
                long lLine = 0;
                string strChapterName = "";
                while ((sLine = streamReader.ReadLine()) != null && !datacontext.UnloadPgm)
                {
                    lLine++;
                    sLine = sLine.Trim();
                    if (!string.IsNullOrEmpty(sLine.Trim())){
                        //Match matche = Regex.Match(line, "[^第]*(第[]*[][^章节页]*");// Process line
                        string sPattern = "[\\t 　=-]*第[0-9０-９ \t一二三四五六七八九十百千万亿壹贰叁肆伍陆柒捌玖拾佰仟零]+[章节页]";
                        bool result = Regex.IsMatch(sLine, sPattern);
                        if (result)
                        {
                            if (nChapter > 0)
                            {
                                Debug.WriteLine("Handle Chapter: " + strChapterName);
                                ConvertOneChapterToMp3(datacontext, strChapterName, sbConvert.ToString(), strMp3OutputPath);
                                sbConvert = new StringBuilder();
                            }
                            nChapter++;
                            sbConvert.AppendLine(sLine);
                            strChapterName = sLine;
                            this.UpdateStatusMsg(datacontext, lLine.ToString() + " : " + sLine.Trim(), -1);
                        }
                        else
                        {
                            sbConvert.AppendLine(sLine);
                        }
                    }
                }
                if (sbConvert.Length > 0)
                {
                    Debug.WriteLine("Handle Chapter: " + strChapterName);
                    ConvertOneChapterToMp3(datacontext, strChapterName, sbConvert.ToString(), strMp3OutputPath);
                }
                this.UpdateStatusMsg(datacontext, "Finished Analysize Chapter and Convert Novel file : " + strNovelFilePath, -1);
            }
        }

        public void ConvertTextToMp3Thread(BaseWndContextData datacontext, string strNovelFilePath, string strMp3OutputPath)
        {
            Debug.WriteLine("covert " + strNovelFilePath + " to " + strMp3OutputPath + " ...");

            
            this.UpdateStatusMsg(datacontext, "Begin Convert Novel file : " + strNovelFilePath, -1);
            string strText= File.ReadAllText(strNovelFilePath);
            Util.ConvertTextToMp3(this, datacontext, Path.GetFileNameWithoutExtension(strNovelFilePath), strText, "zh-Hans", strMp3OutputPath);
            this.UpdateStatusMsg(datacontext, "Finished Convert Novel file : " + strNovelFilePath, -1);
        }

        public void ConvertOneChapterToMp3(BaseWndContextData datacontext, string strChapterName, string strText, string strMp3OutputPath) {
            string strFileName = strChapterName.Replace('\\', '￥').Replace('#', '＃')
                    .Replace('$', '＄').Replace('%', '％').Replace('!', '！').Replace('&', '＆').Replace('\'', '’').Replace('{', '｛')
                    .Replace('\"', '”').Replace('}', '｝').Replace(':', '：').Replace('\\', '￥').Replace('@', '＠').Replace('<', '＜').Replace('>', '＞').Replace('+', '＋')
                    .Replace('`', '‘').Replace('*', '＊').Replace('|', '｜').Replace('?', '？').Replace('=', '＝').Replace('/', '／');
            StreamWriter sw = new StreamWriter(strMp3OutputPath + strFileName + ".txt");
            sw.Write(strText);
            sw.Close();
            Util.ConvertTextToMp3(this, datacontext, strFileName, strText.ToString(), "zh-Hans", strMp3OutputPath);
        }
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
    }
}