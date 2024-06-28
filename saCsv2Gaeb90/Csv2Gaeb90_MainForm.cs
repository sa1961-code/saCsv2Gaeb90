using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace saCsv2Gaeb90
{
    public partial class Csv2Gaeb90_MainForm : Form
    {
        SqlCsvResult CsvDefinition = new SqlCsvResult("OZMaske;OZ;Text;Menge;Einheit\r\n" + "LL1122PPPPI;OZ;Kurztext alt;Massen;Einheiten\r\n");
        SqlCsvResult CsvSource = null;
        string CsvFileName = "*.csv";

        int nViewMode = 0;  // Quelle, Definition, Ergebnis

        public Csv2Gaeb90_MainForm()
        {
            InitializeComponent();


            comboBoxView.Items.Add("Quelle");
            comboBoxView.Items.Add("Definition");
            comboBoxView.Items.Add("Ergebnis");
            comboBoxView.SelectedIndex = 0;
        }

        private void comboBoxView_SelectedIndexChanged(object sender, EventArgs e)
        {
            nViewMode = comboBoxView.SelectedIndex;
            UpdateControls();
        }

        void UpdateControls()
        {
            switch (nViewMode)
            {
                case 0:
                    textBoxContent.Text = CsvSource != null ? CsvSource.ToCsvText(true) : "(keine Datei geladen)";
                    textBoxContent.ReadOnly = true;
                    buttonSave.Enabled = false;
                    break;
                case 1:
                    textBoxContent.Text = CsvDefinition.ToCsvText(true);
                    textBoxContent.ReadOnly = false;
                    buttonSave.Enabled = false;
                    break;
                case 2:
                    if ((CsvSource != null) && (CsvDefinition != null))
                    {
                        Csv2Gaeb90_Konverter Konverter = new Csv2Gaeb90_Konverter(CsvSource, CsvDefinition);
                        textBoxContent.Text = Konverter.GetResult(83);
                        buttonSave.Enabled = true;
                    }
                    else
                    {
                        textBoxContent.Text = "(leer)";
                        buttonSave.Enabled = false;
                    }
                    textBoxContent.ReadOnly = true;
                    break;
                default:
                    textBoxContent.Text = "";
                    textBoxContent.ReadOnly = true;
                    break;
            }
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            string bsTempName = Tools.AskOpenFileName(CsvFileName, "csv");
            string bsTempText;
            try
            {
                bsTempText = System.IO.File.ReadAllText(bsTempName, Encoding.Default);
            }
            catch (Exception)
            {
                MessageBox.Show("Die Datei konnte nicht eingelesen werden.");
                return;
            }

            SqlCsvResult csv = new SqlCsvResult();
            if (!csv.Attach(SqlCsvResult.ExcelCsvToInternalCsv(bsTempText)))
            {
                MessageBox.Show("Die Datei wurde nicht als csv-Format erkannt.");
                return;
            }

            CsvFileName = bsTempName;
            CsvSource = csv;
            this.Text = CsvFileName;
            comboBoxView.SelectedIndex = 0;
            UpdateControls();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // TODO nViewMode
            string bsExtension = "d83";
            string bsName = CsvFileName.Substring(0, CsvFileName.LastIndexOf('.')) + "." + bsExtension;

            bsName = Tools.AskSaveFileName(bsName, bsExtension);
            if (!String.IsNullOrEmpty(bsName))
            {
                System.IO.File.WriteAllText(bsName, textBoxContent.Text, Encoding.GetEncoding(858));
            }
        }
    }
}
