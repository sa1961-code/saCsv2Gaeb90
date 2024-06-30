using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saCsv2Gaeb90
{
    /// <summary>
    /// Verwaltet die Definition der Feldzuordnung
    /// </summary>
    internal class Csv2Gaeb90_Definition
    {
        public Csv2Gaeb90_Definition()
        {
            SourceText = "[Csv2Gaeb90]\r\n"
                + "OZMaske=LL1122PPPPI\r\n"
                + "DP=82\r\n"
                + "PosNr=Pos\r\n"
                + "Kurztext=Bezeichnung\r\n"
                + "Langtext=Bezeichnung\r\n"
                + "Titeltext=Bezeichnung\r\n"
                + "Hinweistext=Bezeichnung\r\n"
                + "Menge=Menge\r\n"
                + "Einheit=ME\r\n"
                + "EP=EP\r\n";
            FileName = "(unbenannt).ini";
        }

        /// <summary>
        /// Name der Definitionsdatei
        /// </summary>
        public string FileName { set; get; }

        /// <summary>
        /// Text der beschreibenden ini-Datei
        /// </summary>
        public string SourceText { set; get; }

        /// <summary>
        /// Csv-Objekt mit den aus der Beschreibung resultierenden Steuerungsdaten
        /// </summary>
        public CsvHandler ControlData
        {
            get { return new CsvHandler(GetCsvText()); }
        }

        /// <summary>
        /// Inhalt der ini-Datei als csv-formatierten Text zurueckgeben.
        /// </summary>
        /// <returns></returns>
        string GetCsvText()
        {
            string bsHead = "";
            string bsData = "";

            foreach (string rawLine in SourceText.Replace("\r","").Split('\n'))
            {
                string line = rawLine.Trim();
                if ((line.Trim().Length > 0) && (line[0] != ';') && (line[0] != '#') && (line[0] != '['))
                {
                    int nPos = line.IndexOf('=');
                    if (nPos > 0)
                    {
                        bsHead += ";" + line.Substring(0, nPos);
                        bsData += ";" + line.Substring(nPos + 1);
                    }
                }
            }
            // entferne das erste (sinnlose) Semikolon
            if (bsHead.Length > 0)
            {
                bsHead = bsHead.Substring(1);
                bsData = bsData.Substring(1);
            }
            return bsHead + "\r\n" + bsData + "\r\n";
        }
    }
}
