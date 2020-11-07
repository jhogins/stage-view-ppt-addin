using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Tools;
using WindowsDisplayAPI;
using Office = Microsoft.Office.Core;

// TODO:  Follow these steps to enable the Ribbon (XML) item:

// 1: Copy the following code block into the ThisAddin, ThisWorkbook, or ThisDocument class.


// 2. Create callback methods in the "Ribbon Callbacks" region of this class to handle user
//    actions, such as clicking a button. Note: if you have exported this Ribbon from the Ribbon designer,
//    move your code from the event handlers to the callback methods and modify the code to work with the
//    Ribbon extensibility (RibbonX) programming model.

// 3. Assign attributes to the control tags in the Ribbon XML file to identify the appropriate callback methods in your code.  

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help.


namespace StageViewPpt
{
    [ComVisible(true)]
    public class OptionsRibbon : Office.IRibbonExtensibility
    {
        int lastMonitorCount = 0;
        private Office.IRibbonUI ribbon;
        private Timer screenCheckTimer;

        public OptionsRibbon()
        {
        }

        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("StageViewPpt.OptionsRibbon.xml");
        }

        #endregion

        #region Ribbon Callbacks
        //Create callback methods here. For more information about adding callback methods, visit https://go.microsoft.com/fwlink/?LinkID=271226


        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;
            screenCheckTimer = new Timer();
            screenCheckTimer.Interval = 2000;
            screenCheckTimer.Tick += ScreenCheckTimer_Tick;
            screenCheckTimer.Start();
        }

        private void ScreenCheckTimer_Tick(object sender, EventArgs e)
        {
            var monitorCount = MonitorChoiceCount;
            if (monitorCount != lastMonitorCount)
            {
                lastMonitorCount = monitorCount;
                this.ribbon.Invalidate();
            }
        }

        public void OnEnableStageView(Office.IRibbonControl control, bool pressed)
        {
            Properties.Settings.Default.ShowStageView = pressed;
            this.ribbon.Invalidate();
        }
        public bool GetEnableStageViewPressed(Office.IRibbonControl control)
        {
            return Properties.Settings.Default.ShowStageView;
        }

        public bool AreSubcontrolsEnabled(Office.IRibbonControl control)
        {
            return Properties.Settings.Default.ShowStageView;
        }

        public int GetMonitorCount(Office.IRibbonControl control)
        {
            return MonitorChoiceCount;
        }

        public string GetMonitorLabel(Office.IRibbonControl control, int index)
        {
            return GetMonitorDropdownIds().ToArray()[index];
        }

        public string GetMonitorID(Office.IRibbonControl control, int index)
        {
            return GetMonitorDropdownIds().ToArray()[index];
        }

        public string GetSelectedMonitorID(Office.IRibbonControl control)
        {
            var currentId = Properties.Settings.Default.TargetDisplayId;
            if (!GetMonitorIds().Contains(currentId))
                return "Automatic";
            
            return currentId;
        }

        private IEnumerable<string> GetMonitorDropdownIds()
        {
            return new[] {"Automatic"}.Concat(GetMonitorIds());
        }

        private IEnumerable<string> GetMonitorIds()
        {
            return Display.GetDisplays().Select(d => d.SmartDeviceName());
            //return Screen.AllScreens.Select(s => DisplayQuery.DeviceFriendlyName(s));
        }

        int MonitorChoiceCount => Screen.AllScreens.Length + 1;

        public void OnMonitorSwitch(Office.IRibbonControl control, string id, int index)
        {
            Properties.Settings.Default.TargetDisplayId = id;
            
        }

        public void OnStartWindowed(Office.IRibbonControl control, bool pressed)
        {
            Properties.Settings.Default.StartWindowed = pressed;
        }

        public bool GetStartWindowedPressed(Office.IRibbonControl control)
        {
            return Properties.Settings.Default.StartWindowed;
        }

        public string GetFontSizeText(Office.IRibbonControl control) => Properties.Settings.Default.FontSize.ToString();

        public void OnFontSizeChanged(Office.IRibbonControl control, string text)
        {
            if (int.TryParse(text, out var fontSize) && fontSize > 0)
                Properties.Settings.Default.FontSize = fontSize;
            else
                this.ribbon.Invalidate();
        }

        #endregion

        #region Helpers

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
