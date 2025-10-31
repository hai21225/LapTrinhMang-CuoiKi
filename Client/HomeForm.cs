using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class HomeForm : Form
    {

        public HomeForm()
        {

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string playerName = textBox1.Text.Trim();

            if (string.IsNullOrWhiteSpace(playerName))
            {
                MessageBox.Show("Vui lòng nhập tên trước khi chơi!", "Thông báo");
                return;
            }

            this.Hide();

            var gameForm = new Form1(playerName);
            gameForm.FormClosed += (s, args) => this.Close();
            gameForm.Show();
        }
    }
}
