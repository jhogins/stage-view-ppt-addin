namespace StageViewPpt
{
    partial class StageViewForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.nextSlideLabel = new System.Windows.Forms.Label();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.clockLabel = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // nextSlideLabel
            // 
            this.nextSlideLabel.AutoSize = true;
            this.nextSlideLabel.BackColor = System.Drawing.Color.Black;
            this.nextSlideLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 40F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nextSlideLabel.ForeColor = System.Drawing.Color.White;
            this.nextSlideLabel.Location = new System.Drawing.Point(0, 345);
            this.nextSlideLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.nextSlideLabel.Name = "nextSlideLabel";
            this.nextSlideLabel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.nextSlideLabel.Size = new System.Drawing.Size(393, 66);
            this.nextSlideLabel.TabIndex = 0;
            this.nextSlideLabel.Text = "Next Slide Text";
            this.nextSlideLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.nextSlideLabel.DoubleClick += new System.EventHandler(this.StageViewForm_DoubleClick);
            // 
            // pictureBox
            // 
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(632, 405);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            this.pictureBox.DoubleClick += new System.EventHandler(this.StageViewForm_DoubleClick);
            this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
            this.pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
            // 
            // clockLabel
            // 
            this.clockLabel.AutoSize = true;
            this.clockLabel.BackColor = System.Drawing.Color.Black;
            this.clockLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 45F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clockLabel.ForeColor = System.Drawing.Color.White;
            this.clockLabel.Location = new System.Drawing.Point(0, 0);
            this.clockLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.clockLabel.Name = "clockLabel";
            this.clockLabel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.clockLabel.Size = new System.Drawing.Size(179, 72);
            this.clockLabel.TabIndex = 2;
            this.clockLabel.Text = "00:00";
            this.clockLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timer
            // 
            this.timer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.timer.AutoSize = true;
            this.timer.BackColor = System.Drawing.Color.Black;
            this.timer.Font = new System.Drawing.Font("Microsoft Sans Serif", 45F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timer.ForeColor = System.Drawing.Color.White;
            this.timer.Location = new System.Drawing.Point(450, 0);
            this.timer.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.timer.Name = "timer";
            this.timer.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.timer.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.timer.Size = new System.Drawing.Size(179, 72);
            this.timer.TabIndex = 3;
            this.timer.Text = "00:00";
            this.timer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // StageViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(632, 405);
            this.ControlBox = false;
            this.Controls.Add(this.timer);
            this.Controls.Add(this.clockLabel);
            this.Controls.Add(this.nextSlideLabel);
            this.Controls.Add(this.pictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "StageViewForm";
            this.Activated += new System.EventHandler(this.StageViewForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StageViewForm_FormClosing);
            this.Load += new System.EventHandler(this.StageViewForm_Load);
            this.SizeChanged += new System.EventHandler(this.StageViewForm_SizeChanged);
            this.VisibleChanged += new System.EventHandler(this.StageViewForm_VisibleChanged);
            this.DoubleClick += new System.EventHandler(this.StageViewForm_DoubleClick);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label nextSlideLabel;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label clockLabel;
        private System.Windows.Forms.Label timer;
    }
}