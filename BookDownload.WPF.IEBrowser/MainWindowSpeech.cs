using BaseBookDownloader;
using System.Diagnostics;
using System.Windows;
using System.Speech.Synthesis;
using System.Windows.Controls;
using System.Windows.Navigation;
using WebBrowser = System.Windows.Controls.WebBrowser;
using System.Globalization;

namespace WpfIEBookDownloader
{
    public partial class WPFMainWindow : Window, IBaseMainWindow
    {

        Thread? threadSpeech=null;
        bool bSpeaking = false;
        int nShift4Errr = -1;
        int nShift = 0;
        SpeechSynthesizer? synthesizer = null;

        private SpeechSynthesizer InitAndHookSpeech(SpeechSynthesizer? synthesizer=null)
        {
            if (synthesizer == null)
            {
                synthesizer = new SpeechSynthesizer();
            }
            synthesizer.SetOutputToDefaultAudioDevice();
            synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
            synthesizer.SpeakProgress += Synthesizer_SpeakProgress;
            return synthesizer;
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnMainWindowLoaded invoked...");
            //HideScriptErrors(webBrowser, true);
            if (cmbInstalledVoice.Items.Count == 0)
            {
                synthesizer = InitAndHookSpeech(synthesizer);
                var installed_voices = synthesizer.GetInstalledVoices();

                for (int i = 0; i < installed_voices.Count; i++)
                {
                    Console.WriteLine(GetVoiceInfoDesc(installed_voices[i].VoiceInfo));
                    cmbInstalledVoice.Items.Add(GetVoiceInfoDesc(installed_voices[i].VoiceInfo));
                }
                
            }
        }

        static string GetVoiceInfoDesc(VoiceInfo vi)
        {
            return vi.Culture.Name + " - " + vi.AdditionalInfo["Name"] + " (ID = " + vi.AdditionalInfo["Language"] + ", " + vi.AdditionalInfo["Age"] + ", " + vi.AdditionalInfo["Gender"] + ", " + vi.Culture + ")";
        }

        private void btnSpeechText(object sender, RoutedEventArgs e)
        {
            if (threadSpeech != null && (threadSpeech.ThreadState == System.Threading.ThreadState.Running || threadSpeech.ThreadState == System.Threading.ThreadState.WaitSleepJoin)) { 
                if(threadSpeech != null && synthesizer != null)
                {

                    threadSpeech = null;
                    Prompt curPromptBuilder = synthesizer.GetCurrentlySpokenPrompt();
                    synthesizer.SpeakAsyncCancel(curPromptBuilder);

                    bSpeaking = false;
                    btnSpeech.Content = "Speech Play";
                    nShift4Errr = -1;
                }
                    
            }
            else{
                if (!string.IsNullOrEmpty(txtAnalysizedContents.Text.Trim()))
                {
                    if (synthesizer == null)
                    {
                        synthesizer = InitAndHookSpeech(synthesizer);
                    }
                    PromptBuilder builder = new PromptBuilder();
                    if(cmbInstalledVoice.SelectedIndex == -1) 
                        builder.StartVoice(new CultureInfo("zh-Hans"));
                    else
                    {
                        string strCul = cmbInstalledVoice.Text;
                        builder.StartVoice(new CultureInfo(strCul.Substring(0, strCul.IndexOf(" - "))));
                    }
                    string strText = txtAnalysizedContents.SelectedText.Trim(new char[] { ' ','\t','\n','\r'});
                    if (!string.IsNullOrEmpty(strText) && strText.Length > 4)
                    {
                        nShift = txtAnalysizedContents.SelectionStart;
                    }
                    else {
                        nShift = 0; strText = "";
                    }

                    builder.AppendText(string.IsNullOrEmpty(strText)? txtAnalysizedContents.Text: strText);
                    builder.EndVoice();
                    
                    btnSpeech.Content = "Speech Stop";
                    bSpeaking = true;
                    nShift4Errr = -1;
                    threadSpeech = new Thread(() =>
                    {
                        try
                        {
                            synthesizer.Speak(builder);
                        }
                        catch (Exception ) { 
                        }
                        nShift4Errr = -1;
                        bSpeaking = false;
                        threadSpeech = null; 
                        this.Dispatcher.Invoke(() => { btnSpeech.Content = "Speech Play"; });
                        
                    });
                    threadSpeech.Start();
                }
            }
        }

        private void Synthesizer_SpeakProgress(object? sender, SpeakProgressEventArgs e)
        {
            if (nShift4Errr == -1 && e.CharacterPosition >=0) {
                nShift4Errr = e.CharacterPosition;
            }
            Debug.WriteLine("Synthesizer_SpeakProgress(bSpeaking:" + bSpeaking + ", Text:" + e.Text +
                ", CharacterPosition:" + (nShift4Errr == -1 ? e.CharacterPosition + nShift : e.CharacterPosition - nShift4Errr + nShift) +
                ", CharacterCount:" + e.CharacterCount +
                ", Cancelled:" + e.Cancelled +
                ")");
            this.Dispatcher.Invoke(() => {
                //txtAnalysizedContents.Select(e.CharacterPosition, e.CharacterCount);
                txtAnalysizedContents.Focus();
                txtAnalysizedContents.Select((nShift4Errr == -1 ? e.CharacterPosition + nShift : e.CharacterPosition - nShift4Errr + nShift), e.CharacterCount);
            });
        }

        private void Synthesizer_SpeakCompleted(object? sender, SpeakCompletedEventArgs e)
        {
            bSpeaking = false;
            Debug.WriteLine("Synthesizer_SpeakCompleted.  bSpeaking:" + bSpeaking);
        }
    }
}