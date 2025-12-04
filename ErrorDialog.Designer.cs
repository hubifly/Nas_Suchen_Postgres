namespace Nas_Suchen
{
    partial class ErrorDialog
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox textBox1;
        private Button btnCopy;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            textBox1 = new TextBox();
            btnCopy = new Button();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Dock = DockStyle.Fill;
            textBox1.Font = new Font("Consolas", 10F);
            textBox1.Location = new Point(0, 0);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.ScrollBars = ScrollBars.Both;
            textBox1.Size = new Size(584, 421);
            textBox1.TabIndex = 0;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // btnCopy
            // 
            btnCopy.Dock = DockStyle.Bottom;
            btnCopy.Location = new Point(0, 421);
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new Size(584, 40);
            btnCopy.TabIndex = 1;
            btnCopy.Text = "Copy";
            btnCopy.Click += btnCopy_Click;
            // 
            // ErrorDialog
            // 
            ClientSize = new Size(584, 461);
            Controls.Add(textBox1);
            Controls.Add(btnCopy);
            MinimizeBox = false;
            Name = "ErrorDialog";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Error";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
