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
            Width = 250;
            Height = 450;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;

            checkedListBox = new CheckedListBox
            {
                Dock = DockStyle.Top,
                Height = 350,
                CheckOnClick = true
            };

            for (int i = 0; i < columnNames.Length; i++)
                checkedListBox.Items.Add(columnNames[i], currentStates[i]);

            okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Location = new Point(30, Height - 80),
                Size = new Size(60, 20)
            };

            cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Location = new Point(130, Height - 80),
                Size = new Size(60, 20)
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
