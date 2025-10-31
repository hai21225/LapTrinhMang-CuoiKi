namespace Client
{
    partial class HomeForm
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
            button1 = new Button();
            label1 = new Label();
            label2 = new Label();
            textBox1 = new TextBox();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Cursor = Cursors.Hand;
            button1.Font = new Font("Segoe UI", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.ForeColor = SystemColors.ControlText;
            button1.Location = new Point(247, 299);
            button1.Name = "button1";
            button1.Size = new Size(269, 93);
            button1.TabIndex = 0;
            button1.Text = "Play";
            button1.TextAlign = ContentAlignment.TopCenter;
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(12, 37);
            label1.Name = "label1";
            label1.Size = new Size(768, 54);
            label1.TabIndex = 1;
            label1.Text = "Welcome to the best game in the world";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(247, 116);
            label2.Name = "label2";
            label2.Size = new Size(266, 41);
            label2.TabIndex = 2;
            label2.Text = "Nhập tên vào đây";
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            textBox1.Location = new Point(247, 186);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(269, 43);
            textBox1.TabIndex = 3;
            textBox1.PlaceholderText = "Nhập tên của bạn...";
            // 
            // HomeForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(textBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(button1);
            Name = "HomeForm";
            Text = "HomeForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label label1;
        private Label label2;
        private TextBox textBox1;
    }
}