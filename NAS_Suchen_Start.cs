using System;
using System.Windows.Forms;

namespace Nas_Suchen
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Nas_Suche());
        }
    }
}
