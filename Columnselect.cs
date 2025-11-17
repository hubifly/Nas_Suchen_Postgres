using System;
using System.Drawing;
using System.Windows.Forms;

namespace Nas_Suchen
{
    public class ColumnSelectorForm : Form
    {
        private CheckedListBox checkedListBox;
        private Button okButton;
        private Button cancelButton;

        public bool[] CheckedStates { get; private set; }

        public ColumnSelectorForm(string[] columnNames, bool[] currentStates)
        {
            Text = "Spaltenauswahl";
            Width = 300;
            Height = 400;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;

            checkedListBox = new CheckedListBox
            {
                Dock = DockStyle.Top,
                Height = 300,
                CheckOnClick = true
            };

            for (int i = 0; i < columnNames.Length; i++)
                checkedListBox.Items.Add(columnNames[i], currentStates[i]);

            okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Location = new Point(60, 320),
                Size = new Size(80, 30)
            };

            cancelButton = new Button
            {
                Text = "Abbrechen",
                DialogResult = DialogResult.Cancel,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Location = new Point(160, 320),
                Size = new Size(80, 30)
            };

            Controls.Add(checkedListBox);
            Controls.Add(okButton);
            Controls.Add(cancelButton);

            AcceptButton = okButton;
            CancelButton = cancelButton;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            CheckedStates = new bool[checkedListBox.Items.Count];
            for (int i = 0; i < checkedListBox.Items.Count; i++)
                CheckedStates[i] = checkedListBox.GetItemChecked(i);
            base.OnFormClosing(e);
        }
    }
}
