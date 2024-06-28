using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saCsv2Gaeb90
{
    internal class SqlCsvResult
    {
        string[] lines;
        string[] fieldNames;
        int fieldCount;
        int rowCount;
        string[] currentRow;

        public SqlCsvResult()
        {
        }

        public SqlCsvResult(string bsText)
        {
            Attach(bsText);
        }

        public bool Attach(string csvText)
        {
            csvText = csvText.Replace("\r", "");
            int csvLen = csvText.Length;
            if ((csvLen > 0) && (csvText[csvLen - 1] == '\n'))
                csvText = csvText.Substring(0, csvLen - 1);
            lines = csvText.Split('\n');
            rowCount = lines.Length - 1;
            fieldCount = 0;

            if (rowCount >= 0)
            {
                fieldNames = lines[0].Split(';');
                fieldCount = fieldNames.Length;
            }
            setPosition(0);
            return fieldCount > 0;
        }

        public bool setPosition(int n)
        {
            if ((n >= 0) && (n < rowCount))
            {
                currentRow = lines[n + 1].Split(';');
                return true;
            }
            currentRow = null;
            return false;
        }

        public string getFieldName(int n)
        {
            return (n >= 0) && (n < fieldCount) ? fieldNames[n] : "";
        }

        public string getFieldValue(int n)
        {
            return (currentRow != null) && (n >= 0) && (n < currentRow.Length) ? currentRow[n] : "";
        }

        public string getFieldValueByName(string bsName)
        {
            for (int i = 0; i < fieldCount; i++)
                if (getFieldName(i) == bsName)
                    return getFieldValue(i);
            return "";
        }

        public string getRowRaw(int n)
        {
            return (n >= 0) && (n < rowCount) ? lines[n + 1] : "";
        }

        public void setFieldValue(int n, string bsValue)
        {
            if ((currentRow != null) && (n < currentRow.Length))
                currentRow[n] = bsValue;
        }

        public void setFieldValueByName(string bsName, string bsValue)
        {
            for (int i = 0; i < fieldCount; i++)
                if (getFieldName(i) == bsName)
                {
                    setFieldValue(i, bsValue);
                    return;
                }
        }

        public void setRowRaw(int n, string newLine)
        {
            if ((n >= 0) && (n < rowCount))
                lines[n + 1] = newLine;
        }

        public void FlushCurrentRowTo(int n)
        {
            string bsLine = "";
            for (int i = 0; i < currentRow.Length; i++)
            {
                if (i > 0) bsLine += ";";
                bsLine += currentRow[i];
            }
            setRowRaw(n, bsLine);
        }

        public int appendRow()
        {
            List<string> tmpList = lines.ToList();
            tmpList.Add("".PadRight(fieldCount - 1, ';'));
            lines = tmpList.ToArray();
            rowCount = lines.Length - 1;
            return rowCount - 1;
        }

        public string ToCsvText(bool WithHeader)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = WithHeader ? 0 : 1; i < lines.Length; i++)
                sb.AppendLine(lines[i]);
            return sb.ToString();
        }

        public int getRowCount()
        {
            return rowCount;
        }

        public void setRowCount(int n)
        {
            while (n > rowCount)
                appendRow();
            rowCount = n;
        }

        public int getFieldCount()
        {
            return fieldCount;
        }

        public static string EncodeCsvField(string bsSrc)
        {
            StringBuilder sbResult = new StringBuilder();
            int nLen = bsSrc.Length;
            //bool FieldStart = true;
            //bool QuotedField = false;

            for (int i = 0; i < nLen; )
            {
                char c = bsSrc[i++];
                if ((c == '\\') && (i < nLen))
                {
                    c = bsSrc[i++];
                    switch (c)
                    {
                        case 'r':
                            sbResult.Append('\r');
                            break;
                        case 'n':
                            sbResult.Append('\n');
                            break;
                        case 't':
                            sbResult.Append('\t');
                            break;
                        case 'q':
                            sbResult.Append('"');
                            break;
                        case 's':
                            sbResult.Append(';');
                            break;
                        default:
                            sbResult.Append(c);
                            break;
                    }
                }
                else
                {
                    sbResult.Append(c);
                }
            }

            return sbResult.ToString();
        }

        public static string ExcelCsvToInternalCsv(string bsSrc)
        {
            StringBuilder sbResult = new StringBuilder();
            int nLen = bsSrc.Length;
            bool FieldStart = true;
            bool QuotedField = false;

            for (int i = 0; i < nLen; )
            {
                char c = bsSrc[i++];
                switch (c)
                {
                    case '\r':
                    case '\n':
                    case ';':
                        if (FieldStart || (!QuotedField))
                        {
                            sbResult.Append(c);
                            FieldStart = true;
                            QuotedField = false;
                            continue;
                        }
                        break;
                    case '"':
                        if (FieldStart)
                        {
                            QuotedField = true;
                            FieldStart = false;
                            continue;
                        }
                        if (QuotedField && (i < nLen) && (bsSrc[i] == '"'))
                        {
                            c = bsSrc[i++];
                            sbResult.Append(ExcelCharToInternal(c));
                            continue;
                        }
                        if (QuotedField)
                        {
                            QuotedField = false;
                            continue;
                        }
                        break;
                    default:
                        FieldStart = false;
                        break;
                }
                sbResult.Append(ExcelCharToInternal(c));
            }

            return sbResult.ToString();
        }

        public static string ExcelCharToInternal(char c)
        {
            switch (c)
            {
                case '\r':
                    return "\\r";
                case '\n':
                    return "\\n";
                case '\t':
                    return "\\t";
                case '\\':
                    return "\\\\";
                case '"':
                    return "\\q";
                case ';':
                    return "\\s";
                default:
                    return "" + c;
            }
        }
    }
}
