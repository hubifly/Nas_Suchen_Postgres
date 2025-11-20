namespace Nas_Suchen
{
    partial class Nas_Suche
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Nas_Suche));
            dataGridView1 = new DataGridView();
            btn_sort = new Button();
            btn_clear = new Button();
            btn_close = new Button();
            pic_anz = new TextBox();
            btn_preview_db = new Button();
            lbl_max = new Label();
            lbl_records = new Label();
            txt_records = new TextBox();
            txt_max_recs = new TextBox();
            lbl_max_recs = new Label();
            btn_help = new Button();
            btnColumnSelect = new Button();
            chk_original = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Dock = DockStyle.Top;
            dataGridView1.Location = new Point(0, 0);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(1188, 389);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
            // 
            // btn_sort
            // 
            btn_sort.Location = new Point(12, 399);
            btn_sort.Name = "btn_sort";
            btn_sort.Size = new Size(124, 22);
            btn_sort.TabIndex = 1;
            btn_sort.Text = "Suchen/Sortieren";
            btn_sort.UseVisualStyleBackColor = true;
            btn_sort.Click += btn_sort_Click;
            // 
            // btn_clear
            // 
            btn_clear.Location = new Point(151, 399);
            btn_clear.Name = "btn_clear";
            btn_clear.Size = new Size(155, 22);
            btn_clear.TabIndex = 2;
            btn_clear.Text = "Sort/Filter löschen";
            btn_clear.UseVisualStyleBackColor = true;
            btn_clear.Click += btn_clear_Click;
            // 
            // btn_close
            // 
            btn_close.Location = new Point(660, 423);
            btn_close.Name = "btn_close";
            btn_close.Size = new Size(114, 22);
            btn_close.TabIndex = 3;
            btn_close.Text = "Close";
            btn_close.UseVisualStyleBackColor = true;
            btn_close.Click += btn_close_Click;
            // 
            // pic_anz
            // 
            pic_anz.Location = new Point(525, 427);
            pic_anz.Name = "pic_anz";
            pic_anz.Size = new Size(52, 23);
            pic_anz.TabIndex = 6;
            pic_anz.Text = "30";
            pic_anz.TextAlign = HorizontalAlignment.Center;
            pic_anz.TextChanged += pic_anz_TextChanged;
            // 
            // btn_preview_db
            // 
            btn_preview_db.Location = new Point(445, 395);
            btn_preview_db.Name = "btn_preview_db";
            btn_preview_db.Size = new Size(118, 31);
            btn_preview_db.TabIndex = 7;
            btn_preview_db.Text = "Preview aus DB";
            btn_preview_db.UseVisualStyleBackColor = true;
            btn_preview_db.Click += button1_Click;
            // 
            // lbl_max
            // 
            lbl_max.Location = new Point(440, 428);
            lbl_max.Name = "lbl_max";
            lbl_max.Size = new Size(89, 17);
            lbl_max.TabIndex = 8;
            lbl_max.Text = "Preview";
            lbl_max.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbl_records
            // 
            lbl_records.Location = new Point(11, 428);
            lbl_records.Name = "lbl_records";
            lbl_records.Size = new Size(53, 17);
            lbl_records.TabIndex = 9;
            lbl_records.Text = "Max. in Grid";
            lbl_records.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txt_records
            // 
            txt_records.Location = new Point(65, 424);
            txt_records.Name = "txt_records";
            txt_records.Size = new Size(46, 23);
            txt_records.TabIndex = 10;
            txt_records.Text = "500";
            txt_records.TextAlign = HorizontalAlignment.Center;
            txt_records.TextChanged += txt_records_TextChanged;
            // 
            // txt_max_recs
            // 
            txt_max_recs.Location = new Point(268, 426);
            txt_max_recs.Name = "txt_max_recs";
            txt_max_recs.Size = new Size(53, 23);
            txt_max_recs.TabIndex = 11;
            txt_max_recs.TextAlign = HorizontalAlignment.Center;
            // 
            // lbl_max_recs
            // 
            lbl_max_recs.Location = new Point(167, 428);
            lbl_max_recs.Name = "lbl_max_recs";
            lbl_max_recs.Size = new Size(97, 17);
            lbl_max_recs.TabIndex = 12;
            lbl_max_recs.Text = "vorhanden";
            lbl_max_recs.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btn_help
            // 
            btn_help.Location = new Point(660, 401);
            btn_help.Name = "btn_help";
            btn_help.Size = new Size(94, 20);
            btn_help.TabIndex = 13;
            btn_help.Text = "Hilfe";
            btn_help.UseVisualStyleBackColor = true;
            btn_help.Click += btn_help_Click;
            // 
            // btnColumnSelect
            // 
            btnColumnSelect.Location = new Point(818, 404);
            btnColumnSelect.Name = "btnColumnSelect";
            btnColumnSelect.Size = new Size(95, 21);
            btnColumnSelect.TabIndex = 14;
            btnColumnSelect.Text = "Spalten wählen";
            btnColumnSelect.UseVisualStyleBackColor = true;
            btnColumnSelect.Click += btnColumnSelect_Click;
            // 
            // chk_original
            // 
            chk_original.AutoSize = true;
            chk_original.Location = new Point(992, 402);
            chk_original.Name = "chk_original";
            chk_original.Size = new Size(99, 19);
            chk_original.TabIndex = 15;
            chk_original.Text = "Original verw.";
            chk_original.UseVisualStyleBackColor = true;
            // 
            // Nas_Suche
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1188, 450);
            Controls.Add(chk_original);
            Controls.Add(btnColumnSelect);
            Controls.Add(btn_help);
            Controls.Add(lbl_max_recs);
            Controls.Add(txt_max_recs);
            Controls.Add(txt_records);
            Controls.Add(lbl_records);
            Controls.Add(lbl_max);
            Controls.Add(btn_preview_db);
            Controls.Add(pic_anz);
            Controls.Add(btn_close);
            Controls.Add(btn_clear);
            Controls.Add(btn_sort);
            Controls.Add(dataGridView1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Nas_Suche";
            Text = "NAS Suche";
            Load += Nas_Suche_Load;
            Resize += Nas_Suche_Resize;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private Button btn_sort;
        private Button btn_clear;
        private Button btn_close;
        private Button btn_previews;
        private TextBox pic_height;
        private Button btn_preview_db;
        private TextBox pic_anz;
        private Label lbl_max;
        private Label lbl_records;
        private TextBox txt_records;
        private TextBox txt_max_recs;
        private Label lbl_max_recs;
        private Button btn_help;
        private Button btnColumnSelect;
        private CheckBox chk_original;
    }
}