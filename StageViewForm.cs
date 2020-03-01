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
using Point = System.Drawing.Point;

namespace StageViewPpt
{
    public partial class StageViewForm : Form
    {
        private const int TimerInterval = 100;
        private SlideShowWindow slideShowWindow;
        private Timer refreshTimer;

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

            LayoutObjects();
        }

        public string NextSlideText
        {
            set => this.nextSlideLabel.Text = value.Split('\n').FirstOrDefault();
        }

        private void LayoutObjects()
        {
            this.nextSlideLabel.Location = new System.Drawing.Point(0, this.Height - nextSlideLabel.Height);
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
                grphics.CopyFromScreen((int)slideShowWindow.Left, (int)slideShowWindow.Top, 0, 0, 
                    new Size((int)Math.Min(slideShowWidth, image.Width), (int)Math.Min(slideShowHeight, image.Height)),
                    CopyPixelOperation.SourceCopy);
            }
            this.Refresh();
        }

        private void StageViewForm_Load(object sender, EventArgs e)
        {
            var targetDisplayId = Properties.Settings.Default.TargetDisplayId;
            var targetScreen = Screen.AllScreens.FirstOrDefault(s => s.DeviceName == targetDisplayId);
            if (targetScreen == null)
            {
                if (slideShowWindow == null)
                    return;

                var centerPoint = new Point((int)(slideShowWindow.Width / 2 + slideShowWindow.Left),
                    (int)(slideShowWindow.Height / 2 + slideShowWindow.Height));

                targetScreen = Screen.AllScreens.FirstOrDefault(s => !s.Bounds.Contains(centerPoint)) ?? Screen.PrimaryScreen;
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

        //public const int WM_NCLBUTTONDOWN = 0xA1;
        //public const int HT_CAPTION = 0x2;

        //[System.Runtime.InteropServices.DllImport("user32.dll")]
        //public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        //[System.Runtime.InteropServices.DllImport("user32.dll")]
        //public static extern bool ReleaseCapture();

        //private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        ReleaseCapture();
        //        SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        //    }
        //}
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
    }
}
