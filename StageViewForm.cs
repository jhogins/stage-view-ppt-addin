﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.PowerPoint;
using Tools;
using Point = System.Drawing.Point;

namespace StageViewPpt
{
    public partial class StageViewForm : Form
    {
        private const int TimerInterval = 200;
        private const int DeactivateDelay = 2000;
        private SlideShowWindow slideShowWindow;
        private Timer refreshTimer;
        private Timer deactivateTimer;
        private DateTime? stageTimerEnd;
        private bool shouldClose = false;

        public StageViewForm()
        {
            InitializeComponent();
        }
        public StageViewForm(SlideShowWindow wn)
        {
            SlideShowWindow = wn;
            InitializeComponent();
            refreshTimer = new Timer();
            refreshTimer.Interval = TimerInterval;
            refreshTimer.Tick += OnDisplayTick;
            refreshTimer.Start();

            deactivateTimer = new Timer();
            deactivateTimer.Interval = DeactivateDelay;
            deactivateTimer.Tick += OnDeactivateTimer;

            LayoutObjects();
        }

        public string NextSlideText { get; set; }

        public SlideShowWindow SlideShowWindow 
        {
            get => slideShowWindow; 
            set
            {
                slideShowWindow = value;
                RelayoutWindow();
            }
        }

        private void LayoutObjects()
        {
            var font = new System.Drawing.Font(this.nextSlideLabel.Font.FontFamily, Properties.Settings.Default.FontSize);
            this.clockLabel.Font = font;
            this.timer.Font = font;
            this.nextSlideLabel.Font = font;
            this.nextSlideLabel.Location = new System.Drawing.Point(0, this.Height - nextSlideLabel.Height - nextSlideLabel.Padding.Size.Height);
            this.pictureBox.Location = System.Drawing.Point.Empty;
            this.pictureBox.Size = this.ClientSize;
            this.pictureBox.SendToBack();
        }

        private void OnDisplayTick(object sender, EventArgs e)
        {
            if (shouldClose)
            {
                this.Close();
                return;
            }
            if (SlideShowWindow == null)
                return;

            this.nextSlideLabel.Text = NextSlideText == null ? "" : NextSlideText.Split('\n').FirstOrDefault();

            GetWindowRect(SlideShowWindow.HWND, out RECT lpRect);

            var image = this.pictureBox.Image;
            var slideShowWidth = lpRect.Right - lpRect.Left;
            var slideShowHeight = lpRect.Bottom - lpRect.Top;
            if (slideShowWidth == 0 || slideShowHeight == 0)
                return;

            if (image == null || image.Width != slideShowWidth || image.Height != slideShowHeight)
            {
                image?.Dispose();
                image = new Bitmap(slideShowWidth, slideShowHeight);
                pictureBox.Image = image;
            }
            using (var graphics = Graphics.FromImage(image))
            {
                try
                {
                    graphics.CopyFromScreen((int)lpRect.Left, (int)lpRect.Top, 0, 0,
                        new Size((int)Math.Min(slideShowWidth, image.Width), (int)Math.Min(slideShowHeight, image.Height)),
                        CopyPixelOperation.SourceCopy);
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    //do nothing. This can happen, and it is ok to skip the copy
                }
            }

            clockLabel.Text = DateTime.Now.ToString("h:mm");
            UpdateTimer();

            this.Refresh();
        }

        private void UpdateTimer()
        {
            if (stageTimerEnd.HasValue)
            {
                var span = stageTimerEnd.Value - DateTime.Now;
                var duration = span.Duration();
                var timeString = $"{duration.Minutes}:{duration.Seconds}";
                if (duration.Hours > 0)
                    timeString = duration.ToString("h\\:mm\\:ss");
                else
                    timeString = duration.ToString("m\\:ss");

                if (span.TotalSeconds < 0)
                {
                    timeString = "-" + timeString;
                    timer.ForeColor = Color.Orange;
                }
                else
                {
                    timer.ForeColor = Color.White;
                }
                timer.Text = timeString;
                //align top-right
                timer.Location = new Point(this.Width - timer.Width - timer.Padding.Size.Width, timer.Padding.Size.Height);
            }
            else
            {
                timer.Text = string.Empty;
            }
        }

        internal void NotifyClose()
        {
            shouldClose = true;
        }

        private void OnDeactivateTimer(object sender, EventArgs e)
        {
            if (Form.ActiveForm == this)
            {
                SlideShowWindow.Activate();
            }
        }

        private void StageViewForm_Load(object sender, EventArgs e)
        {
        }

        private void RelayoutWindow()
        {
            if (!Visible)
                return;

            if (SlideShowWindow == null)
                return;

            SlideShowWindow.Activate();
            var targetDisplayId = Properties.Settings.Default.TargetDisplayId;
            var targetScreen = Screen.AllScreens.FirstOrDefault(s => DisplayQuery.DeviceFriendlyName(s) == targetDisplayId);
            if (targetScreen == null)
            {

                var centerPoint = new Point((int)(SlideShowWindow.Width / 2 + SlideShowWindow.Left),
                    (int)(SlideShowWindow.Height / 2 + SlideShowWindow.Height));

                //default to the first found non-primary screen that also does not contain the slide show window (the third monitor), or the primary screen if we can't find one.
                targetScreen = Screen.AllScreens.OrderByDescending(s => s.Bounds.Left).FirstOrDefault(s => !s.Primary && !s.Bounds.Contains(centerPoint));
                if (targetScreen == null)
                    targetScreen = Screen.PrimaryScreen;
            }

            if (!Properties.Settings.Default.StartWindowed)
            {
                this.Bounds = targetScreen.Bounds;
                //Run asynchronously to let the window update before maximizing
                this.BeginInvoke(new MethodInvoker(() => { this.WindowState = FormWindowState.Maximized; }));
            }
            else
            {
                this.Bounds = new Rectangle(targetScreen.Bounds.Left, targetScreen.Bounds.Top, 500, (int)(500 * SlideShowWindow.Height / SlideShowWindow.Width));
            }
        }

        internal void StartTimer(TimeSpan timeSpan)
        {
            stageTimerEnd = DateTime.Now + timeSpan;
        }

        internal void EndTimer()
        {
            stageTimerEnd = null;
        }

        private void StageViewForm_DoubleClick(object sender, EventArgs e)
        {
            ToggleMaximized();
        }

        private void ToggleMaximized()
        {
            deactivateTimer.Stop();
            deactivateTimer.Start();
            this.WindowState = this.WindowState == FormWindowState.Maximized
                ? FormWindowState.Normal
                : FormWindowState.Maximized;
        }

        private void StageViewForm_VisibleChanged(object sender, EventArgs e)
        {
            RelayoutWindow();
            if (refreshTimer != null)
                refreshTimer.Enabled = this.Visible;
        }

        private void StageViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (refreshTimer != null)
                refreshTimer.Enabled = false;
        }

        private void StageViewForm_SizeChanged(object sender, EventArgs e)
        {
            LayoutObjects();
        }

        private bool mouseDown;
        private Point lastLocation;

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            deactivateTimer.Stop();
            deactivateTimer.Start();
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(int hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        private void StageViewForm_Activated(object sender, EventArgs e)
        {
            deactivateTimer.Start();
        }
    }
}
