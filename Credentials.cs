using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Npgsql;

namespace Nas_Suchen
{
    public partial class db_Login : Form
    {
        private const string CredentialAppName = "Nas_suchen";

        public db_Login()
        {
            InitializeComponent();
        }

        private void db_Login_Load_1(object sender, EventArgs e)
        {
            ReadCredentials();
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            SaveCredentials(
                txt_server.Text,
                txt_dbname.Text,
                txt_user.Text,
                txt_password.Text,
                txt_port.Text,
                txt_url.Text
            );

            Globals1.D_server = txt_server.Text;
            Globals1.D_port = txt_port.Text;
            Globals1.D_db = txt_dbname.Text;
            Globals1.D_user = txt_user.Text;
            Globals1.D_pw = txt_password.Text;
            Globals1.D_QNAP_Cloud_id = txt_url.Text;

            this.Close();
        }

        // --------------------------------------------------------------------
        // Credentials speichern
        // --------------------------------------------------------------------
        private void SaveCredentials(string server, string dbName, string user, string password, string port, string wanUrl)
        {
            string credentialData = $"{server}|{dbName}|{user}|{password}|{port}|{wanUrl}";
            byte[] credentialBytes = Encoding.Unicode.GetBytes(credentialData);

            IntPtr credBlobPtr = Marshal.AllocHGlobal(credentialBytes.Length + 2);
            Marshal.Copy(credentialBytes, 0, credBlobPtr, credentialBytes.Length);
            Marshal.WriteInt16(credBlobPtr, credentialBytes.Length, 0);

            byte[] targetNameBytes = Encoding.Unicode.GetBytes(CredentialAppName);
            IntPtr targetNamePtr = Marshal.AllocHGlobal(targetNameBytes.Length + 2);
            Marshal.Copy(targetNameBytes, 0, targetNamePtr, targetNameBytes.Length);
            Marshal.WriteInt16(targetNamePtr, targetNameBytes.Length, 0);

            byte[] commentBytes = Encoding.Unicode.GetBytes("Database Credentials");
            IntPtr commentPtr = Marshal.AllocHGlobal(commentBytes.Length + 2);
            Marshal.Copy(commentBytes, 0, commentPtr, commentBytes.Length);
            Marshal.WriteInt16(commentPtr, commentBytes.Length, 0);

            byte[] userNameBytes = Encoding.Unicode.GetBytes(Environment.UserName);
            IntPtr userNamePtr = Marshal.AllocHGlobal(userNameBytes.Length + 2);
            Marshal.Copy(userNameBytes, 0, userNamePtr, userNameBytes.Length);
            Marshal.WriteInt16(userNamePtr, userNameBytes.Length, 0);

            var cred = new NativeCredential
            {
                Flags = 0,
                CredentialType = CredentialType.Generic,
                TargetName = targetNamePtr,
                Comment = commentPtr,
                LastWritten = default(FILETIME),
                CredentialBlob = credBlobPtr,
                CredentialBlobSize = (uint)credentialBytes.Length + 2,
                AttributeCount = 0,
                Attributes = IntPtr.Zero,
                UserName = userNamePtr,
                Persist = CredentialPersistence.LocalMachine,
                TargetAlias = IntPtr.Zero
            };

            bool saved = NativeMethods.CredWrite(ref cred, 0);

            Marshal.FreeHGlobal(credBlobPtr);
            Marshal.FreeHGlobal(targetNamePtr);
            Marshal.FreeHGlobal(commentPtr);
            Marshal.FreeHGlobal(userNamePtr);

            if (!saved)
            {
                MessageBox.Show("Credentials konnten nicht gespeichert werden.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --------------------------------------------------------------------
        // Credentials lesen
        // --------------------------------------------------------------------
        private void ReadCredentials()
        {
            IntPtr credPtr;

            if (NativeMethods.CredRead(CredentialAppName, CredentialType.Generic, 0, out credPtr))
            {
                var cred = (NativeCredential)Marshal.PtrToStructure(credPtr, typeof(NativeCredential));

                byte[] credentialBytes = new byte[cred.CredentialBlobSize];
                Marshal.Copy(cred.CredentialBlob, credentialBytes, 0, (int)cred.CredentialBlobSize);

                string credentialData = Encoding.Unicode.GetString(credentialBytes).TrimEnd('\0');
                string[] parts = credentialData.Split('|');

                txt_server.Text = parts[0];
                txt_dbname.Text = parts[1];
                txt_user.Text = parts[2];
                txt_password.Text = parts[3];
                txt_port.Text = parts.Length > 4 ? parts[4] : "";
                txt_url.Text = parts.Length > 5 ? parts[5] : "";

                NativeMethods.CredFree(credPtr);
            }
        }

        // --------------------------------------------------------------------
        // POSTGRES → Test Connection (ohne SSL)
        // --------------------------------------------------------------------
        private void btn_check_Click(object sender, EventArgs e)
        {
            try
            {
                var builder = new NpgsqlConnectionStringBuilder
                {
                    Host = txt_server.Text,
                    Port = int.Parse(txt_port.Text),
                    Database = txt_dbname.Text,
                    Username = txt_user.Text,
                    Password = txt_password.Text,
                    SslMode = SslMode.Disable
                };

                using (var conn = new NpgsqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    MessageBox.Show(
                        "PostgreSQL Verbindung erfolgreich!\n\nVersion: " +
                        conn.PostgreSqlVersion
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Verbindungsfehler:\n" + ex.Message);
            }
        }

        private void label4_Click(object sender, EventArgs e) { }

        private void label3_Click(object sender, EventArgs e) { }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            Globals1.D_cancel = 1;
            this.Close();
        }

        // --------------------------------------------------------------------
        // Windows Credential Store Struct/Imports
        // --------------------------------------------------------------------
        internal enum CredentialPersistence : uint { Session = 1, LocalMachine = 2, Enterprise = 3 }
        internal enum CredentialType : uint { Generic = 1 }

        [StructLayout(LayoutKind.Sequential)]
        internal struct FILETIME { public uint dwLowDateTime; public uint dwHighDateTime; }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct NativeCredential
        {
            public uint Flags;
            public CredentialType CredentialType;
            public IntPtr TargetName;
            public IntPtr Comment;
            public FILETIME LastWritten;
            public uint CredentialBlobSize;
            public IntPtr CredentialBlob;
            public CredentialPersistence Persist;
            public uint AttributeCount;
            public IntPtr Attributes;
            public IntPtr TargetAlias;
            public IntPtr UserName;
        }

        internal static class NativeMethods
        {
            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern bool CredWrite(ref NativeCredential credential, uint flags);

            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern bool CredRead(string target, CredentialType type, int reservedFlag, out IntPtr credential);

            [DllImport("advapi32.dll", SetLastError = true)]
            public static extern void CredFree(IntPtr buffer);
        }
    }
}
