using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Nas_Suchen
{
    public class FaceWindow : Form
    {
        private readonly Image _image;
        private readonly long _mediaId;
        private readonly MySqlConnection _db;

        private PictureBox pictureBox1;
        private DataGridView dataGridView1;
        private SplitContainer splitContainer1;

        public FaceWindow(Image img, long mediaId, MySqlConnection dbconnection)
        {
            _image = img;
            _mediaId = mediaId;
            _db = dbconnection;

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

            // ZWINGT den SplitContainer zu einem sauberen Layout-Refresh
            splitContainer1.ResumeLayout();
            splitContainer1.PerformLayout();
            splitContainer1.Invalidate();
            splitContainer1.Update();
            this.PerformLayout();

            LoadFaces();
        }

        private void LoadFaces()
        {
            string sql =
                @"SELECT number, thumbnail
                  FROM Media.face_vectors
                  WHERE id = @id
                  ORDER BY number ASC";

            using var cmd = new MySqlCommand(sql, _db);
            cmd.Parameters.AddWithValue("@id", _mediaId);

            var dt = new DataTable();
            dt.Columns.Add("number", typeof(int));
            dt.Columns.Add("thumbnail", typeof(Image));

            //_db.Open();
            using var rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                int num = rdr.GetInt32("number");

                Image faceImg = null;

                if (!rdr.IsDBNull(1))
                {
                    var bytes = (byte[])rdr["thumbnail"];
                    using var ms = new MemoryStream(bytes);
                    faceImg = Image.FromStream(ms);
                }

                dt.Rows.Add(num, faceImg);
            }

            rdr.Close();

            dataGridView1.DataSource = dt;

            ConfigureGrid();
        }

        private void ConfigureGrid()
        {
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            dataGridView1.Columns["number"].Width = 60;

            var colImg = (DataGridViewImageColumn)dataGridView1.Columns["thumbnail"];
            colImg.ImageLayout = DataGridViewImageCellLayout.Zoom;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var img = row.Cells["thumbnail"].Value as Image;
                if (img != null)
                    row.Height = img.Height;
                else
                    row.Height = 60;
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

            // splitContainer1
            this.splitContainer1.Dock = DockStyle.Fill;
            this.splitContainer1.Orientation = Orientation.Vertical;
            this.splitContainer1.SplitterDistance = 400;

            // pictureBox1 (left)
            this.pictureBox1.Dock = DockStyle.Fill;
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox1.BackColor = Color.Black;

            // dataGridView1 (right)
            this.dataGridView1.Dock = DockStyle.Fill;
            this.dataGridView1.AllowUserToResizeRows = false;

            // Panels
            this.splitContainer1.Panel1.Controls.Add(this.pictureBox1);
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);

            // Form
            this.Controls.Add(this.splitContainer1);
            this.Text = "Faces";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Normal;
            this.Load += new EventHandler(FaceWindow_Load);
            this.Width = 900;
            this.Height = 700;

            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);

            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();

            this.ResumeLayout(false);
        }
    }
}
