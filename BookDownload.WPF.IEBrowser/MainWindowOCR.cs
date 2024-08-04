using BaseBookDownloader;
using DynamicTesseract;
using IronOcr;
using System.Windows;
using TesseractOCR;
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
        private void ClickTesseractOCR(object sender, RoutedEventArgs e)
        {
            //using var engine = new Engine(@"./tessdata", TesseractOCR.Enums.Language.English, EngineMode.Default);
            using var engine = new Engine(@"./OcrData", TesseractOCR.Enums.Language.ChineseSimplified, TesseractOCR.Enums.EngineMode.Default);
            using var img = TesseractOCR.Pix.Image.LoadFromFile("gcMangager.png");
            using var page = engine.Process(img);
            Console.WriteLine("Mean confidence: {0}", page.MeanConfidence);
            Console.WriteLine("Text: \r\n{0}", page.Text);
            txtAnalysizedContents.Text = page.Text;
        }

        private void ClickOCR(object sender, RoutedEventArgs e)
        {
            ClickTesseractOCR(sender,e);
        }

    }
}