using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace xmlPds2Itc
{
    public partial class XmlPdsMainForm : Form
    {
        string bsPdsFileName = null;
        string bsItcFileName = null;
        XmlDocument itcDocument = null;

        public XmlPdsMainForm()
        {
            InitializeComponent();
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "xml-Dateien (*.xml)|*.xml|alle Dateien (*.*)|*.*";
            ofd.FileName = bsPdsFileName;
            ofd.CheckFileExists = true;
            ofd.DereferenceLinks = true;
            ofd.Multiselect = false;
            ofd.Title = "Datei öffnen...";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.PreserveWhitespace = true;
                try
                {
                    xDoc.Load(ofd.FileName);
                }
                catch (Exception) 
                {
                    itcDocument = null;
                    textBoxInfo.Text = "Kein Dokument geladen.";
                    return;
                }

                bsPdsFileName = ofd.FileName;
                var Konverter = new PdsKonverter();
                itcDocument = Konverter.ConvertPds(xDoc);
                textBoxInfo.Text = Konverter.Messages.ToString();
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (bsPdsFileName == null)
            {
                MessageBox.Show("Es wurde noch kein Dokument geladen.");
                return;
            }
            if (itcDocument == null)
            {
                MessageBox.Show("Es liegt kein konvertiertes Dokument vor.");
                return;
            }

            bsItcFileName = bsPdsFileName.Substring(0, bsPdsFileName.LastIndexOf('.')) + "_itcrafts.xml";

            SaveFileDialog ofd = new SaveFileDialog();
            ofd.Filter = "xml-Dateien (*.xml)|*.xml|alle Dateien (*.*)|*.*";
            ofd.FileName = bsItcFileName;
            ofd.OverwritePrompt = true;
            ofd.Title = "Datei speichern...";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                bsItcFileName = ofd.FileName;
                if (XmlElementHelper.SaveDocument(itcDocument, bsItcFileName))
                    MessageBox.Show("Das Dokument wurde gesichert.");
                else
                    MessageBox.Show("Fehler beim Sichern des Dokuments.");
            }
        }
    }
}
