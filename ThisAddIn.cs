using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Office = Microsoft.Office.Core;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;

namespace StageViewPpt
{
    public partial class ThisAddIn
    {
        const int MaxNextTextLength = 180;
        private StageViewForm stageViewForm;
        private Thread stageViewThread;
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            Application.SlideShowNextClick += (wn, effect) => ShowTextOnNextSlide(wn);
            Application.SlideShowBegin += OnSlideShowBegin;
            Application.SlideShowEnd += OnSlideShowEnd;

            Properties.Settings.Default.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RefreshStageView();
        }

        private void RefreshStageView()
        {
            CloseStageView();
            if (Application.SlideShowWindows.Count > 0 && Properties.Settings.Default.ShowStageView)
            {
                stageViewThread = new Thread(() =>
                {
                    stageViewForm = new StageViewForm(null);

                    PowerPoint.SlideShowWindow slideShowWindow = null;
                    bool isReady = false;
                    do
                    {
                        try
                        {
                            //call an API to ensure the app is ready for API calls
                            slideShowWindow = Application.SlideShowWindows[1];
                            var unused = slideShowWindow.Active;
                            isReady = true;
                        }
                        catch (System.Exception)
                        {
                            isReady = false;
                            Thread.Sleep(200);
                        }
                    } while (!isReady);

                    stageViewForm.SlideShowWindow = slideShowWindow;
                    System.Windows.Forms.Application.Run(stageViewForm);
                    //var form = new Form1();
                    //System.Windows.Forms.Application.Run(form);
                });
                stageViewThread.Start();

                ////Showing a winform at this point causes Escape and Enter in the presenter view to have very strange behavior, so we delay the Show call.
                //Task.Run(async delegate
                //{
                //    await Task.Delay(200);
                //    stageViewForm.Show();
                //});
            }
        }

        private void CloseStageView()
        {
            if (stageViewForm != null)
            {
                stageViewForm.NotifyClose();
                stageViewForm = null;

                //stageViewForm.Close();
                //stageViewForm.Dispose();
                //stageViewForm = null;
            }
        }

        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            var ribbon = new OptionsRibbon();
            return ribbon;
        }

        private void OnSlideShowEnd(PowerPoint.Presentation pres)
        {
            CloseStageView();
        }

        private void OnSlideShowBegin(PowerPoint.SlideShowWindow wn)
        {
            RefreshStageView();
        }

        private void ShowTextOnNextSlide(PowerPoint.SlideShowWindow Wn)
        {
            if (Wn.View.State != PowerPoint.PpSlideShowState.ppSlideShowRunning)
            {
                OnNextSlideTextChanged(string.Empty);
                return;
            }
            var slide = Wn.View.Slide;
            var slideIndex = slide.SlideIndex;
            var slides = Wn.Presentation.Slides;
            if (slides.Count == slideIndex)
            {
                OnNextSlideTextChanged(string.Empty);
                return;
            }

            var nextSlide = slides[slideIndex + 1];
            var textFrames = nextSlide.Shapes.Cast<PowerPoint.Shape>()
                .Where(s => s.HasTextFrame == Office.MsoTriState.msoTrue)
                .OrderBy(s => s.Top)
                .Select(s => s.TextFrame)
                .Where(t => t.HasText == Office.MsoTriState.msoTrue)
                .ToList();

            var nextText = string.Join(" ", textFrames.Select(f => f.TextRange.Text.Replace('\n', ' ').Replace('\r', ' ')));
            if (nextText.Length > MaxNextTextLength)
                nextText = nextText.Substring(0, MaxNextTextLength);

            OnNextSlideTextChanged(nextText);
        }

        private void OnNextSlideTextChanged(string text)
        {
            if (stageViewForm == null)
                return;

            stageViewForm.NextSlideText = text;
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            stageViewForm?.Dispose();
            stageViewForm = null;

            Properties.Settings.Default.Save();
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
