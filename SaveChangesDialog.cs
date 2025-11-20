using System;
using System.Drawing;
using System.Windows.Forms;

namespace Nas_Suchen
{
    public class SaveChangesDialog : Form
    {
        public enum Result
        {
            Save,
            DontSave,
            Cancel
        }

        public Result DialogResultCustom { get; private set; }

        public SaveChangesDialog()
        {
            Text = "Schließen";
            Width = 280;
            Height = 220;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;

            var label = new Label
            {
                Text = "Änderungen speichern?",
                AutoSize = true,
                Location = new Point(20, 20)
            };

            var btnSave = new Button
            {
                Text = "Schließen + Speichern",
                Width = 200,
                Location = new Point(40, 60)
            };
            btnSave.Click += (s, e) => { DialogResultCustom = Result.Save; Close(); };

            var btnDontSave = new Button
            {
                Text = "Schließen ohne",
                Width = 200,
                Location = new Point(40, 100)
            };
            btnDontSave.Click += (s, e) => { DialogResultCustom = Result.DontSave; Close(); };

            var btnCancel = new Button
            {
                Text = "Abbrechen",
                Width = 200,
                Location = new Point(40, 140)
            };
            btnCancel.Click += (s, e) => { DialogResultCustom = Result.Cancel; Close(); };

            Controls.Add(label);
            Controls.Add(btnSave);
            Controls.Add(btnDontSave);
            Controls.Add(btnCancel);
        }
    }
}
