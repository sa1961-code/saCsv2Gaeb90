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
        Csv2Gaeb90_Definition m_CsvDefinition = new Csv2Gaeb90_Definition();
        CsvHandler m_CsvSource = null;
        string m_CsvFileName = "*.csv";
        string m_DP = "";

        int nViewMode = 0;  // Quelle, Definition, Ergebnis

        public Csv2Gaeb90_MainForm()
        {
            InitializeComponent();

            comboBoxView.Items.Add("Quelle");
            comboBoxView.Items.Add("Definition");
            comboBoxView.Items.Add("Steuerung");
            comboBoxView.Items.Add("GAEB-Daten");
            comboBoxView.SelectedIndex = 1;

            // typeof(Csv2Gaeb90_MainForm).Assembly.GetName.Version.ToString();
        }

        private void comboBoxView_SelectedIndexChanged(object sender, EventArgs e)
        {
            nViewMode = comboBoxView.SelectedIndex;
            UpdateControls();
        }

        void UpdateControls()
        {
            textBoxDefintionName.Text = m_CsvDefinition.FileName;

            switch (nViewMode)
            {
                case 0:
                    textBoxContent.Text = m_CsvSource != null ? m_CsvSource.ToCsvText(true) : "(keine Datei geladen)";
                    textBoxContent.ReadOnly = true;
                    buttonSave.Enabled = false;
                    break;
                case 1:
                    textBoxContent.Text = m_CsvDefinition.SourceText;
                    textBoxContent.ReadOnly = false;
                    buttonSave.Enabled = true;
                    break;
                case 2:
                    textBoxContent.Text = m_CsvDefinition.ControlData.ToCsvText(true);
                    textBoxContent.ReadOnly = true;
                    buttonSave.Enabled = false;
                    break;
                case 3:
                    if ((m_CsvSource != null) && (m_CsvDefinition != null))
                    {
                        Csv2Gaeb90_Konverter Konverter = new Csv2Gaeb90_Konverter(m_CsvSource, m_CsvDefinition.ControlData);
                        textBoxContent.Text = Konverter.GetResult();
                        m_DP = Konverter.DP;
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
            string bsTempName = Tools.AskOpenFileName(m_CsvFileName, "csv");
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

            CsvHandler csv = new CsvHandler();
            if (!csv.Attach(CsvHandler.ExcelCsvToInternalCsv(bsTempText)))
            {
                MessageBox.Show("Die Datei wurde nicht als csv-Format erkannt.");
                return;
            }

            m_CsvFileName = bsTempName;
            m_CsvSource = csv;
            this.Text = m_CsvFileName;
            comboBoxView.SelectedIndex = 0;
            UpdateControls();
        }

        private void buttonOpenDefinition_Click(object sender, EventArgs e)
        {
            string bsTempName = Tools.AskOpenFileName(m_CsvDefinition.FileName, "ini");
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

            m_CsvDefinition.FileName = bsTempName;
            m_CsvDefinition.SourceText = bsTempText;
            comboBoxView.SelectedIndex = 1;
            UpdateControls();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            switch (nViewMode)
            {
                case 1: // Definition
                    SaveDefinition(textBoxContent.Text);
                    break;
                case 3: // GAEB-Inhalt
                    SaveGaeb(textBoxContent.Text);
                    break;
            }
        }

        private void SaveGaeb(string bsContent)
        {
            string bsExtension = String.IsNullOrEmpty(m_DP) ? "d82" : "d" + m_DP;
            string bsName = m_CsvFileName.Substring(0, m_CsvFileName.LastIndexOf('.')) + "." + bsExtension;

            bsName = Tools.AskSaveFileName(bsName, bsExtension);
            if (!String.IsNullOrEmpty(bsName))
            {
                System.IO.File.WriteAllText(bsName, bsContent, Encoding.GetEncoding(858));
            }
        }

        private void SaveDefinition(string bsContent)
        {
            string bsExtension = "ini";
            string bsName = m_CsvDefinition.FileName;

            bsName = Tools.AskSaveFileName(bsName, bsExtension);
            if (!String.IsNullOrEmpty(bsName))
            {
                System.IO.File.WriteAllText(bsName, bsContent, Encoding.Default);
            }
        }

        private void textBoxContent_TextChanged(object sender, EventArgs e)
        {
            if (nViewMode == 1)
            {
                m_CsvDefinition.SourceText = textBoxContent.Text;
            }
        }

        private void textBoxDefintionName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
