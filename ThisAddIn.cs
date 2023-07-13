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
using System.Text.RegularExpressions;
using System.Globalization;

namespace StageViewPpt
{
    public partial class ThisAddIn
    {
        const int MaxNextTextLength = 180;
        private StageViewForm stageViewForm;
        private Thread stageViewThread;
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            Application.SlideShowNextClick += (wn, effect) => OnNextSlide(wn);
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
            ribbon.PowerPointApplication = this.GetHostItem<Microsoft.Office.Interop.PowerPoint.Application>(typeof(Microsoft.Office.Interop.PowerPoint.Application), "Application");
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

        private void OnNextSlide(PowerPoint.SlideShowWindow wn)
        {
            if (wn.View.State != PowerPoint.PpSlideShowState.ppSlideShowRunning)
            {
                ShowTextOnNextSlide(null);
                return;
            }
            var slide = wn.View.Slide;
            var slideIndex = slide.SlideIndex;
            var slides = wn.Presentation.Slides;
            if (slides.Count == slideIndex)
            {
                ShowTextOnNextSlide(null);
                return;
            }

            var nextSlide = slides[slideIndex + 1];

            ShowTextOnNextSlide(nextSlide);
            UpdateTimer(slides[slideIndex]);
        }

        private void UpdateTimer(PowerPoint.Slide slide)
        {
            var notes = GetSlideNotes(slide);
            if (string.IsNullOrEmpty(notes))
                return;

            notes = notes.ToLowerInvariant();
            var match = new Regex("([0-9\\:]+) timer").Match(notes);
            if (match.Success)
            {
                var timeString = match.Groups[1].Value;
                TimeSpan timeSpan;
                if (!TimeSpan.TryParseExact(timeString, "h\\:mm\\:ss", CultureInfo.CurrentCulture, out timeSpan))
                {
                    if (!TimeSpan.TryParseExact(timeString, "m\\:ss", CultureInfo.CurrentCulture, out timeSpan))
                        return;
                }

                stageViewForm?.StartTimer(timeSpan);
            }

            if (notes.Contains("stop timer"))
                stageViewForm?.EndTimer();
        }

        //return slide notes
        private string GetSlideNotes(PowerPoint.Slide slide)
        {
            if (slide.HasNotesPage == Office.MsoTriState.msoFalse)
                return null;

            var notesSlide = slide.NotesPage;
            var notes = notesSlide.Shapes.Cast<PowerPoint.Shape>()
                .Where(s => s.HasTextFrame == Office.MsoTriState.msoTrue)
                .Select(s => s.TextFrame.TextRange.Text)
                .FirstOrDefault();
            return notes;
        }

        private void ShowTextOnNextSlide(PowerPoint.Slide slide)
        {
            if (slide == null)
            {
                OnNextSlideTextChanged(string.Empty);
                return;
            }
            var textFrames = slide.Shapes.Cast<PowerPoint.Shape>()
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
