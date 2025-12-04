using System;
using System.Windows.Forms;

namespace Nas_Suchen
{
    public partial class ErrorDialog : Form
    {
        public ErrorDialog(string title, string message)
        {
            InitializeComponent();

            this.Text = title;
            textBox1.Text = message;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox1.Text);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
