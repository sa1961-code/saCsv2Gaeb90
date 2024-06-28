using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace saCsv2Gaeb90
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Csv2Gaeb90_MainForm());
        }
    }
}
