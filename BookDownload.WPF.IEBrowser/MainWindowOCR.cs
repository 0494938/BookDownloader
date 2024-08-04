using BaseBookDownloader;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TesseractOCR;  // https://github.com/tesseract-ocr/tessdata 
//using IronOcr;  //Has licnese limititaion. so quit.
namespace WpfIEBookDownloader
{
    public partial class WPFMainWindow : Window, IBaseMainWindow
    {
#if false //Has licnese limititaion. so quit.
        IronTesseract ocr = new IronTesseract();
        private void ClickIronOCR(object sender, RoutedEventArgs e)
        {
            ocr.Language = OcrLanguage.ChineseSimplified;
            using (OcrInput ocrInput = new OcrInput())
            {
                ocrInput.LoadImage("gcMangager.png");
                //ocrInput.LoadPdf("document.pdf");

                // Optionally Apply Filters if needed:
                // ocrInput.Deskew();  // use only if image not straight
                // ocrInput.DeNoise(); // use only if image contains digital noise

                var ocrResult = ocr.Read(ocrInput);
                Console.WriteLine(ocrResult.Text);
                txtAnalysizedContents.Text = ocrResult.Text;
            }
        }
#endif
        // Download language pack: https://github.com/tesseract-ocr/tessdata 
        TesseractOCR.Enums.Language GetOcrLanguage(int nIdx)
        {
            switch (nIdx)
            {
                case 0:
                    return TesseractOCR.Enums.Language.ChineseSimplified;
                //case 1:
                //    return TesseractOCR.Enums.Language.ChineseSimplifiedVertical;
                case 1:
                    return TesseractOCR.Enums.Language.ChineseTraditional;
                case 2:
                    return TesseractOCR.Enums.Language.Japanese;
                case 3:
                    return TesseractOCR.Enums.Language.JapaneseVertical;
                case 4:
                    return TesseractOCR.Enums.Language.English;
                case -1:
                    cmbOcrLanguage.SelectedIndex = 0;
                    return TesseractOCR.Enums.Language.ChineseSimplified;
                default:
                    return TesseractOCR.Enums.Language.ChineseSimplified;
            }
        }
        
        private void ClickTesseractOCR(object sender, RoutedEventArgs e)
        {
            //using var engine = new Engine(@"./tessdata", TesseractOCR.Enums.Language.English, EngineMode.Default);
            using (Engine engine = new Engine(@"./tessdata", /*TesseractOCR.Enums.Language.ChineseSimplified*/ GetOcrLanguage(cmbOcrLanguage.SelectedIndex), TesseractOCR.Enums.EngineMode.Default))
            {
                string sFile = "";
                if (imgOCR.Source is System.Windows.Media.Imaging.BitmapImage) {
                    sFile = (imgOCR.Source as System.Windows.Media.Imaging.BitmapImage)?.UriSource?.LocalPath??"";
                }
                else if (imgOCR.Source is ImageSource)
                {
                    sFile = ".\\gcMangager.png";
                }
                else
                {
                    sFile = ".\\gcMangager.png";
                }
                if (!string.IsNullOrEmpty(sFile))
                {
                    using (TesseractOCR.Pix.Image img = TesseractOCR.Pix.Image.LoadFromFile(sFile))
                    using (TesseractOCR.Page page = engine.Process(img))
                    {
                        Console.WriteLine("Mean confidence: {0}", page.MeanConfidence);
                        Console.WriteLine("Text: \r\n{0}", page.Text);
                        txtAnalysizedContents.Text = "OCR 信赖度:" + string.Format("{0:#.##}", page.MeanConfidence * 100) + "%\r\n" + page.Text;
                        StringReader sr = new StringReader(page.Text);
                        string? sLine = null;
                        StringBuilder sb = new StringBuilder();
                        while((sLine = sr.ReadLine()) !=null)
                        {
                            sLine = sLine.Trim(new char[] { ' ','\t','\r','\n'});
                            sLine = sLine.Replace(" ","").Replace("　","");
                            if (!string.IsNullOrEmpty(sLine))
                                sb.AppendLine(sLine);
                        }
                        txtAnalysizedContents.Text = "OCR 信赖度:" + string.Format("{0:#.##}", page.MeanConfidence * 100) + "%\r\n" + sb.ToString(); 
                    }
                }
            }
        }

        private void ClickOCR(object sender, RoutedEventArgs e)
        {
            ClickTesseractOCR(sender,e);
        }

        private void imgOCR_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    //imgOCR.ClearValue();
                    foreach (string file in files)
                    {
                        FileAttributes attr = File.GetAttributes(file);
                        if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
                        {
                            string sFileName = file;//Path.GetFileName(file);
                            if (sFileName.EndsWith(".png") || sFileName.EndsWith(".jpg") || sFileName.EndsWith(".jpeg") || sFileName.EndsWith(".bmp") || sFileName.EndsWith(".gif") 
                                || sFileName.EndsWith(".dib") || sFileName.EndsWith(".tif") || sFileName.EndsWith(".wmf") || sFileName.EndsWith(".eps") || sFileName.EndsWith(".pcx")
                                || sFileName.EndsWith(".pcd") || sFileName.EndsWith(".tga"))
                            {
                                imgOCR.Source = new BitmapImage(new Uri(sFileName, UriKind.RelativeOrAbsolute));
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void imgOCR_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
        }
    }
}