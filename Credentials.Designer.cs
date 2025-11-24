namespace Nas_Suchen
{
    partial class db_Login
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
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            txt_server = new TextBox();
            txt_dbname = new TextBox();
            txt_user = new TextBox();
            txt_password = new TextBox();
            btn_ok = new Button();
            btn_check = new Button();
            btn_cancel = new Button();
            label5 = new Label();
            txt_port = new TextBox();
            txt_url = new TextBox();
            label6 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Location = new Point(11, 8);
            label1.Name = "label1";
            label1.Size = new Size(122, 20);
            label1.TabIndex = 0;
            label1.Text = "DB-Server (nur IP)";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            label2.Location = new Point(11, 55);
            label2.Name = "label2";
            label2.Size = new Size(122, 20);
            label2.TabIndex = 1;
            label2.Text = "Datenbankname";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            label3.Location = new Point(11, 78);
            label3.Name = "label3";
            label3.Size = new Size(122, 20);
            label3.TabIndex = 2;
            label3.Text = "Benutzer";
            label3.TextAlign = ContentAlignment.MiddleRight;
            label3.Click += label3_Click;
            // 
            // label4
            // 
            label4.Location = new Point(11, 101);
            label4.Name = "label4";
            label4.Size = new Size(122, 20);
            label4.TabIndex = 3;
            label4.Text = "Passwort";
            label4.TextAlign = ContentAlignment.MiddleRight;
            label4.Click += label4_Click;
            // 
            // txt_server
            // 
            txt_server.Location = new Point(138, 8);
            txt_server.Name = "txt_server";
            txt_server.PlaceholderText = "192.168.1.22";
            txt_server.Size = new Size(165, 23);
            txt_server.TabIndex = 4;
            // 
            // txt_dbname
            // 
            txt_dbname.Location = new Point(138, 56);
            txt_dbname.Name = "txt_dbname";
            txt_dbname.PlaceholderText = "Media";
            txt_dbname.Size = new Size(165, 23);
            txt_dbname.TabIndex = 5;
            // 
            // txt_user
            // 
            txt_user.Location = new Point(138, 79);
            txt_user.Name = "txt_user";
            txt_user.PlaceholderText = "ha";
            txt_user.Size = new Size(165, 23);
            txt_user.TabIndex = 6;
            // 
            // txt_password
            // 
            txt_password.Location = new Point(138, 102);
            txt_password.Name = "txt_password";
            txt_password.PasswordChar = '*';
            txt_password.Size = new Size(165, 23);
            txt_password.TabIndex = 7;
            // 
            // btn_ok
            // 
            btn_ok.Location = new Point(162, 199);
            btn_ok.Name = "btn_ok";
            btn_ok.Size = new Size(92, 22);
            btn_ok.TabIndex = 8;
            btn_ok.Text = "Ok";
            btn_ok.UseVisualStyleBackColor = true;
            btn_ok.Click += btn_ok_Click;
            // 
            // btn_check
            // 
            btn_check.Location = new Point(12, 166);
            btn_check.Name = "btn_check";
            btn_check.Size = new Size(106, 22);
            btn_check.TabIndex = 9;
            btn_check.Text = "Test Connection";
            btn_check.UseVisualStyleBackColor = true;
            btn_check.Click += btn_check_Click;
            // 
            // btn_cancel
            // 
            btn_cancel.Location = new Point(162, 166);
            btn_cancel.Name = "btn_cancel";
            btn_cancel.Size = new Size(92, 22);
            btn_cancel.TabIndex = 10;
            btn_cancel.Text = "Cancel";
            btn_cancel.UseVisualStyleBackColor = true;
            btn_cancel.Click += btn_cancel_Click;
            // 
            // label5
            // 
            label5.Location = new Point(25, 33);
            label5.Name = "label5";
            label5.Size = new Size(108, 18);
            label5.TabIndex = 11;
            label5.Text = "Port";
            label5.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txt_port
            // 
            txt_port.Location = new Point(138, 32);
            txt_port.Name = "txt_port";
            txt_port.PlaceholderText = "3306";
            txt_port.Size = new Size(165, 23);
            txt_port.TabIndex = 12;
            // 
            // txt_url
            // 
            txt_url.Location = new Point(138, 126);
            txt_url.Name = "txt_url";
            txt_url.Size = new Size(165, 23);
            txt_url.TabIndex = 13;
            // 
            // label6
            // 
            label6.Location = new Point(31, 125);
            label6.Name = "label6";
            label6.Size = new Size(102, 20);
            label6.TabIndex = 14;
            label6.Text = "URL von Zertifikat";
            label6.TextAlign = ContentAlignment.MiddleRight;
            // 
            // db_Login
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(315, 240);
            ControlBox = false;
            Controls.Add(label6);
            Controls.Add(txt_url);
            Controls.Add(txt_port);
            Controls.Add(label5);
            Controls.Add(btn_cancel);
            Controls.Add(btn_check);
            Controls.Add(btn_ok);
            Controls.Add(txt_password);
            Controls.Add(txt_user);
            Controls.Add(txt_dbname);
            Controls.Add(txt_server);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            Name = "db_Login";
            Text = "Datenbank Credentials";
            Load += db_Login_Load_1;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private TextBox txt_server;
        private TextBox txt_dbname;
        private TextBox txt_user;
        private TextBox txt_password;
        private Button btn_ok;
        private Button btn_check;
        private Button btn_cancel;
        private Label label5;
        private TextBox txt_port;
        private TextBox txt_url;
        private Label label6;
    }
}