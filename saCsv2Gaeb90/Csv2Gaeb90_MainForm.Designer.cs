namespace saCsv2Gaeb90
{
    partial class Csv2Gaeb90_MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxDefintionName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxView = new System.Windows.Forms.ComboBox();
            this.textBoxContent = new System.Windows.Forms.TextBox();
            this.buttonOpenDefinition = new System.Windows.Forms.Button();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Definition";
            // 
            // textBoxDefintionName
            // 
            this.textBoxDefintionName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDefintionName.Location = new System.Drawing.Point(69, 7);
            this.textBoxDefintionName.Name = "textBoxDefintionName";
            this.textBoxDefintionName.ReadOnly = true;
            this.textBoxDefintionName.Size = new System.Drawing.Size(251, 20);
            this.textBoxDefintionName.TabIndex = 1;
            this.textBoxDefintionName.TextChanged += new System.EventHandler(this.textBoxDefintionName_TextChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(416, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Ansicht";
            // 
            // comboBoxView
            // 
            this.comboBoxView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxView.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxView.FormattingEnabled = true;
            this.comboBoxView.Location = new System.Drawing.Point(464, 7);
            this.comboBoxView.Name = "comboBoxView";
            this.comboBoxView.Size = new System.Drawing.Size(119, 21);
            this.comboBoxView.TabIndex = 3;
            this.comboBoxView.SelectedIndexChanged += new System.EventHandler(this.comboBoxView_SelectedIndexChanged);
            // 
            // textBoxContent
            // 
            this.textBoxContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxContent.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxContent.Location = new System.Drawing.Point(12, 34);
            this.textBoxContent.Multiline = true;
            this.textBoxContent.Name = "textBoxContent";
            this.textBoxContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxContent.Size = new System.Drawing.Size(571, 277);
            this.textBoxContent.TabIndex = 4;
            this.textBoxContent.WordWrap = false;
            this.textBoxContent.TextChanged += new System.EventHandler(this.textBoxContent_TextChanged);
            // 
            // buttonOpenDefinition
            // 
            this.buttonOpenDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOpenDefinition.Location = new System.Drawing.Point(326, 8);
            this.buttonOpenDefinition.Name = "buttonOpenDefinition";
            this.buttonOpenDefinition.Size = new System.Drawing.Size(71, 20);
            this.buttonOpenDefinition.TabIndex = 5;
            this.buttonOpenDefinition.Text = "auwählen";
            this.buttonOpenDefinition.UseVisualStyleBackColor = true;
            this.buttonOpenDefinition.Click += new System.EventHandler(this.buttonOpenDefinition_Click);
            // 
            // buttonOpen
            // 
            this.buttonOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOpen.Location = new System.Drawing.Point(427, 323);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(75, 23);
            this.buttonOpen.TabIndex = 6;
            this.buttonOpen.Text = "csv öffnen";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Location = new System.Drawing.Point(508, 323);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 7;
            this.buttonSave.Text = "speichern";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // Csv2Gaeb90_MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 358);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonOpen);
            this.Controls.Add(this.buttonOpenDefinition);
            this.Controls.Add(this.textBoxContent);
            this.Controls.Add(this.comboBoxView);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxDefintionName);
            this.Controls.Add(this.label1);
            this.Name = "Csv2Gaeb90_MainForm";
            this.Text = "Csv-Datei in GAEB90 konvertieren";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxDefintionName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxView;
        private System.Windows.Forms.TextBox textBoxContent;
        private System.Windows.Forms.Button buttonOpenDefinition;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.Button buttonSave;
    }
}

