using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Office = Microsoft.Office.Core;
using System.Windows.Forms;

namespace StageViewPpt
{
    public partial class ThisAddIn
    {
        private StageViewForm stageViewForm;
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            this.Application.SlideShowOnNext += Application_SlideShowOnNext;
            this.Application.SlideShowOnPrevious += Application_SlideShowOnPrev;
            Application.SlideShowNextClick += (wn, effect) => ShowTextOnNextSlide(wn);
            Application.SlideShowNextBuild += (wn) => MessageBox.Show("NextBuild");
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
                stageViewForm = new StageViewForm(Application.SlideShowWindows[1]);
                stageViewForm.Show();
            }
        }

        private void CloseStageView()
        {
            if (stageViewForm != null)
            {
                stageViewForm.Close();
                stageViewForm.Dispose();
                stageViewForm = null;
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
            var slide = Wn.View.Slide;
            var slideIndex = slide.SlideIndex;
            var slides = Wn.Presentation.Slides;
            if (slides.Count == slideIndex)
            {
                OnNextSlideTextChanged(string.Empty);
                return;
            }

            var nextSlide = slides[slideIndex + 1];
            var sb = new StringBuilder();
            foreach (PowerPoint.Shape shape in nextSlide.Shapes)
            {
                if (shape.HasTextFrame == Office.MsoTriState.msoTrue)
                {
                    var textFrame = shape.TextFrame;
                    if (textFrame.HasText == Office.MsoTriState.msoTrue)
                    {
                        sb.AppendLine(textFrame.TextRange.Text);
                    }
                }
            }

            OnNextSlideTextChanged(sb.ToString());
        }

        private void OnNextSlideTextChanged(string text)
        {
            if (stageViewForm == null)
                return;

            stageViewForm.NextSlideText = text;
        }

        private void Application_SlideShowOnNext(PowerPoint.SlideShowWindow Wn)
        {
            System.Windows.Forms.MessageBox.Show("Slide next!");
        }

        private void Application_SlideShowOnPrev(PowerPoint.SlideShowWindow Wn)
        {
            System.Windows.Forms.MessageBox.Show("Slide prev!");
        }

        private void Application_SlideSelectionChanged(PowerPoint.SlideRange SldRange)
        {
            System.Windows.Forms.MessageBox.Show("Slide changed!");
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            this.Application.SlideSelectionChanged -= Application_SlideSelectionChanged;
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
