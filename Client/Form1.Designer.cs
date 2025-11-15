namespace Client
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            _timer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(1280, 720); // 1280, 720
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // không cho kéo dãn
            this.MaximizeBox = false; // tắt nút phóng to
            this.MinimizeBox = true;  // nếu muốn vẫn cho thu nhỏ thì để true
            this.StartPosition = FormStartPosition.CenterScreen; //
            KeyPreview = true;
            Name = "Form1";
            FormClosed += Form1_FormClosed;
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Timer _timer;
    }
}
