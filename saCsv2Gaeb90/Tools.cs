using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace saCsv2Gaeb90
{
    /// <summary>
    /// statische Funtionen, die in keinem besondern Kontext stehen
    /// </summary>
    internal class Tools
    {
        public static double StringToDouble(string bsParam)
        {
            bsParam = bsParam.Trim();
            if (String.IsNullOrEmpty(bsParam)) return 0.00;

            StringBuilder sb = new StringBuilder(30);
            bool bDigit = false;
            int nKomma = 0;
            bool bKommaAfterPunkt = false;
            int nPunkt = 0;
            bool bPunktAfterKomma = false;
            foreach (char c in bsParam)
            {
                if ((c >= '0') && (c <= '9'))
                {
                    sb.Append(c);
                    bDigit = true;
                }
                else switch (c)
                    {
                        case '-':
                            if (!bDigit)
                            {
                                sb.Append(c);
                                bDigit = true;
                            }
                            break;
                        case '+':
                            if (!bDigit)
                            {
                                bDigit = true;
                            }
                            break;
                        case ',':
                            sb.Append(c);
                            nKomma++;
                            bKommaAfterPunkt = nPunkt > 0;
                            break;
                        case '.':
                            sb.Append(c);
                            nPunkt++;
                            bPunktAfterKomma = nKomma > 0;
                            break;
                    }
            }

            string s = sb.ToString();
            if (nKomma > 0)
            {
                if (bKommaAfterPunkt)
                    s = s.Replace(".", "");
                s = s.Replace(",", ".");
            }
            if (String.IsNullOrEmpty(s)) return 0;

            try { return double.Parse(s, System.Globalization.CultureInfo.InvariantCulture); }
            catch (Exception) { return 0; }
        }

        public static int StringToInt(string bsParam)
        {
            return (int)StringToDouble(bsParam);
        }
        
        public static bool StringToBool(string bsParam)
        {
            return StringToInt(bsParam) != 0;
        }

        public static string DateToString(DateTime d, bool bOdbc = false)
        {
            return bOdbc ? d.ToString("yyyy-MM-dd") : d.ToString("dd.MM.yyyy");
        }

        public static string AskOpenFileName(string bsFileName, string bsExtension)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = bsExtension + "-Dateien (*." + bsExtension + ")|*." + bsExtension + "|alle Dateien (*.*)|*.*";
            ofd.FileName = bsFileName;
            ofd.CheckFileExists = true;
            ofd.DereferenceLinks = true;
            ofd.Multiselect = false;
            ofd.Title = "Datei öffnen...";
            return ofd.ShowDialog() == DialogResult.OK ? ofd.FileName : "";
        }

        public static string AskSaveFileName(string bsFileName, string bsExtension)
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.Filter = bsExtension + "-Dateien (*." + bsExtension + ")|*." + bsExtension + "|alle Dateien (*.*)|*.*";
            ofd.FileName = bsFileName;
            ofd.OverwritePrompt = true;
            //ofd.CheckFileExists = true;
            //ofd.DereferenceLinks = true;
            //ofd.Multiselect = false;
            ofd.Title = "Datei speichern...";
            return ofd.ShowDialog() == DialogResult.OK ? ofd.FileName : "";
        }

        public static string GetMainModulePath()
        {
            System.Diagnostics.Process p = System.Diagnostics.Process.GetCurrentProcess();
            string bsPath = System.IO.Path.GetDirectoryName(p.MainModule.FileName);
            if (bsPath.Substring(bsPath.Length - 1) != "\\")
                bsPath += "\\";
            return bsPath;
        }
    }
}
