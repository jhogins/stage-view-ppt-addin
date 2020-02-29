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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // nextSlideLabel
            // 
            this.nextSlideLabel.AutoSize = true;
            this.nextSlideLabel.BackColor = System.Drawing.Color.Black;
            this.nextSlideLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nextSlideLabel.ForeColor = System.Drawing.Color.White;
            this.nextSlideLabel.Location = new System.Drawing.Point(0, 682);
            this.nextSlideLabel.Name = "nextSlideLabel";
            this.nextSlideLabel.Size = new System.Drawing.Size(626, 97);
            this.nextSlideLabel.TabIndex = 0;
            this.nextSlideLabel.Text = "Next Slide Text";
            this.nextSlideLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.nextSlideLabel.DoubleClick += new System.EventHandler(this.StageViewForm_DoubleClick);
            // 
            // pictureBox
            // 
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(1263, 779);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            this.pictureBox.DoubleClick += new System.EventHandler(this.StageViewForm_DoubleClick);
            this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
            this.pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
            // 
            // StageViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1263, 779);
            this.ControlBox = false;
            this.Controls.Add(this.nextSlideLabel);
            this.Controls.Add(this.pictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "StageViewForm";
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
    }
}