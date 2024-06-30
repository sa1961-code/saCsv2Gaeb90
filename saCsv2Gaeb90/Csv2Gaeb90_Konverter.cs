using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saCsv2Gaeb90
{
    /// <summary>
    /// Konvertiert eine csv-Datei mit Hilfe der Feldzuordnung in eine GAEB90-Datei
    /// </summary>
    internal class Csv2Gaeb90_Konverter
    {
        CsvHandler m_csvSource;
        CsvHandler m_csvDefinition;
        bool m_bMitLosen; // Losverarbeitung oder nicht
        int[] m_ozTokenLength; // Laenge der einzelnen Teile der Ordnungszahl
        int m_nPosTiefe; // Tiefe der Positionsebene
        int m_nCurrentPosition; // aktuelle Zeile in der csv-Quelle
        int m_nGaebLineNo; // aktuelle Zeilennummer der GAEB-Zieldatei

        public string DP { get; private set; }

        public Csv2Gaeb90_Konverter(CsvHandler csvSource, CsvHandler csvDefinition)
        {
            m_csvSource = csvSource;
            m_csvDefinition = csvDefinition;
            m_csvDefinition.setPosition(0);

            // OZMaske Checken
            string ozMaske = m_csvDefinition.getFieldValueByName("OZMaske");
            m_ozTokenLength = new int[10];
            m_ozTokenLength[0] = 2;
            m_ozTokenLength[1] = 2;
            m_ozTokenLength[2] = 4;
            m_ozTokenLength[3] = 1;
            m_nPosTiefe = 2;
            m_bMitLosen = true;
        }

        /// <summary>
        /// Gibt den (raw-)Inhalt eines Feldes in der Quelldatei zurück.
        /// Der uebergebene Feldname wird in der Definition ermittelt und in den Feldnamen der originalen Eingabe uebersetzt.
        /// Mit dem uebersetzen Feldnamen wird dann die Quelldatei analysiert.
        /// </summary>
        /// <param name="bsName"></param>
        /// <param name="bsDefault"></param>
        /// <returns></returns>
        string GetField(string bsName, string bsDefault = null)
        {
            string bsOrgFieldName = m_csvDefinition.getFieldValueByName(bsName);
            string bsResult = bsDefault;

            if (!String.IsNullOrEmpty(bsOrgFieldName))
                bsResult = CsvHandler.EncodeCsvField(m_csvSource.getFieldValueByName(bsOrgFieldName).Trim());
            if (String.IsNullOrEmpty(bsResult))
                bsResult = bsDefault;
            return bsResult == null ? "" : bsResult;
        }

        /// <summary>
        /// Gibt das Ergebnis der GAEB-Konvertierung zurueck
        /// </summary>
        /// <returns></returns>
        public string GetResult()
        {
            StringBuilder sbGaeb = new StringBuilder();
            m_nGaebLineNo = 0;

            DP = m_csvDefinition.getFieldValueByName("DP");
            string bsOzMaske = CalcOzMaske();
            string bsLosKz = m_bMitLosen ? "X" : " ";
            string bsLVName = "aus csv-Daten konvertiert";
            string bsLVDatum = Tools.DateToString(DateTime.Now, false);
            bsLVDatum = bsLVDatum.Substring(0, 6) + bsLVDatum.Substring(8, 2);

            // Satzarten 00, 01, 02, 03 und 08
            sbGaeb.AppendFormat("00{0}{1}{2}{3}90{4}{5}\r\n", FixedText("", 8), FixedText(DP, 2), FixedText("", 50), bsOzMaske, bsLosKz, GetLineNo());
            sbGaeb.AppendFormat("01{0}{1}{2}{3}\r\n", FixedText(bsLVName, 40), bsLVDatum, FixedText("", 24), GetLineNo());
            sbGaeb.AppendFormat("02{0}{1}\r\n", FixedText(bsLVName, 72), GetLineNo());
            sbGaeb.AppendFormat("03{0}{1}\r\n", FixedText("n.a.", 72), GetLineNo());
            sbGaeb.AppendFormat("04{0}{1}\r\n", FixedText("n.a.", 72), GetLineNo());
            sbGaeb.AppendFormat("08{0}{1}{2}\r\n", FixedText("EUR", 6), FixedText("Euro", 66), GetLineNo());

            for (m_nCurrentPosition = 0; m_nCurrentPosition < m_csvSource.getRowCount(); m_nCurrentPosition++)
            {
                m_csvSource.setPosition(m_nCurrentPosition);

                string bsOz = GetField("PosNr");
                switch (GetOZType(bsOz))
                {
                    case OZType.leer:
                        OnHinweis(bsOz, sbGaeb);
                        break;
                    case OZType.Los:
                        OnLosBeginn(bsOz, sbGaeb);
                        break;
                    case OZType.Titel:
                        OnTitelBeginn(bsOz, sbGaeb);
                        break;
                    case OZType.Position:
                        OnPosition(bsOz, sbGaeb);
                        break;
                }
            }
            return sbGaeb.ToString();
        }

        void OnLosBeginn(string bsOZ, StringBuilder sbGaeb)
        {
            string bsText = GetField("Titeltext");
            // Satzart 10
            sbGaeb.AppendFormat("10{0}{1}{2}{3}\r\n", FormatOZ(bsOZ), FixedText(bsText, 40), FixedText("",30), GetLineNo());

            for (m_nCurrentPosition += 1; m_nCurrentPosition < m_csvSource.getRowCount(); m_nCurrentPosition++)
            {
                m_csvSource.setPosition(m_nCurrentPosition);

                string bsNextOz = GetField("PosNr");
                switch (GetOZType(bsNextOz))
                {
                    case OZType.leer:
                        OnHinweis(bsNextOz, sbGaeb);
                        break;
                    case OZType.Los:
                        m_nCurrentPosition -= 1;
                        OnLosEnde(bsOZ, sbGaeb);
                        return;
                    case OZType.Titel:
                        OnTitelBeginn(bsNextOz, sbGaeb);
                        break;
                    case OZType.Position:
                        OnPosition(bsNextOz, sbGaeb);
                        break;
                }
            }
            OnLosEnde(bsOZ, sbGaeb);
        }

        void OnLosEnde(string bsOZ, StringBuilder sbGaeb)
        {
            // Satzart 33
            sbGaeb.AppendFormat("33{0}{1}{2}\r\n", FormatOZ(bsOZ), FixedText("", 70), GetLineNo());
        }

        void OnTitelBeginn(string bsOZ, StringBuilder sbGaeb)
        {
            string bsText = GetField("Titeltext");
            // Satzart 11
            sbGaeb.AppendFormat("11{0}N{1}{2}\r\n", FormatOZ(bsOZ), FixedText("", 62), GetLineNo());
            // Satzart 12
            sbGaeb.AppendFormat("12{0}{1}\r\n", FixedText(bsText, 72), GetLineNo());

            for (m_nCurrentPosition += 1; m_nCurrentPosition < m_csvSource.getRowCount(); m_nCurrentPosition++)
            {
                m_csvSource.setPosition(m_nCurrentPosition);

                string bsNextOz = GetField("PosNr");
                switch (GetOZType(bsNextOz))
                {
                    case OZType.leer:
                        OnHinweis(bsNextOz, sbGaeb);
                        break;
                    case OZType.Los:
                        m_nCurrentPosition -= 1;
                        OnTitelEnde(bsOZ, sbGaeb);
                        return;
                    case OZType.Titel:
                        if (GetOZTiefe(bsNextOz) <= GetOZTiefe(bsOZ))
                        {
                            m_nCurrentPosition -= 1;
                            OnTitelEnde(bsOZ, sbGaeb);
                            return;
                        }
                        OnTitelBeginn(bsNextOz, sbGaeb);
                        break;
                    case OZType.Position:
                        OnPosition(bsNextOz, sbGaeb);
                        break;
                }
            }
            OnTitelEnde(bsOZ, sbGaeb);
        }

        void OnTitelEnde(string bsOZ, StringBuilder sbGaeb)
        {
            // Satzart 31
            sbGaeb.AppendFormat("31{0}{1}{2}\r\n", FormatOZ(bsOZ), FixedText("", 63), GetLineNo());
            // Satzart 32 optional
        }

        void OnPosition(string bsOZ, StringBuilder sbGaeb)
        {
            // Satzart 21
            string bsEinheit = GetField("Einheit");
            string bsMenge = GetField("Menge");
            bsMenge = String.IsNullOrEmpty(bsMenge) ? FixedText("", 12) : FixedNum(Tools.StringToDouble(bsMenge), 12, 3);
            sbGaeb.AppendFormat("21{0}NNN{1}{2}{3}{4}{5}\r\n", FormatOZ(bsOZ), FixedText("", 8), bsMenge, FixedText(bsEinheit, 4), FixedText("", 36), GetLineNo());

            // Satzart 23 optional
            string bsEP = GetField("EP");
            if (!String.IsNullOrEmpty(bsEP))
                sbGaeb.AppendFormat("23{0}{1}{2}{3}\r\n", FormatOZ(bsOZ), FixedNum(Tools.StringToDouble(bsEP), 11, 2), FixedText("", 52), GetLineNo());

            // Satzart 25
            string bsKurzText = GetField("Kurztext");
            WriteKurztext(bsKurzText, sbGaeb);

            // Satzart 26
            string bsLangText = GetField("Langtext");
            WriteLangtext(bsLangText, sbGaeb);
        }

        void OnHinweis(string bsOZ, StringBuilder sbGaeb)
        {
            string bsText = GetField("Hinweistext");

            // Satzart 20
            sbGaeb.AppendFormat("20{0}{1}\r\n", FixedText("", 72), GetLineNo());
            WriteLangtext(bsText, sbGaeb);
        }

        void WriteKurztext(string bsText, StringBuilder sbGaeb)
        {
            // Satzart 25 Kurztext
            string[] lines = GetTextZeilen(bsText, 55); // Breite wie Langtext, um die Gleichheit der ersten Zeile sicherzustellen.
            if (lines.Length > 0)
                sbGaeb.AppendFormat("25{0}{1}{2}\r\n", FixedText(lines[0], 70), FixedText("", 2), GetLineNo());
        }

        void WriteLangtext(string bsText, StringBuilder sbGaeb)
        {
            // Satzart 26 Langtext
            string[] lines = GetTextZeilen(bsText, 55);
            foreach (string line in lines)
            {
                sbGaeb.AppendFormat("26{0}{1}{2}{3}\r\n", FixedText("", 3), FixedText(line, 55), FixedText("", 14), GetLineNo());
            }
        }

        string[] GetTextZeilen(string bsText, int nLen)
        {
            string[] rawLines = bsText.Replace('\t', ' ').Replace("\r", "").Split('\n');
            if (nLen <= 0)
            {
                return rawLines;
            }

            List<string> Lines = new List<string>();
            foreach (string rawLine in rawLines)
            {
                string[] words = rawLine.Split(' ');
                string sammel = "";
                foreach (string word in words)
                {
                    string nextSammel = sammel + (String.IsNullOrEmpty(sammel) ? "" : " ")  + word;
                    if (nextSammel.Length > nLen)
                    {
                        Lines.Add(sammel);
                        sammel = word;
                    }
                    else
                    {
                        sammel = nextSammel;
                    }
                }
                if (sammel.Length > 0)
                {
                    Lines.Add(sammel);
                }
            }
            return Lines.ToArray();
        }

        int GetOZTiefe(string bsOZ)
        {
            string[] tokens = bsOZ.Split('.');
            int nMinIndex = m_bMitLosen ? 1 : 0;
            int nIndex = m_nPosTiefe + nMinIndex;

            if ((tokens.Length > nIndex) && (!String.IsNullOrEmpty(tokens[nIndex].Trim())))
                return nIndex;

            for (nIndex -= 1; nIndex >= nMinIndex; nIndex--)
                if ((tokens.Length > nIndex) && (!String.IsNullOrEmpty(tokens[nIndex].Trim())))
                    return nIndex;

            return 0;
        }

        OZType GetOZType(string bsOZ)
        {
            string[] tokens = bsOZ.Split('.');
            int nMinIndex = m_bMitLosen ? 1 : 0;
            int nIndex = m_nPosTiefe + nMinIndex;

            if ((tokens.Length > nIndex) && (!String.IsNullOrEmpty(tokens[nIndex].Trim())))
                return OZType.Position;

            for (nIndex -= 1; nIndex >= nMinIndex; nIndex--)
                if ((tokens.Length > nIndex) && (!String.IsNullOrEmpty(tokens[nIndex].Trim())))
                    return OZType.Titel;

            if (m_bMitLosen)
                if ((tokens.Length > 0) && (!String.IsNullOrEmpty(tokens[0].Trim())))
                    return OZType.Los;

            return OZType.leer;
        }

        string FormatOZ(string bsOZ)
        {
            string[] tokens = bsOZ.Split('.');
            int nMinIndex = m_bMitLosen ? 1 : 0;

            // Titel/Positionen
            for (int nIndex = m_nPosTiefe + nMinIndex; nIndex >= nMinIndex; nIndex--)
                if ((tokens.Length > nIndex) && (!String.IsNullOrEmpty(tokens[nIndex].Trim())))
                {
                    string ozResult = "";
                    for (int i = 0; i < (nIndex + nMinIndex); i++)
                    {
                        ozResult += tokens[i + nMinIndex].Trim().PadLeft(m_ozTokenLength[i]);
                    }
                    return FixedText(ozResult,9);
                }

            // Los
            if (m_bMitLosen)
                if ((tokens.Length > 0) && (!String.IsNullOrEmpty(tokens[0].Trim())))
                {
                    return tokens[0].Trim().PadLeft(2);
                }

            return "";
        }

        string CalcOzMaske()
        {
            string bsResult = "";
            for (int i = 0; i < m_nPosTiefe; i++)
                bsResult += "".PadLeft(m_ozTokenLength[i], (char)('1' + i));
            bsResult += "".PadLeft(m_ozTokenLength[m_nPosTiefe], 'P') + "I";
            return bsResult.PadRight(9, '0').Substring(0, 9);
        }

        // auszugebender Text mit fester Feldlaenge
        string FixedText(string bsText, int nLen)
        {
            return bsText.PadRight(nLen).Substring(0, nLen);
        }

        // auszugebendes numerisches Feld mit fester Feldlaenge und Vornullen
        string FixedNum(double dValue, int nLen, int nDec)
        {
            long l = (long)(dValue * Math.Pow(10, nDec));

            if (l < 0)
                return "-" + (0 - l).ToString().PadLeft(nLen - 1, '0');
            else
                return " " + l.ToString().PadLeft(nLen - 1, '0');
        }

        // auszugebende Zeilenummer fuer die Zieldatei
        string GetLineNo()
        {
            return (++m_nGaebLineNo).ToString().PadLeft(6, '0');
        }
    }

    internal enum OZType
    {
        leer, Los, Titel, Position
    }
}
