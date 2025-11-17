using System.Data;
using System.Diagnostics;
//using System.Globalization;
using System.Net;
using MySql.Data.MySqlClient;
//using Org.BouncyCastle.Tls;
using Sort_Dialog;
//using static System.ComponentModel.Design.ObjectSelectorEditor;
//using static Sort_Dialog.Globals;

namespace Nas_Suchen
{
    public partial class Nas_Suche : Form
    {
        int renumbering = 0;
        int show_all = 0;             // wenn auf 1 gesetzt werden auch Multimedia2 und Multimedia3 mitselektiert.
        string sql_where_public = " mf.public = 1";   //  nur jugendfreie Medien werden gezeigt  --  public = 1 
        string sql_filter = "";

        int Media_name = 0;
        int Media_pfad = 1;
        int Media_interpret = 2;
        int Media_year = 3;
        int Media_album = 4;
        int Media_genre = 5;
        int Media_ext = 6;
        int Media_fileext = 7;
        int Media_added = 8;
        int Media_aufnahme_datum = 9;
        int Media_image = 10;
        int Media_source = 11;
        int Media_latitude = 12;
        int Media_longitude = 13;
        int Media_size_bytes = 14;
        int Media_thumbnail_size = 15;

        int expert_mode = 0;

        List<string> kfiletypen = new List<string>();
        List<string> kfiletypen_t = new List<string>();

        // Basis-Definition der Spalten und Typen für den Sort-Dialog
        // ----------------------------------------------------------
        string[] Spalten_namen = { "Dateiname", "Pfad", "Interpret", "Jahr", "Album", "Genre", "DateiTyp", "FileExt", "Hinzugefügt", "Erstellt",
                                   "Ordnertyp", "Breitengrad", "Längengrad", "Größe", "Thumbnailgröße" };
        string[] Spalten_namen_col_select = { "Dateiname", "Pfad", "Interpret", "Jahr", "Album", "Genre", "DateiTyp", "FileExt", "Hinzugefügt", "Erstellt", "Preview",
                                   "Ordnertyp", "Breitengrad", "Längengrad", "Größe", "Thumbnailgröße" };
        // Spaltenüberschriften für Sort und Suchen-Dialog   

        string[] felder_z = { "mf.filename", "mf.pathname", "mf.interpret", "mf.year", "mf.album", "mf.genre", "mf.extension", "mf.ext_char", "mf.added",
                              "mf.aufnahme_datum", "mf.source", "mf.latitude", "mf.longitude", "mf.size_bytes", "length(mf.thumbnail)" };
        // Spaltennamen in der Datenbank   

        // Typen in den jeweiligen Spalten in der DB
        int[] DBtypen = { (int)Globals.DBtypen.db_string,
                          (int)Globals.DBtypen.db_string,
                          (int)Globals.DBtypen.db_string,
                          (int)Globals.DBtypen.db_int32,
                          (int)Globals.DBtypen.db_string,
                          (int)Globals.DBtypen.db_string,
                          (int)Globals.DBtypen.db_string,
                          (int)Globals.DBtypen.db_string,
                          (int)Globals.DBtypen.db_datetime,
                          (int)Globals.DBtypen.db_datetime,
                          (int)Globals.DBtypen.db_int32,
                          (int)Globals.DBtypen.db_decimal,
                          (int)Globals.DBtypen.db_decimal,
                          (int)Globals.DBtypen.db_int64,
                          (int)Globals.DBtypen.db_int32};

        // Verwendeter Usercode aus der Tabelle sm.usercodes 0 kein Usercode
        int[] Usercodes = { 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 7, 0, 0, 0, 0 };

        public List<int> spalten;
        public List<int> sortierung;
        public List<int> conditions;
        public List<int> filtering;
        public List<string> filterkeys;

        public List<int> klammer1;
        public List<int> verknuepfung;
        public List<int> klammer2;

        MySql.Data.MySqlClient.MySqlConnection dbconnection = new MySql.Data.MySqlClient.MySqlConnection();

        string def_select = "SELECT mf.filename, mf.pathname, mf.interpret, cast(mf.year AS char), mf.album, mf.genre, et.description, DATE_FORMAT(mf.added, '%d-%m-%Y')," +
                "  mf.ext_char, DATE_FORMAT(mf.aufnahme_datum, '%d-%m-%Y %H:%i'), mf.source " +
                " ,COUNT(*) OVER(), mf.latitude, mf.longitude, mf.size_bytes, length(mf.thumbnail) FROM Media.media_files mf LEFT JOIN Media.media_types et ON(et.m_key = mf.extension) where mf.pathname = '\\\\sm-nas3\\Multimedia' ";

        string connect = "";
        public Nas_Suche()
        {
            InitializeComponent();
        }

        private void Nas_Suche_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            Form credent = new db_Login();
            credent.StartPosition = FormStartPosition.Manual;
            credent.Load += (s, e) =>
            {
                var screen = Screen.FromControl(this);
                credent.Location = new Point(
                    screen.Bounds.Left + (screen.Bounds.Width - credent.Width) / 2,
                    screen.Bounds.Top + (screen.Bounds.Height - credent.Height) / 2
                );
            };
            credent.ShowDialog();

            if (Globals1.D_cancel == 1) { this.Close(); }

            //MessageBox.Show(Environment.OSVersion.Version.Major.ToString());

            if (Environment.OSVersion.Version.Build < 22000)
            {
                // Windows 10
                connect = $"server = {Globals1.D_server}; port = {Globals1.D_port}; Database = {Globals1.D_db}; uid = {Globals1.D_user}; pwd = {Globals1.D_pw}; Charset = utf8;" +
                                 "SslMode = VerifyCA; SslCa = C:\\Certificate\\ca-cert.pem";
            }
            else
            {
                if (Globals1.D_pw != "")
                {
                    // Windows 11
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
                    connect = $"server={Globals1.D_server}; port={Globals1.D_port}; Database={Globals1.D_db}; uid={Globals1.D_user}; pwd={Globals1.D_pw}; Charset = utf8;TlsVersion=TlSv1.3;" +
                              "SslMode = VerifyFull; SslCa = C:\\Certificate\\ca-cert.pem;SslCert=C:\\Certificate\\client-cert.pem;SslKey=C:\\Certificate\\client-key.pem;";
                    //     "SslMode = VerifyCA; SslMode=Required"
                }
                else
                {
                    // Windows 11
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
                    connect = $"server={Globals1.D_server}; port={Globals1.D_port}; Database={Globals1.D_db}; uid={Globals1.D_user}; pwd=''; Charset = utf8;TlsVersion=Tlsv1.3;" +
                              "SslMode=VerifyCA;SslCa=C:\\Certificate\\ca-cert.pem;SslCert=C:\\Certificate\\client-cert.pem;SslKey=C:\\Certificate\\client-key.pem;";
                    //     "SslMode = VerifyCA; SslMode=Required"
                }
            }

            dbconnection.ConnectionString = connect;
            dbconnection.Open();

            //using (var cmd = new MySqlCommand("SET GLOBAL max_allowed_packet = 1024*1024*1024;", dbconnection))          
            //using (var cmd = new MySqlCommand("SET SESSION net_read_timeout=60000, net_write_timeout=60000;", dbconnection))

            this.Text = this.Text + $"  / DB-Server: {Globals1.D_server}:{Globals1.D_port} " + dbconnection.ServerVersion + " Database: " + dbconnection.Database;

            using (var cmd = new MySqlCommand("SHOW SESSION STATUS LIKE 'Ssl_cipher'", dbconnection))
            {
                MySqlDataReader reader = cmd.ExecuteReader();
                {
                    while (reader.Read())
                    {
                        this.Text = this.Text + " / " + reader.GetString(0);
                        this.Text = this.Text + ": " + reader.GetString(1);
                    }
                    reader.Close();
                }
            }

            using (var cmd = new MySqlCommand("SHOW SESSION STATUS LIKE 'Ssl_version';", dbconnection))
            {
                MySqlDataReader reader = cmd.ExecuteReader();
                {
                    while (reader.Read())
                    {
                        this.Text = this.Text + " " + reader.GetString(0);
                        this.Text = this.Text + ": " + reader.GetString(1);
                    }
                    reader.Close();
                }
            }

            using (var cmd = new MySqlCommand("select count(*) from media_files", dbconnection))
            {
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                this.Text = this.Text + "  Anzahl Datensätze: " + reader.GetInt32(0);
                reader.Close();
            }

            InitializeLists();

            BuildMediaDataGrid();
            FillDataGridMedia(sql_filter);
            dataGridView1.DefaultCellStyle.Font = new Font("Courier", 10);

            Renumber_Grid(dataGridView1);
            rearrange();
            Cursor.Current = Cursors.Default;
        }

        private void InitializeLists()
        {
            spalten = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
            sortierung = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            conditions = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 };
            filtering = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            filterkeys = new List<string> { "", "", "", "", "", "", "", "", "", "", "0", "", "", "", "" };

            klammer1 = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            verknuepfung = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            klammer2 = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        }

        void Renumber_Grid(DataGridView g)
        {
            renumbering = 1;
            foreach (DataGridViewRow dGVRow in g.Rows)
            {
                dGVRow.HeaderCell.Value = String.Format("{0}", dGVRow.Index + 1);
            }
            g.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
            renumbering = 0;
        }

        private void BuildMediaDataGrid()
        {
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.False;

            for (int i = 0; i < Spalten_namen.Length; i++)
            {
                if (DBtypen[i] == (int)Globals.DBtypen.db_string)
                {    // Text 
                    dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = Spalten_namen[i],
                        ValueType = typeof(string),
                        ReadOnly = true
                    });
                }
                else if (DBtypen[i] == (int)Globals.DBtypen.db_int32)
                {   // Integer
                    if (Usercodes[i] <= 0)
                    {
                        var col = new DataGridViewTextBoxColumn
                        {
                            Name = Spalten_namen[i],
                            ValueType = typeof(int),
                            ReadOnly = true,
                            DefaultCellStyle =
                            {
                                Alignment = DataGridViewContentAlignment.MiddleRight,
                                Format = "N0",
                                FormatProvider = new System.Globalization.CultureInfo("de-DE")
                            }
                        };
                        col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dataGridView1.Columns.Add(col);
                    }
                    else
                    {   //Combobox
                        var comboCol = new DataGridViewComboBoxColumn
                        {
                            Name = Spalten_namen[i],
                            ReadOnly = true
                        };

                        string usercode_read = "SELECT codetyp_c, code_beschreib FROM sm.usercodes WHERE codetyp_i = @codetyp AND sprache = 'german'";
                        using var cmd1 = new MySqlCommand(usercode_read, dbconnection);
                        cmd1.Parameters.AddWithValue("@codetyp", Usercodes[i].ToString());

                        using var rdr = cmd1.ExecuteReader();
                        while (rdr.Read())
                        {
                            comboCol.Items.Add(rdr.GetString(1));
                            kfiletypen.Add(rdr.GetString(0));
                            kfiletypen_t.Add(rdr.GetString(1));
                        }

                        rdr.Close();
                        dataGridView1.Columns.Add(comboCol);
                    }
                }
                else if (DBtypen[i] == (int)Globals.DBtypen.db_int64)
                {   // Long
                    var col = new DataGridViewTextBoxColumn
                    {
                        Name = Spalten_namen[i],
                        ValueType = typeof(long),
                        ReadOnly = true,
                        DefaultCellStyle =
                        {
                            Alignment = DataGridViewContentAlignment.MiddleRight,
                            Format = "N0",
                            FormatProvider = new System.Globalization.CultureInfo("de-DE")
                        }
                    };
                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dataGridView1.Columns.Add(col);
                }

                else if (DBtypen[i] == (int)Globals.DBtypen.db_decimal)
                {   // Decimal
                    dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = Spalten_namen[i],
                        ValueType = typeof(decimal),
                        ReadOnly = true,
                        DefaultCellStyle =
                        {
                            Alignment = DataGridViewContentAlignment.MiddleRight,
                            Format = "N2",
                            FormatProvider = new System.Globalization.CultureInfo("de-DE")
                        }
                    });
                }
                else if (DBtypen[i] == (int)Globals.DBtypen.db_datetime)
                {   // Date&Time
                    dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        Name = Spalten_namen[i],
                        ValueType = typeof(DateTime),
                        ReadOnly = true,
                        DefaultCellStyle = { Format = "dd.MM.yyyy" }
                    });
                }
                if (i == Media_image - 1)
                {   // Image Column
                    dataGridView1.Columns.Add(new DataGridViewImageColumn
                    {
                        Name = "Preview",
                        ReadOnly = true,
                        ImageLayout = DataGridViewImageCellLayout.Zoom
                    });
                }
            }
        }

        private void loadFileFromNas(string filename, string pathname)
        {
            pathname = pathname.Replace("\\", "/");
            pathname = pathname.Replace("//sm-nas3", "/share");
            pathname = pathname + "/" + filename;
            //MessageBox.Show(pathname);

            string query = "SELECT LOAD_FILE(@filePath)";

            using (var cmd = new MySqlCommand(query, dbconnection))
            {
                cmd.Parameters.AddWithValue("@filePath", pathname);
                cmd.CommandTimeout = 300;
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    byte[] fileBytes = (byte[])result;
                    string downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", filename);
                    //MessageBox.Show(downloadPath);
                    File.WriteAllBytes(downloadPath, fileBytes);
                    MessageBox.Show(downloadPath + " " + (fileBytes.Length) / (1024 * 1024) + " MB downloaded successfully.");
                }
                else
                {
                    MessageBox.Show(filename + " Download hat nicht geklappt!");
                }
            }
        }

        private void FillDataGridMedia(string sql_select)
        {
            var cmd1 = new MySqlCommand("", dbconnection);
            if (sql_select != "")
            {
                //sql_select = sql_select.Replace(@"\", @"\\");
                cmd1 = new MySqlCommand(sql_select, dbconnection);
            }
            else
            {
                string numbers = new string(txt_records.Text.Where(char.IsDigit).ToArray());
                cmd1 = new MySqlCommand("SELECT mf.filename, mf.pathname, mf.interpret, cast(mf.year AS char), mf.album, mf.genre, et.description, " +
                    "DATE_FORMAT(mf.added, '%d-%m-%Y'), mf.ext_char, DATE_FORMAT(mf.aufnahme_datum, '%d-%m-%Y %H:%i'), mf.source,  COUNT(*) OVER(), mf.latitude, " +
                    "mf.longitude, mf.size_bytes, length(mf.thumbnail) " +
                    "FROM Media.media_files mf LEFT JOIN Media.media_types et ON(et.m_key = mf.extension) " +
                    "where mf.pathname = '\\\\sm-nas3\\Multimedia' LIMIT " + numbers + " ", dbconnection);
            }
            MySqlDataReader rdr = cmd1.ExecuteReader();

            dataGridView1.Rows.Clear();
            int anz = 0;
            while (rdr.Read())
            {
                anz++;
                dataGridView1.Rows.Add();
                FillDataGridHandleNull(dataGridView1, rdr, 0, Media_name, (int)Globals.DBtypen.db_string);
                FillDataGridHandleNull(dataGridView1, rdr, 1, Media_pfad, (int)Globals.DBtypen.db_string);
                FillDataGridHandleNull(dataGridView1, rdr, 2, Media_interpret, (int)Globals.DBtypen.db_string);
                FillDataGridHandleNull(dataGridView1, rdr, 3, Media_year, (int)Globals.DBtypen.db_int32);
                FillDataGridHandleNull(dataGridView1, rdr, 4, Media_album, (int)Globals.DBtypen.db_string);
                FillDataGridHandleNull(dataGridView1, rdr, 5, Media_genre, (int)Globals.DBtypen.db_string);
                FillDataGridHandleNull(dataGridView1, rdr, 6, Media_ext, (int)Globals.DBtypen.db_string);
                FillDataGridHandleNull(dataGridView1, rdr, 7, Media_added, (int)Globals.DBtypen.db_string);
                FillDataGridHandleNull(dataGridView1, rdr, 8, Media_fileext, (int)Globals.DBtypen.db_string);
                FillDataGridHandleNull(dataGridView1, rdr, 9, Media_aufnahme_datum, (int)Globals.DBtypen.db_string);

                DataGridViewComboBoxCell comboBoxCell = dataGridView1[Media_source, dataGridView1.RowCount - 1] as DataGridViewComboBoxCell;
                if (kfiletypen.IndexOf(rdr.GetInt32(10).ToString()) >= 0)
                {
                    //MessageBox.Show("X" + ktypen_t[ktypen.IndexOf(rdr.GetString(Haupbt_s_typ))] + "X");
                    comboBoxCell.Value = kfiletypen_t[kfiletypen.IndexOf(rdr.GetInt32(10).ToString())];
                }

                FillDataGridHandleNull(dataGridView1, rdr, 12, Media_latitude, (int)Globals.DBtypen.db_decimal);
                FillDataGridHandleNull(dataGridView1, rdr, 13, Media_longitude, (int)Globals.DBtypen.db_decimal);
                FillDataGridHandleNull(dataGridView1, rdr, 14, Media_size_bytes, (int)Globals.DBtypen.db_int64);
                FillDataGridHandleNull(dataGridView1, rdr, 15, Media_thumbnail_size, (int)Globals.DBtypen.db_int32);
            }
            if (anz > 0)
            {
                txt_max_recs.Text = rdr.GetInt32(Media_source).ToString();
            }
            else
            {
                txt_max_recs.Text = "0";
            }
            rdr.Close();
        }

        void FillDataGridHandleNull(DataGridView g, MySqlDataReader r, int selpos, int gridpos, int d_type)
        {
            if (d_type == (int)Globals.DBtypen.db_string) { if (r.IsDBNull(selpos) == false) { g[gridpos, g.RowCount - 1].Value = r.GetString(selpos); } }
            if (d_type == (int)Globals.DBtypen.db_decimal) { if (r.IsDBNull(selpos) == false) { g[gridpos, g.RowCount - 1].Value = r.GetDecimal(selpos); } }
            if (d_type == (int)Globals.DBtypen.db_datetime) { if (r.IsDBNull(selpos) == false) { g[gridpos, g.RowCount - 1].Value = r.GetDateTime(selpos); } }
            if (d_type == (int)Globals.DBtypen.db_int32) { if (r.IsDBNull(selpos) == false) { g[gridpos, g.RowCount - 1].Value = r.GetInt32(selpos); } }
            if (d_type == (int)Globals.DBtypen.db_int64) { if (r.IsDBNull(selpos) == false) { g[gridpos, g.RowCount - 1].Value = r.GetInt64(selpos); } }
        }
        private void Nas_Suche_Resize(object sender, EventArgs e)
        {
            rearrange();
        }
        private void rearrange()
        {
            int i = 5; //Offset oben
            int h = 22; // height
            int j = h + 3; //Offset zwischen den Buttons
            int w = 150; // width
            int d = 6;
            int dist = 3; // Vertical distance of controls

            dataGridView1.Height = this.Height - 95;

            btn_sort.Top = dataGridView1.Height + i;
            btn_sort.Left = dataGridView1.Left + d;
            btn_sort.Width = w;
            btn_sort.Height = h;

            btn_clear.Top = dataGridView1.Height + i;
            btn_clear.Width = w;
            btn_clear.Height = h;

            btn_clear.Left = btn_sort.Left + btn_sort.Width + d;

            btn_close.Height = h;
            btn_close.Top = dataGridView1.Height + i;
            btn_close.Left = this.Width - btn_close.Width - (d * 3);

            btn_help.Height = h;
            btn_help.Width = btn_close.Width;
            btn_help.Top = btn_close.Top + btn_close.Height + dist;
            btn_help.Left = btn_close.Left;

            btn_preview_db.Height = h;
            btn_preview_db.Width = w;
            btn_preview_db.Top = dataGridView1.Height + i;
            btn_preview_db.Left = btn_clear.Left + btn_clear.Width + d;

            btnColumnSelect.Height = h;
            btnColumnSelect.Width = w;  
            btnColumnSelect.Top = dataGridView1.Height + i;
            btnColumnSelect.Left = btn_preview_db.Left + btn_preview_db.Width + d;  

            lbl_max.Height = h;
            lbl_max.Width = w / 2 - 3;
            lbl_max.Top = btn_preview_db.Top + btn_preview_db.Height + dist;
            lbl_max.Left = btn_preview_db.Left;

            pic_anz.Height = h;
            pic_anz.Width = w / 2 - 3;
            pic_anz.Top = btn_preview_db.Top + btn_preview_db.Height + 3;
            pic_anz.Left = btn_preview_db.Left + lbl_max.Width + dist + 3;

            lbl_records.Height = h;
            lbl_records.Width = w / 2 - 3;
            lbl_records.Top = btn_sort.Top + btn_sort.Height + dist;
            lbl_records.Left = btn_sort.Left;

            txt_records.Height = h;
            txt_records.Width = w / 2 - 3;
            txt_records.Top = lbl_records.Top;
            txt_records.Left = lbl_records.Left + lbl_records.Width + dist + 3;

            lbl_max_recs.Height = h;
            lbl_max_recs.Width = w / 2 - 3;
            lbl_max_recs.Top = lbl_records.Top;
            lbl_max_recs.Left = btn_clear.Left;

            txt_max_recs.Height = h;
            txt_max_recs.Width = w / 2 - 3;
            txt_max_recs.Top = lbl_records.Top;
            txt_max_recs.Left = lbl_max_recs.Left + lbl_max_recs.Width + dist + 3;
        }

        private void btn_sort_Click(object sender, EventArgs e)
        {
            Sort_Dialog.Sort_Dialog Sort_Grid = new Sort_Dialog.Sort_Dialog()
            {
                Spalten_z = Spalten_namen,
                Felder_z = felder_z,
                Db_typen = DBtypen,
                Usercode = Usercodes,

                Spalten = spalten,
                Sortierung = sortierung,
                Conditions = conditions,
                Filterkeys = filterkeys,
                Filtering = filtering,

                Klammer1 = klammer1,
                Klammer2 = klammer2,
                Verknuepfung = verknuepfung,

                Tabelle = "Media.media_files",
                Tabelle_media_types = "Media.media_types",
                Tabelle_ucodes = "sm.usercodes",
                Sqlsort = "SELECT mf.filename, mf.pathname, mf.interpret, cast(mf.year AS char), mf.album, mf.genre, et.description, DATE_FORMAT(mf.added, '%d-%m-%Y')," +
                          "  mf.ext_char, DATE_FORMAT(mf.aufnahme_datum, '%d-%m-%Y %H:%i'), mf.source " +
                          " ,COUNT(*) OVER(), mf.latitude, mf.longitude, mf.size_bytes, length(mf.thumbnail) FROM Media.media_files mf " +
                          " LEFT JOIN Media.media_types et ON(et.m_key = mf.extension) ",
                StartPosition = FormStartPosition.Manual,

                Sort_cancel = "Cancel",
                Sort_caption = "Filtern / Sortieren Tabelle: ",
                Sort_cond = "Bedingung",
                Sort_filter = "Filter",
                Sort_ok = "OK",
                Sort_sort = "Sortierung",
                Sort_spalte = "Spalte",
                Sort_klammer1 = "Klammer auf",
                Sort_klammer2 = "Klammer zu",
                Sort_verknuepfung = "Verknüpfung",
                Sort_expert = "Expertenmodus",
                Sort_expertmode = expert_mode,
                Connection = connect
            };

            Form af = this;
            var screen = Screen.FromControl(af);
            //Console.WriteLine("Active Screen: " + screen.DeviceName);
            Sort_Grid.Location = new System.Drawing.Point(
                screen.Bounds.Left + (screen.Bounds.Width - Sort_Grid.Width) / 2,
                screen.Bounds.Top + (screen.Bounds.Height - Sort_Grid.Height) / 2
            );
            Sort_Grid.Sql_where_public = sql_where_public;

            Sort_Grid.ShowDialog(this);
            Cursor.Current = Cursors.WaitCursor;
            Console.WriteLine(Sort_Grid.Sqlstring);
            string numbers = new string(txt_records.Text.Where(char.IsDigit).ToArray());
            sql_filter = Sort_Grid.Sqlstring + " LIMIT " + numbers + " ";

            if (Sort_Grid.Sqlstring != "")
            {
                FillDataGridMedia(sql_filter);
                Renumber_Grid(dataGridView1);
            }
            expert_mode = Sort_Grid.Sort_expertmode;
            Clipboard.SetText(sql_filter.Replace("'", "\""));
            Sort_Grid.Dispose();
            Cursor.Current = Cursors.Default;
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            sql_filter = "";
            renumbering = 1;
            FillDataGridMedia(sql_filter);
            Renumber_Grid(dataGridView1);
            renumbering = 0;

            InitializeLists();

            Cursor.Current = Cursors.Default;
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private Size GetThumbnailSize(Image original, int maxWidth, int maxHeight)
        {
            // Calculate the scaling factor to maintain the aspect ratio
            double widthFactor = (double)maxWidth / original.Width;
            double heightFactor = (double)maxHeight / original.Height;
            double scaleFactor = Math.Min(widthFactor, heightFactor);

            // Calculate the new width and height based on the scaling factor
            int width = (int)(original.Width * scaleFactor);
            int height = (int)(original.Height * scaleFactor);

            return new Size(width, height);
        }
        void GetThumbnailFromDatabase(int zeile)
        {
            var cmd1 = new MySqlCommand("select thumbnail from media_files where filename = @filename and pathname = @pathname", dbconnection);
            cmd1.Parameters.AddWithValue("@filename", dataGridView1[Media_name, zeile].Value.ToString());
            cmd1.Parameters.AddWithValue("@pathname", dataGridView1[Media_pfad, zeile].Value.ToString());
            MySqlDataReader rdr = cmd1.ExecuteReader();

            if (rdr.Read())
            {
                if (rdr.IsDBNull(0) == false)
                {
                    byte[] thumbnailData = (byte[])rdr["thumbnail"];
                    if (thumbnailData.Length != 0)
                    {
                        using (MemoryStream ms = new MemoryStream(thumbnailData))
                        {
                            dataGridView1[Media_image, zeile].Value = Image.FromStream(ms);
                            //dataGridView1.Refresh();
                            dataGridView1.Rows[zeile].Height = 200;
                        }
                    }
                }
            }
            rdr.Close();
            dataGridView1.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Holt Thumbnails aus der Datenbank
            Cursor.Current = Cursors.WaitCursor;
            int i = 0;
            int k = dataGridView1.CurrentCell.RowIndex;
            string s = "";

            for (i = k; (i < k + int.Parse(pic_anz.Text)) && (i < dataGridView1.RowCount); i++)
            {
                if (dataGridView1[Media_image, i].Value is null && dataGridView1[Media_thumbnail_size, i].Value is not null)
                {
                    GetThumbnailFromDatabase(i);
                }
            }
            Cursor.Current = Cursors.Default;
        }

        private void chk_thump_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void pic_anz_TextChanged(object sender, EventArgs e)
        {

        }

        private void txt_records_TextChanged(object sender, EventArgs e)
        {
            if (txt_records.Text.StartsWith("&"))
            {
                sql_where_public = "";
            }
            else
            {
                sql_where_public = " mf.public = 1 ";
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (e.RowIndex >= 0) // Ensure the double-click is on a valid row
            {
                if (e.ColumnIndex == Media_name) // Filename column clicked
                {
                    // Get the filename and path
                    string fileName = dataGridView1.Rows[e.RowIndex].Cells[0].Value?.ToString();
                    string filePath = dataGridView1.Rows[e.RowIndex].Cells[1].Value?.ToString();

                    if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(filePath))
                    {
                        string fullPath = Path.Combine(filePath, fileName);

                        if (File.Exists(fullPath))
                        {
                            try
                            {
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = fullPath,
                                    UseShellExecute = true
                                });
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Failed to open the file.\nError: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("The file does not exist or the path is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                else if (e.ColumnIndex == Media_pfad) // Path column clicked
                {
                    // Get the path
                    string filePath = dataGridView1.Rows[e.RowIndex].Cells[1].Value?.ToString();
                    string fileName = dataGridView1.Rows[e.RowIndex].Cells[0].Value?.ToString();
                    string fullPath = Path.Combine(filePath, fileName);

                    if (!string.IsNullOrEmpty(fullPath))
                    {
                        if (File.Exists(fullPath)) // Ensure the file exists
                        {
                            try
                            {
                                //Process.Start(new ProcessStartInfo
                                //{
                                //    FileName = filePath,
                                //    UseShellExecute = true
                                //});                              

                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = "explorer",
                                    Arguments = $"/select,\"{fullPath}\"",
                                    UseShellExecute = true
                                });

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Failed to open Explorer.\nError: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("The directory does not exist or the path is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                else if (e.ColumnIndex == Media_latitude) // && dataGridView1.Rows[e.RowIndex].Cells[Media_latitude].Value is double latitude && latitude != 0)                  
                {
                    if (dataGridView1.Rows[e.RowIndex].Cells[Media_latitude].Value != null
                        && !string.IsNullOrWhiteSpace(dataGridView1.Rows[e.RowIndex].Cells[Media_latitude].Value.ToString()))
                    {
                        string lat = dataGridView1.Rows[e.RowIndex].Cells[Media_latitude].Value.ToString();
                        lat = lat.Replace(",", ".");
                        string lon = dataGridView1.Rows[e.RowIndex].Cells[Media_longitude].Value.ToString();
                        lon = lon.Replace(",", ".");
                        string url = "https://www.google.com/maps?q=" + lat + ',' + lon;
                        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                        Cursor.Current = Cursors.Default;
                    }
                }
                else if (e.ColumnIndex == Media_source)
                {
                    string filePath = dataGridView1.Rows[e.RowIndex].Cells[Media_pfad].Value.ToString();
                    string fileName = dataGridView1.Rows[e.RowIndex].Cells[Media_name].Value.ToString();

                    loadFileFromNas(fileName, filePath);
                }
                else if (e.ColumnIndex == Media_image)
                {
                    if (dataGridView1.Rows[e.RowIndex].Cells[Media_thumbnail_size].Value != null)
                    {    // Thumnaill vorhanden in der DB
                        if (dataGridView1[Media_image, e.RowIndex].Value is null)
                        {   // Thumbnail noch nicht von der DB gelesen
                            GetThumbnailFromDatabase(e.RowIndex);
                        }
                        var img = dataGridView1[Media_image, e.RowIndex].Value as Image;
                        if (img != null)
                        {
                            ShowZoomableImage(img);
                        }
                        else
                        {
                            MessageBox.Show("Kein Previewbild vorhanden!");
                        }
                    }
                }
                else if (e.ColumnIndex == Media_size_bytes)
                {
                    if (e.RowIndex < 0) return;

                    var imgObj = dataGridView1.Rows[e.RowIndex].Cells[Media_image].Value;
                    if (imgObj == null) return;

                    Image img = imgObj as Image;
                    if (img == null) return;

                    long id = 90653;

                    using (var win = new FaceWindow(img, id, dbconnection))
                    {
                        win.ShowDialog(this);
                    }
                }
            }
        }
        private void ShowZoomableImage(Image img)
        {
            Form zoomForm = new Form();
            zoomForm.Text = "Bild ansehen";
            zoomForm.Width = 800;
            zoomForm.Height = 600;

            PictureBox pb = new PictureBox();
            pb.Dock = DockStyle.Fill;
            pb.Image = img;
            pb.SizeMode = PictureBoxSizeMode.Zoom;

            // Mausrad-Zoom
            pb.MouseWheel += (s, e) =>
            {
                int newWidth = pb.Width + e.Delta;
                int newHeight = pb.Height + e.Delta * img.Height / img.Width;
                pb.Width = Math.Max(100, newWidth);
                pb.Height = Math.Max(100, newHeight);
            };

            pb.Focus(); // für Mausrad
            zoomForm.Controls.Add(pb);
            zoomForm.ShowDialog();
        }

        private void btn_help_Click(object sender, EventArgs e)
        {

            string helpText =
        "Doppelklick auf Spalte" + Environment.NewLine +
        "======================" + Environment.NewLine +
        "--> Dateiname: ladet die Datei vom Server über das Netzwerk und öffnet diese mit der Applikation, die mit der Extension verknüpft ist." + Environment.NewLine +
        "               User muss im LAN mit geeigneter User-Id eingeloggt sein." + Environment.NewLine + Environment.NewLine +
        "--> Pfad: öffnet den Explorer mit den entsprechendem Verzeichnis und markiert die Datei." + Environment.NewLine +
        "          User muss im LAN mit geeigneter User-Id eingeloggt sein." + Environment.NewLine + Environment.NewLine +
        "--> Preview: soferne eine Preview in der Datenbank verspeichert ist, wird diese mit der lokal verknüpften Applikation gestartet." + Environment.NewLine + Environment.NewLine +
        "--> Ordnertyp: ladet die entsprechende Datei vom Server mittels Datenbankzugriff in das lokale Downloadverzeichnis." + Environment.NewLine +
        "               User muss am Datenbankserver im entsprechenden Dateisystem zumindest Leserechte haben." + Environment.NewLine + Environment.NewLine +
        "--> Breitengrad: wenn ein GEO-Tag abgespeichert ist, wird der Browser(Google-Maps) mit den entsprechenden Koordinaten geöffnet.";

            using (var dlg = new Form())
            {
                dlg.Text = "Hilfe";
                dlg.ClientSize = new Size(1000, 270);
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;
                dlg.ShowIcon = false;
                dlg.StartPosition = FormStartPosition.CenterParent;

                var textBox = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    Dock = DockStyle.Fill,
                    ScrollBars = ScrollBars.Vertical,
                    BorderStyle = BorderStyle.None,
                    BackColor = SystemColors.Control,
                    Font = new Font("Courier New", 9f),
                    WordWrap = true,
                    Text = helpText
                };

                var buttonsPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Bottom,
                    Height = 40,
                    FlowDirection = FlowDirection.RightToLeft,
                    Padding = new Padding(6)
                };

                var ok = new Button
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Width = 90,
                    Height = 28
                };

                buttonsPanel.Controls.Add(ok);

                dlg.Controls.Add(textBox);
                dlg.Controls.Add(buttonsPanel);
                dlg.AcceptButton = ok;

                dlg.Shown += (s, ev) =>
                {
                    ok.Focus();
                    // Falls die TextBox dennoch ausgewählt ist: deselect
                    textBox.SelectionStart = 0;
                    textBox.SelectionLength = 0;
                    textBox.DeselectAll();
                };
                dlg.ShowDialog(this); // modal
            }
        }

        private void btnColumnSelect_Click(object sender, EventArgs e)
        {              
            bool[] aktuelleSichtbarkeit = new bool[Spalten_namen_col_select.Length];
            for (int i = 0; i < Spalten_namen_col_select.Length; i++)
            {
                if (dataGridView1.Columns.Contains(Spalten_namen_col_select[i]))
                    aktuelleSichtbarkeit[i] = dataGridView1.Columns[Spalten_namen_col_select[i]].Visible;
                else
                    aktuelleSichtbarkeit[i] = false;
            }

            using (var dlg = new ColumnSelectorForm(Spalten_namen_col_select, aktuelleSichtbarkeit))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    for (int i = 0; i < Spalten_namen_col_select.Length; i++)
                    {
                        if (dataGridView1.Columns.Contains(Spalten_namen_col_select[i]))
                            dataGridView1.Columns[Spalten_namen_col_select[i]].Visible = dlg.CheckedStates[i];
                    }
                }
            }        
        }
    }
}
    

