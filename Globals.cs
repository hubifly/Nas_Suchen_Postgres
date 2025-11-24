using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nas_Suchen
{
    public class Globals1
    {
        // DB
        public static string D_server = "";
        public static string D_port = "";
        public static string D_db = "";
        public static string D_user = "";
        public static string D_pw = "";
        public static int D_cancel = 0;

        // NAS Session
        public static string Nas_connect = null;    
        public static int Nas_local = 1;        // 1 = LAN (HTTP), 0 = WAN (HTTPS)

        // NAS Adressen
        public static string D_QNAP_Cloud_id = ""; // WAN Domain → HTTPS
    }
}
