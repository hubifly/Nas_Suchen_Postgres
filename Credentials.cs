using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Net;

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
            SaveCredentials(txt_server.Text, txt_dbname.Text, txt_user.Text, txt_password.Text, txt_port.Text);
                    
            Globals1.D_server = txt_server.Text;
            Globals1.D_port = txt_port.Text;
            Globals1.D_db = txt_dbname.Text;
            Globals1.D_user = txt_user.Text;
            Globals1.D_pw = txt_password.Text;

            this.Close();
        }

        private void SaveCredentials(string server, string dbName, string user, string password, string port)
        {
            string credentialData = $"{server}|{dbName}|{user}|{password}|{port}";

            // Windows expects Unicode
            byte[] credentialBytes = Encoding.Unicode.GetBytes(credentialData);
            IntPtr credBlobPtr = Marshal.AllocHGlobal(credentialBytes.Length + 2);
            Marshal.Copy(credentialBytes, 0, credBlobPtr, credentialBytes.Length);
            Marshal.WriteInt16(credBlobPtr, credentialBytes.Length, 0); // Unicode null terminator

            byte[] targetNameBytes = Encoding.Unicode.GetBytes(CredentialAppName);
            IntPtr targetNamePtr = Marshal.AllocHGlobal(targetNameBytes.Length + 2);
            Marshal.Copy(targetNameBytes, 0, targetNamePtr, targetNameBytes.Length);
            Marshal.WriteInt16(targetNamePtr, targetNameBytes.Length, 0); // Unicode null terminator

            byte[] commentBytes = Encoding.Unicode.GetBytes("Database Credentials");
            IntPtr commentPtr = Marshal.AllocHGlobal(commentBytes.Length + 2);
            Marshal.Copy(commentBytes, 0, commentPtr, commentBytes.Length);
            Marshal.WriteInt16(commentPtr, commentBytes.Length, 0); // Unicode null terminator

            byte[] userNameBytes = Encoding.Unicode.GetBytes(Environment.UserName);
            IntPtr userNamePtr = Marshal.AllocHGlobal(userNameBytes.Length + 2);
            Marshal.Copy(userNameBytes, 0, userNamePtr, userNameBytes.Length);
            Marshal.WriteInt16(userNamePtr, userNameBytes.Length, 0); // Unicode null terminator

            //MessageBox.Show(Environment.UserName);

            var cred = new NativeCredential
            {
                Flags = 0,
                CredentialType = CredentialType.Generic,
                TargetName = targetNamePtr,
                Comment = commentPtr,
                LastWritten = default(FILETIME), // Set to 0 explicitly
                CredentialBlob = credBlobPtr,
                CredentialBlobSize = (uint)credentialBytes.Length + 2, // FIXED: Add 2 for Unicode null terminator
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
                int lastError = Marshal.GetLastWin32Error();
                MessageBox.Show($"Failed to save credentials. Error code: {lastError}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReadCredentials()
        {
            IntPtr credPtr;
            if (NativeMethods.CredRead(CredentialAppName, CredentialType.Generic, 0, out credPtr))
            {
                var cred = (NativeCredential)Marshal.PtrToStructure(credPtr, typeof(NativeCredential));

                byte[] credentialBytes = new byte[cred.CredentialBlobSize];
                Marshal.Copy(cred.CredentialBlob, credentialBytes, 0, (int)cred.CredentialBlobSize);

                // FIXED: Use Encoding.Unicode instead of UTF8
                string credentialData = Encoding.Unicode.GetString(credentialBytes).TrimEnd('\0');

                string[] parts = credentialData.Split('|');

                txt_server.Text = parts[0];
                txt_dbname.Text = parts[1];
                txt_user.Text = parts[2];
                txt_password.Text = parts[3];
                txt_port.Text = parts.Length > 4 ? parts[4] : string.Empty;         

                NativeMethods.CredFree(credPtr);
            }
        }

        // ======== Native Methods for Credential Manager ========
        internal enum CredentialPersistence : uint
        {
            Session = 1,
            LocalMachine = 2,
            Enterprise = 3
        }

        internal enum CredentialType : uint
        {
            Generic = 1,
            DomainPassword = 2,
            DomainCertificate = 3,
            DomainVisiblePassword = 4,
            GenericCertificate = 5,
            DomainExtended = 6,
            Maximum = 7 // Always last
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct FILETIME
        {
            public uint dwLowDateTime;
            public uint dwHighDateTime;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct NativeCredential
        {
            public uint Flags;
            public CredentialType CredentialType;
            public IntPtr TargetName;
            public IntPtr Comment;
            public FILETIME LastWritten;           // FILETIME (8 bytes)
            public uint CredentialBlobSize;
            public IntPtr CredentialBlob;
            public CredentialPersistence Persist;  // DWORD
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
        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btn_check_Click(object sender, EventArgs e)
        {
            string connect = "";

            if (Environment.OSVersion.Version.Build < 22000)
            {
                if (txt_password.Text != "")
                { // Windows 10
                    connect = $"server = {txt_server.Text}; port = {txt_port.Text}; Database = {txt_dbname.Text}; uid = {txt_user.Text}; pwd = {txt_password.Text}; Charset = utf8;" +
                                     "SslMode = VerifyCA; SslCa = C:\\Certificate\\ca-cert.pem";
                }
                else
                { // Windows 10
                    connect = $"server = {txt_server.Text}; port = {txt_port.Text}; Database = {txt_dbname.Text}; uid = {txt_user.Text}; Charset = utf8;" +
                                     "SslMode = VerifyCA; SslCa = C:\\Certificate\\ca-cert.pem";
                }
            }
            else
            {
                if (txt_password.Text != "")
                {
                    // Windows 11
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
                    connect = $"server={txt_server.Text}; port={txt_port.Text}; Database={txt_dbname.Text}; uid={txt_user.Text}; pwd={txt_password.Text}; Charset = utf8;TlsVersion=TlSv1.3;" +
                              "SslMode = VerifyFull; SslCa = C:\\Certificate\\ca-cert.pem;SslCert=C:\\Certificate\\client-cert.pem;SslKey=C:\\Certificate\\client-key.pem;";
                    //     "SslMode = VerifyCA; SslMode=Required"
                }
                else
                {
                    // Windows 11
                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
                    //connect = $"server={txt_server.Text}; port={txt_port.Text}; Database={txt_dbname.Text}; uid={txt_user.Text}; pwd=''; Charset = utf8;TlsVersion=Tlsv1.3;" +
                    //          "SslMode=VerifyFull;SslCa=C:\\Certificate\\ca-cert.pem;SslCert=C:\\Certificate\\client-cert.pem;SslKey=C:\\Certificate\\client-key.pem;";

                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
                    connect = $"server={txt_server.Text}; port={txt_port.Text}; Database={txt_dbname.Text}; uid={txt_user.Text}; pwd=''; Charset = utf8;" +
                        $"SslMode=VerifyFull;SslCa=C:\\Certificate\\ca-cert.pem;SslCert=C:\\Certificate\\client-cert.pem;SslKey=C:\\Certificate\\client-key.pem;";

                    //     "SslMode = VerifyCA; SslMode=Required"
                }
            }

            MySql.Data.MySqlClient.MySqlConnection dbconnection = new MySql.Data.MySqlClient.MySqlConnection();
  
            try
            {
                dbconnection.ConnectionString = connect;
                dbconnection.Open();
                MessageBox.Show("Connection to: " + dbconnection.ServerVersion + " erfolgreich!");
                MessageBox.Show(connect);
                dbconnection.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            Globals1.D_cancel = 1;
            this.Close();
        }
    }
}
