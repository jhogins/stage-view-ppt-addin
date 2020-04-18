using System;
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
        private const int TimerInterval = 100;
        private const int DeactivateDelay = 2000;
        private SlideShowWindow slideShowWindow;
        private Timer refreshTimer;
        private Timer deactivateTimer;

        public StageViewForm()
        {
            InitializeComponent();
        }
        public StageViewForm(SlideShowWindow wn)
        {
            slideShowWindow = wn;
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

        public string NextSlideText
        {
            set => this.nextSlideLabel.Text = value.Split('\n').FirstOrDefault();
        }

        private void LayoutObjects()
        {
            var font = new System.Drawing.Font(this.nextSlideLabel.Font.FontFamily, Properties.Settings.Default.FontSize);
            this.clockLabel.Font = font;
            this.nextSlideLabel.Font = font;
            this.nextSlideLabel.Location = new System.Drawing.Point(0, this.Height - nextSlideLabel.Height - nextSlideLabel.Padding.Size.Height);
            this.pictureBox.Location = System.Drawing.Point.Empty;
            this.pictureBox.Size = this.ClientSize;
            this.pictureBox.SendToBack();
        }

        private void OnDisplayTick(object sender, EventArgs e)
        {
            if (slideShowWindow == null)
                return;

            GetWindowRect(slideShowWindow.HWND, out RECT lpRect);

            var image = this.pictureBox.Image;
            var slideShowWidth = lpRect.Right - lpRect.Left;
            var slideShowHeight = lpRect.Bottom - lpRect.Top;
            if (image == null || image.Width != slideShowWidth || image.Height != slideShowHeight)
            {
                image?.Dispose();
                image = new Bitmap(slideShowWidth, slideShowHeight);
                pictureBox.Image = image;
            }
            using (var grphics = Graphics.FromImage(image))
            {
                grphics.CopyFromScreen((int)lpRect.Left, (int)lpRect.Top, 0, 0, 
                    new Size((int)Math.Min(slideShowWidth, image.Width), (int)Math.Min(slideShowHeight, image.Height)),
                    CopyPixelOperation.SourceCopy);
            }

            clockLabel.Text = DateTime.Now.ToString("hh:mm");
            this.Refresh();
        }

        private void OnDeactivateTimer(object sender, EventArgs e)
        {
            if (Form.ActiveForm == this)
            {
                slideShowWindow.Activate();
            }
        }

        private void StageViewForm_Load(object sender, EventArgs e)
        {
            slideShowWindow.Activate();
            var targetDisplayId = Properties.Settings.Default.TargetDisplayId;
            var targetScreen = Screen.AllScreens.FirstOrDefault(s => ScreenInterrogatory.DeviceFriendlyName(s) == targetDisplayId);
            if (targetScreen == null)
            {
                if (slideShowWindow == null)
                    return;

                var centerPoint = new Point((int)(slideShowWindow.Width / 2 + slideShowWindow.Left),
                    (int)(slideShowWindow.Height / 2 + slideShowWindow.Height));

                //default to the first found non-primary screen that also does not contain the slide show window (the third monitor), or the primary screen if we can't find one.
                targetScreen = Screen.AllScreens.FirstOrDefault(s => !s.Primary && !s.Bounds.Contains(centerPoint)) ?? Screen.PrimaryScreen;
            }

            this.Bounds = new Rectangle(targetScreen.Bounds.Left, targetScreen.Bounds.Top, 500, (int)(500 * slideShowWindow.Height / slideShowWindow.Width));
            if (!Properties.Settings.Default.StartWindowed)
                this.ToggleMaximized();
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
