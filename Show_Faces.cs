using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
//using MySql.Data.MySqlClient;
using Npgsql;

namespace Nas_Suchen
{
    public class FaceWindow : Form
    {
        private readonly Image _image;
        private readonly long _mediaId;
        private readonly NpgsqlConnection _db;

        private readonly int _origWidth;
        private readonly int _origHeight;

        private PictureBox pictureBox1;
        private DataGridView dataGridView1;
        private SplitContainer splitContainer1;

        private DataTable _personenTable;
        private bool _hasChanges = false;

        private Rectangle? _currentFaceRect = null;

        public FaceWindow(Image img, long mediaId, NpgsqlConnection dbconnection, int origWidth, int origHeight)
        {
            _image = img;
            _mediaId = mediaId;
            _db = dbconnection;

            _origWidth = origWidth;
            _origHeight = origHeight;

            InitializeComponent();

            int w = img.Width;
            int max = this.Width / 2;
            if (w > max) w = max;

            splitContainer1.SplitterDistance = w;
        }

        private void FaceWindow_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = _image;

            int fixedLeftWidth = 250;

            splitContainer1.Panel1MinSize = fixedLeftWidth;
            splitContainer1.SplitterDistance = fixedLeftWidth;

            splitContainer1.ResumeLayout();
            splitContainer1.PerformLayout();
            splitContainer1.Invalidate();
            splitContainer1.Update();
            this.PerformLayout();

            LoadFaces();
        }

        private void LoadPersonen()
        {
            string sql = @"SELECT id,
                      CONCAT(nachname, ', ', vorname) AS name
               FROM public.personen
               ORDER BY nachname ASC, vorname ASC";

            using var cmd = new NpgsqlCommand(sql, _db);
            using var rdr = cmd.ExecuteReader();

            _personenTable = new DataTable();
            _personenTable.Columns.Add("id", typeof(long));
            _personenTable.Columns.Add("name", typeof(string));

            _personenTable.Rows.Add(DBNull.Value, "");

            while (rdr.Read())
            {
                long id = rdr.GetInt64("id");
                string name = rdr.GetString("name");
                _personenTable.Rows.Add(id, name);
            }
        }

        private void LoadFaces()
        {
            string sql =
                @"SELECT number, thumbnail, top, ""right"", bottom, ""left"", personen_id
                  FROM public.face_vectors
                  WHERE id = @id
                  ORDER BY number ASC";

            using var cmd = new NpgsqlCommand(sql, _db);
            cmd.Parameters.AddWithValue("@id", _mediaId);

            var dt = new DataTable();
            dt.Columns.Add("number", typeof(int));
            dt.Columns.Add("thumbnail", typeof(Image));
            dt.Columns.Add("top", typeof(int));
            dt.Columns.Add("right", typeof(int));
            dt.Columns.Add("bottom", typeof(int));
            dt.Columns.Add("left", typeof(int));
            dt.Columns.Add("personen_id", typeof(object));

            using var rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                int num = rdr.GetInt32("number");
                int top = rdr.GetInt32("top");
                int right = rdr.GetInt32("right");
                int bottom = rdr.GetInt32("bottom");
                int left = rdr.GetInt32("left");

                Image faceImg = null;

                if (!rdr.IsDBNull(1))
                {
                    var bytes = (byte[])rdr["thumbnail"];
                    using var ms = new MemoryStream(bytes);
                    faceImg = Image.FromStream(ms);
                }

                object pid = rdr.IsDBNull(rdr.GetOrdinal("personen_id")) ? DBNull.Value : rdr.GetInt64("personen_id");

                dt.Rows.Add(num, faceImg, top, right, bottom, left, pid);
            }

            rdr.Close();

            dataGridView1.DataSource = dt;

            LoadPersonen();
            ConfigureGrid();

            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[0].Selected = true;

                DataGridView1_CellClick(
                    dataGridView1,
                    new DataGridViewCellEventArgs(0, 0)
                );
            }
        }

        private void ConfigureGrid()
        {
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            dataGridView1.Columns["number"].Width = 60;

            var colImg = (DataGridViewImageColumn)dataGridView1.Columns["thumbnail"];
            colImg.ImageLayout = DataGridViewImageCellLayout.Zoom;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var img = row.Cells["thumbnail"].Value as Image;
                row.Height = img != null ? img.Height : 60;
            }

            dataGridView1.Columns["top"].Visible = false;
            dataGridView1.Columns["right"].Visible = false;
            dataGridView1.Columns["bottom"].Visible = false;
            dataGridView1.Columns["left"].Visible = false;

            var combo = new DataGridViewComboBoxColumn();
            combo.HeaderText = "Person";
            combo.Name = "person";
            combo.DataSource = _personenTable;
            combo.DisplayMember = "name";
            combo.ValueMember = "id";
            combo.DataPropertyName = "personen_id";
            combo.Width = 180;

            dataGridView1.Columns.Add(combo);

            dataGridView1.Columns["personen_id"].Visible = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                object pid = row.Cells["personen_id"].Value;
                row.Cells["person"].Value =
                    (pid == null || pid == DBNull.Value) ? DBNull.Value : pid;
            }
        }

        private void InitializeComponent()
        {
            this.splitContainer1 = new SplitContainer();
            this.pictureBox1 = new PictureBox();
            this.dataGridView1 = new DataGridView();

            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();

            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();

            this.SuspendLayout();

            this.splitContainer1.Dock = DockStyle.Fill;
            this.splitContainer1.Orientation = Orientation.Vertical;
            this.splitContainer1.SplitterDistance = 400;

            this.pictureBox1.Dock = DockStyle.Fill;
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox1.BackColor = Color.Black;
            this.pictureBox1.Paint += PictureBox1_Paint;

            this.dataGridView1.Dock = DockStyle.Fill;
            this.dataGridView1.AllowUserToResizeRows = false;

            this.splitContainer1.Panel1.Controls.Add(this.pictureBox1);
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);

            this.Controls.Add(this.splitContainer1);
            this.Text = "Faces";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Width = 900;
            this.Height = 700;
            this.Load += new EventHandler(FaceWindow_Load);

            this.dataGridView1.CellClick += DataGridView1_CellClick;
            this.dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
            this.dataGridView1.CurrentCellDirtyStateChanged += DataGridView1_CurrentCellDirtyStateChanged;
            this.FormClosing += FaceWindow_FormClosing;

            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();

            this.ResumeLayout(false);
        }

        private void DataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dataGridView1.Columns[e.ColumnIndex].Name != "person") return;

            var row = dataGridView1.Rows[e.RowIndex];
            row.DefaultCellStyle.BackColor = Color.LightYellow;
            _hasChanges = true;
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dataGridView1.Rows[e.RowIndex];

            int top = (int)row.Cells["top"].Value;
            int right = (int)row.Cells["right"].Value;
            int bottom = (int)row.Cells["bottom"].Value;
            int left = (int)row.Cells["left"].Value;

            float scaleX = (float)_image.Width / _origWidth;
            float scaleY = (float)_image.Height / _origHeight;

            int x = (int)(left * scaleX);
            int y = (int)(top * scaleY);
            int w = (int)((right - left) * scaleX);
            int h = (int)((bottom - top) * scaleY);

            Rectangle faceRectImage = new Rectangle(x, y, w, h);

            pictureBox1.Image = _image;

            _currentFaceRect = TranslateRectToPictureBox(faceRectImage);
            pictureBox1.Invalidate();
        }

        private Rectangle TranslateRectToPictureBox(Rectangle imgRect)
        {
            if (_image == null || pictureBox1.ClientSize.Width == 0 || pictureBox1.ClientSize.Height == 0)
                return imgRect;

            float imageRatio = (float)_image.Width / _image.Height;
            float boxRatio = (float)pictureBox1.ClientSize.Width / pictureBox1.ClientSize.Height;

            float scale;
            int drawWidth, drawHeight;
            int offsetX, offsetY;

            if (boxRatio > imageRatio)
            {
                scale = (float)pictureBox1.ClientSize.Height / _image.Height;
                drawHeight = pictureBox1.ClientSize.Height;
                drawWidth = (int)(_image.Width * scale);
                offsetX = (pictureBox1.ClientSize.Width - drawWidth) / 2;
                offsetY = 0;
            }
            else
            {
                scale = (float)pictureBox1.ClientSize.Width / _image.Width;
                drawWidth = pictureBox1.ClientSize.Width;
                drawHeight = (int)(_image.Height * scale);
                offsetX = 0;
                offsetY = (pictureBox1.ClientSize.Height - drawHeight) / 2;
            }

            return new Rectangle(
                offsetX + (int)(imgRect.X * scale),
                offsetY + (int)(imgRect.Y * scale),
                (int)(imgRect.Width * scale),
                (int)(imgRect.Height * scale)
            );
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (_currentFaceRect.HasValue)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, _currentFaceRect.Value);
                }
            }
        }

        private void SaveChanges()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                long number = Convert.ToInt64(row.Cells["number"].Value);

                object val = row.Cells["person"].Value;

                object pid = (val == null || val == DBNull.Value)
                    ? (object)DBNull.Value
                    : Convert.ToInt64(val);

                string sql =
                    @"UPDATE public.face_vectors
              SET personen_id = @pid, zuordnung = 3
              WHERE id = @id AND number = @num";

                using var cmd = new NpgsqlCommand(sql, _db);
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@id", _mediaId);
                cmd.Parameters.AddWithValue("@num", number);

                cmd.ExecuteNonQuery();
            }

            _hasChanges = false;
        }

        private void FaceWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_hasChanges)
                return;

            var dlg = new SaveChangesDialog();
            dlg.ShowDialog(this);

            switch (dlg.DialogResultCustom)
            {
                case SaveChangesDialog.Result.Cancel:
                    e.Cancel = true;
                    break;

                case SaveChangesDialog.Result.Save:
                    SaveChanges();
                    break;
            }
        }
    }
}
