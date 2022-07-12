using System;
using System.Windows.Forms;

namespace Kucoin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.key = textBox1.Text;
            form2.secret = textBox2.Text;
            form2.pass = textBox3.Text;
            form2.Show();
        }
    }
}
