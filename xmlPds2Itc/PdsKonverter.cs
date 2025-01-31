using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace xmlPds2Itc
{
    class PdsKonverter
    {
        int m_LastRefKey;
        public StringBuilder Messages { get; private set; }

        public XmlDocument ConvertPds(XmlDocument pdsDoc)
        {
            Messages = new StringBuilder();

            XmlDocument itcDoc = new XmlDocument();
            itcDoc.PreserveWhitespace = true;
            try
            {
                itcDoc.Load(GetMainModulePath() + "xmlPds2Itc-template.xml");
            }
            catch (Exception e1)
            {
                Messages.AppendLine("ConvertPds: " + e1.Message);
                return null;
            }

            XmlElement pdsRoot = pdsDoc.DocumentElement;
            XmlElement itcRoot = itcDoc.DocumentElement;
            if (pdsRoot.LocalName != "Export")
            {
                Messages.AppendLine("Unknown XmlRoot: " + pdsRoot.LocalName);
                return null;
            }

            m_LastRefKey = 100;
            if (convertRoot(itcRoot, pdsRoot))
            {
                Messages.AppendLine("Konvertierung beendet.");
                return itcDoc;
            }

            Messages.AppendLine("Die Konvertierung ist fehlgeschlagen.");
            return null;
        }

        bool convertRoot(XmlElement itcRoot, XmlElement pdsRoot)
        {
            bool result = false;

            for (XmlNode e = pdsRoot.FirstChild; e != null; e = e.NextSibling)
            {
                if (e.NodeType == XmlNodeType.Element)
                    switch (e.LocalName)
                    {
                        case "Version":
                        case "Release":
                            break;
                        case "Angebot":
                            if (! result) result = convertBeleg(itcRoot, (XmlElement)e, " 8", "Angebot");
                            break;
                        case "Rechnung":
                            if (! result) result = convertBeleg(itcRoot, (XmlElement)e, " 1", "Rechnung");
                            break;
                        case "Gutschrift":
                            if (!result) result = convertBeleg(itcRoot, (XmlElement)e, " 2", "Gutschrift");
                            break;
						
						case "Auftrag":
						if (!result) result = convertBeleg(itcRoot, (XmlElement)e, " 7", "Auftrag");
							break;
						
						default:
                            HandleUnknownTag(e.LocalName, "convertRoot");
                            break;
                    }
            }
            return result;
        }

        bool convertBeleg(XmlElement itcRoot, XmlElement pdsAng, string bsBelegArt, string bsHeader)
        {
            bool result = false;
            string t;

            XmlElementHelper itcVorlauf = new XmlElementHelper(itcRoot.GetElementsByTagName("Vorlauf")[0]);
            XmlElementHelper itcKunde = new XmlElementHelper(itcRoot.GetElementsByTagName("Kunde")[0]);

            XmlElement itcPositionenTag = (XmlElement)(itcRoot.GetElementsByTagName("Positionen")[0]);
            XmlElement itcRootEbene = (XmlElement)(itcPositionenTag.GetElementsByTagName("Ebene")[0]);
            XmlElementHelper itcLVRoot = new XmlElementHelper(itcRootEbene.GetElementsByTagName("LVRoot")[0]);
            XmlElementHelper itcMehrwertsteuer = new XmlElementHelper(itcRootEbene.GetElementsByTagName("Mehrwertsteuer")[0]);

            for (XmlNode e = pdsAng.FirstChild; e != null; e = e.NextSibling)
            {
                if (e.NodeType == XmlNodeType.Element)
                    switch (e.LocalName)
                    {
                        case "Nummer":
                            itcRoot.SetAttribute("BELEGART", bsBelegArt);
                            itcRoot.SetAttribute("BELEGNR", KeyStrR(e.InnerText,8));
                            itcRoot.SetAttribute("NavDocNo", e.InnerText);
                            itcVorlauf.SetElement("BELARTTEXT", bsHeader);
                            result = true;
                            break;
                        case "VorgangStatus":
                            break;
                        case "Bezeichnung":
                            itcVorlauf.SetElement("BETREFF1", e.InnerText);
                            itcKunde.SetElement("DestDebNr", e.InnerText);
                            break;
                        case "Kundennummer":
                            itcVorlauf.SetElement("KDNR", e.InnerText);
                            break;
                        case "Anschrift":
                            convertAnschrift(itcVorlauf, itcKunde, (XmlElement)e);
                            break;
                        case "Sachbearbeiter":
                            break;
                        case "Erstelldatum":
                            t = e.InnerText;
                            itcRoot.SetAttribute("Date", t.Substring(8, 2) + "." + t.Substring(5, 2) + "." + t.Substring(0, 4));
                            itcVorlauf.SetElement("TAG", t.Substring(8, 2));
                            itcVorlauf.SetElement("MONAT", t.Substring(5, 2));
                            itcVorlauf.SetElement("JAHR", t.Substring(0, 4));
                            break;
                        case "LetzteAenderungDatum":
                            break;
                        case "Schema":
                            convertSchema(itcLVRoot, itcMehrwertsteuer, (XmlElement)e);
                            break;
                        case "KostenAnteilListe":
                            break;
                        case "EbenenListe":
                            convertEbenenListe(itcLVRoot, 1, "", (XmlElement)e);
                            break;
                        case "PositionenListe":
                            XmlElementHelper itcEbene = itcLVRoot.AddElement("Ebene");
                            itcEbene.SetAttribute("Tiefe", "1");
                            convertPositionenListe(itcEbene, 2, "", (XmlElement)e);
                            break;
                        case "Leistungsdatum":
                            t = e.InnerText;
                            itcVorlauf.SetElement("BAUFERTIG", t.Substring(8, 2) + "." + t.Substring(5, 2) + "." + t.Substring(0, 4));
                            break;
                        // case "ZahlungsbedingungenListe":
                        // case "AbweichendeRechnungsAnschrift":
                        case "Selektionskriterien":
                        case "DmsReferenzHolderID":
                        case "TextDokumentEinstellungen":
                        case "MatchingEinstellungen":   // PDS GUI-Einstellung
                        //case "Leistungsempfaenger":
                        case "Abzuege":
                        //case "NummernVergabe":
                        case "StandardVorgangsArbeitsGruppe":
                        case "LagerZuordnungTyp":
                        case "Geschaeftspartner":
                        case "Valutadatum":
                        case "Periode":
                        case "TeilrechnungsAbzugBeruecksichtigung":
                            break;
                        default:
                            HandleUnknownTag(e.LocalName, "convertBeleg");
                            break;
                    }
            }
            return result;
        }

        void convertAnschrift(XmlElementHelper itcVorlauf, XmlElementHelper itcKunde, XmlElement pdsAnschrift)
        {
            for (XmlNode e = pdsAnschrift.FirstChild; e != null; e = e.NextSibling)
            {
                if (e.NodeType == XmlNodeType.Element)
                    switch (e.LocalName)
                    {
                        case "Name":
                            itcVorlauf.SetElement("NAME1", e.InnerText);
                            itcKunde.SetElement("Name1", e.InnerText);
                            break;
                        case "Strasse":
                            itcVorlauf.SetElement("STRASSE", e.InnerText);
                            itcKunde.SetElement("Strasse", e.InnerText);
                            break;
                        case "Ort":
                            itcVorlauf.SetElement("ORT", e.InnerText);
                            itcKunde.SetElement("Ort", e.InnerText);
                            break;
                        case "Postleitzahl":
                            itcVorlauf.SetElement("PLZ", e.InnerText);
                            itcKunde.SetElement("Plz", e.InnerText);
                            break;
                    }
            }
        }

        void convertSchema(XmlElementHelper itcLVRoot, XmlElementHelper itcMehrwertsteuer, XmlElement pdsSchema)
        {
            XmlElementHelper textPos;

            for (XmlNode e = pdsSchema.FirstChild; e != null; e = e.NextSibling)
            {
                if (e.NodeType == XmlNodeType.Element)
                    switch (e.LocalName)
                    {
                        // Texte vor Root-Position
                        case "TextBetreff":
                        case "TextAnschreiben":
                        case "TextTechnVorbemerkungen":
                            if (e.InnerText != "")
                            {
                                textPos = XmlElementHelper.InsertBefore(itcLVRoot.Element, "Textposition");
                                textPos.Element.SetAttribute("RefKey", (++m_LastRefKey).ToString().PadLeft(6, '0'));
                                textPos.SetElement("KTXT1", GetFirstTextLine(e.InnerText));
                                textPos.SetElement("KZDRK1", "1");
                                textPos.SetElement("TextAng", e.InnerText);
                            }
                            break;
                        // Texte nach Mehrwertsteuer
                        case "TextSchlusstext":
                            if (e.InnerText != "")
                            {
                                textPos = XmlElementHelper.InsertAfter(itcMehrwertsteuer.Element, "Textposition");
                                textPos.Element.SetAttribute("RefKey", (++m_LastRefKey).ToString().PadLeft(6, '0'));
                                textPos.SetElement("KTXT1", GetFirstTextLine(e.InnerText));
                                textPos.SetElement("KZDRK1", "1");
                                textPos.SetElement("TextAng", e.InnerText);
                            }
                            break;
                    }
            }
        }

        void convertEbenenListe(XmlElementHelper itcParent, int nTiefe, string ParentPosNr, XmlElement pdsListe)
        {
            XmlElementHelper itcEbene = itcParent.AddElement("Ebene");
            itcEbene.SetAttribute("Tiefe", nTiefe.ToString());

            for (XmlNode e = pdsListe.FirstChild; e != null; e = e.NextSibling)
            {
                if (e.NodeType == XmlNodeType.Element)
                    switch (e.LocalName)
                    {
                        case "Ebene":
                            convertTitel(itcEbene, nTiefe + 1, ParentPosNr, (XmlElement)e);
                            break;
                    }
            }
        }

        void convertTitel(XmlElementHelper itcEbene, int NaechsteTiefe, string ParentPosNr, XmlElement pdsEbene)
        {
            XmlElementHelper itcTitel = itcEbene.AddElement("Titel");
            itcTitel.Element.SetAttribute("RefKey", (++m_LastRefKey).ToString().PadLeft(6, '0'));
            itcTitel.SetElement("VK2JN", "0");
            itcTitel.SetElement("LOHNJN", "1");
            itcTitel.SetElement("MATJN", "1");
            itcTitel.SetElement("POSNR", ParentPosNr);
            itcTitel.SetElement("KTXT1", "");
            itcTitel.SetElement("MENGE", "1");
            //itcTitel.SetElement("MITTELLOHN", "1");
            itcTitel.SetElement("AUFWAND", "1");
            itcTitel.SetElement("PE", "1");
            itcTitel.SetElement("GEBINDE", "1");
            itcTitel.SetElement("KZDRK1", "1");
            itcTitel.SetElement("FAKTOR1", "1");
            itcTitel.SetElement("FAKTOR2", "1");
            itcTitel.SetElement("FAKTOR3", "1");
            
            string PosNr = ParentPosNr;
            XmlElementHelper itcNaechsteEbene = null;

            for (XmlNode e = pdsEbene.FirstChild; e != null; e = e.NextSibling)
            {
                if (e.NodeType == XmlNodeType.Element)
                    switch (e.LocalName)
                    {
                        case "Nummer":
                            if (PosNr != "") PosNr += ".";
                            PosNr += e.InnerText;
                            itcTitel.SetElement("POSNR", PosNr);
                            break;
                        case "Bezeichnung":
                            itcTitel.SetElement("KTXT1", e.InnerText);
                            break;
                        case "Art":
                            break;
                        case "Ebene":
                            if (itcNaechsteEbene == null)
                            {
                                itcNaechsteEbene = itcTitel.AddElement("Ebene");
                                itcNaechsteEbene.SetAttribute("Tiefe", NaechsteTiefe.ToString());
                            }
                            convertTitel(itcNaechsteEbene, NaechsteTiefe + 1, PosNr, (XmlElement)e);
                            break;
                        case "PositionenListe":
                            if (itcNaechsteEbene == null)
                            {
                                itcNaechsteEbene = itcTitel.AddElement("Ebene");
                                itcNaechsteEbene.SetAttribute("Tiefe", NaechsteTiefe.ToString());
                            }
                            convertPositionenListe(itcNaechsteEbene, NaechsteTiefe + 1, PosNr, (XmlElement)e);
                            break;
                        // bekannte Elemente, die ignoriert werden koennen
                        case "ID":
                        case "SchwierigkeitsFaktorVorgabe":
                        case "SchwierigkeitsFaktorRechnerisch":
                            break;
                        // unbekannte Elemente
                        default:
                            HandleUnknownTag(e.LocalName, "convertTitel");
                            break;
                    }
            }
        }

        void convertPositionenListe(XmlElementHelper itcParent, int NaechsteTiefe, string ParentPosNr, XmlElement pdsListe)
        {
            for (XmlNode e = pdsListe.FirstChild; e != null; e = e.NextSibling)
            {
                if (e.NodeType == XmlNodeType.Element)
                    switch (e.LocalName)
                    {
                        case "Position":
                            convertPosition(itcParent, NaechsteTiefe, ParentPosNr, (XmlElement)e);
                            break;
                        default:
                            HandleUnknownTag(e.LocalName, "convertPositionenListe");
                            break;
                    }
            }
        }

        void convertPosition(XmlElementHelper itcParent, int NaechsteTiefe, string ParentPosNr, XmlElement pdsPosition)
        {
            string pdsTyp = (new XmlElementHelper(pdsPosition)).GetElementText("Typ");
            switch (pdsTyp)
            {
                case "MATERIAL":
                case "MATERIAL_INKL_LOHN":
                    convertPosition_Normal(itcParent, NaechsteTiefe, ParentPosNr, pdsPosition);
                    break;
                case "ZUSCHLAG":
                    convertPosition_Zuschlag(itcParent, NaechsteTiefe, ParentPosNr, pdsPosition);
                    break;
                case "ZUSAMMENGESETZT":
                    convertPosition_Paket(itcParent, NaechsteTiefe, ParentPosNr, pdsPosition);
                    break;
                case "TEXT":
                    convertPosition_Text(itcParent, NaechsteTiefe, ParentPosNr, pdsPosition);
                    break;
                default:
                    HandleUnknownTag("Typ==" + pdsTyp, "convertPosition");
                    break;
            }
        }

        void convertPosition_Normal(XmlElementHelper itcParent, int NaechsteTiefe, string ParentPosNr, XmlElement pdsPosition)
        {
            string pdsArt = (new XmlElementHelper(pdsPosition)).GetElementText("Art");
            switch (pdsArt)
            {
                case "NORMAL":
                case "ALTERNATIV":
                case "EVENTUAL_MITGESAMTSUMME":
                    break;
                default:
                    HandleUnknownTag("Art==" + pdsArt, "convertPosition_Normal");
                    break;
            }

            XmlElementHelper itcPosition = itcParent.AddElement("Position");
            itcPosition.Element.SetAttribute("RefKey", (++m_LastRefKey).ToString().PadLeft(6, '0'));
            itcPosition.SetElement("BEDARF", "0");
            itcPosition.SetElement("ALTERNATIV", pdsArt == "ALTERNATIV" ? "1" : "0");
            itcPosition.SetElement("ENTFAELLT", "0");
            itcPosition.SetElement("VK2JN", "1");
            itcPosition.SetElement("LOHNJN", "1");
            itcPosition.SetElement("MATJN", "1");
            itcPosition.SetElement("POSNR", ParentPosNr);
            itcPosition.SetElement("KTXT1", "");
            itcPosition.SetElement("MENGE", "1");
            itcPosition.SetElement("MITTELLOHN", "1");
            itcPosition.SetElement("AUFWAND", "1");
            itcPosition.SetElement("ME", "");
            itcPosition.SetElement("PE", "1");
            itcPosition.SetElement("GEBINDE", "1");
            itcPosition.SetElement("KZDRK1", "1");
            itcPosition.SetElement("VK2LOHN", "0");
            itcPosition.SetElement("VK2MAT", "0");
            itcPosition.SetElement("FAKTOR1", "1");
            itcPosition.SetElement("FAKTOR2", "1");
            itcPosition.SetElement("FAKTOR3", "1");
            itcPosition.SetElement("TextAng", "");

            for (XmlNode e = pdsPosition.FirstChild; e != null; e = e.NextSibling)
            {
                if (e.NodeType == XmlNodeType.Element)
                    switch (e.LocalName)
                    {
                        case "Nummer":
                            itcPosition.SetElement("POSNR", e.InnerText);
                            break;
                        case "Masseinheit":
                            itcPosition.SetElement("ME", e.InnerText);
                            break;
                        case "Menge":
                            itcPosition.SetElement("MENGE", e.InnerText);
                            break;
                        case "Kurztext":
                            itcPosition.SetElement("KTXT1", GetFirstTextLine(e.InnerText));
                            break;
                        case "Langtext":
                            itcPosition.SetElement("TextAng", e.InnerText);
                            break;
                        case "VkAngebotspreis":
                            itcPosition.SetElement("VK2MAT", ((XmlElement)e).GetAttribute("EinzelPreis"));
                            break;
                        case "EkNettopreis":
                        case "Deckungsbeitraege":
                        case "Preiseinheit":
                        case "Dimensionsfaktor":
                        case "Leistungsfaktor":
                        case "Name":
                        case "Typ":
                        case "Art":
                        case "Fixpreis":
                        case "Pauschalmenge":
                        case "VerbraucherKennzeichen":
                        case "GaebImportMenge":
                        case "WahrscheinlicheMenge":
                        case "Arbeitsgruppe":
                        case "Lohnzeit":
                        case "LohnzeitEinfach":
                        case "SchwierigkeitsFaktor":
                        case "ZuschlagID":
                        case "ID":
                        case "Favorit":
                        case "Berechnungsart":
                        case "Festmenge":
                        case "BearbeitungsGrad":
                        case "Bearbeitung":
                        case "FestpreisTyp":
                        case "MehrwertsteuerBezeichnung":
                        case "MehrwertsteuerProzent":
                        case "TextDruckArt":
                            break;
                        default:
                            HandleUnknownTag(e.LocalName, "convertPosition_Normal");
                            break;
                    }
            }
        }

        void convertPosition_Paket(XmlElementHelper itcParent, int NaechsteTiefe, string ParentPosNr, XmlElement pdsPosition)
        {
            convertPosition_Normal(itcParent, NaechsteTiefe, ParentPosNr, pdsPosition);
        }

        void convertPosition_Text(XmlElementHelper itcParent, int NaechsteTiefe, string ParentPosNr, XmlElement pdsPosition)
        {
            HandleUnknownTag("", "convertPosition_Text");
            /*
                        XmlElementHelper itcPosition = itcParent.AddElement("Position");
                        itcPosition.Element.SetAttribute("RefKey", (++m_LastRefKey).ToString().PadLeft(6, '0'));
                        itcPosition.SetElement("VK2JN", "0");
                        itcPosition.SetElement("LOHNJN", "1");
                        itcPosition.SetElement("MATJN", "1");
                        itcPosition.SetElement("POSNR", ParentPosNr);
                        itcPosition.SetElement("KTXT1", "");
                        itcPosition.SetElement("MENGE", "1");
                        itcPosition.SetElement("MITTELLOHN", "1");
                        itcPosition.SetElement("AUFWAND", "1");
                        itcPosition.SetElement("ME", "");
                        itcPosition.SetElement("PE", "1");
                        itcPosition.SetElement("GEBINDE", "1");
                        itcPosition.SetElement("KZDRK1", "1");
                        itcPosition.SetElement("FAKTOR1", "1");
                        itcPosition.SetElement("FAKTOR2", "1");
                        itcPosition.SetElement("FAKTOR3", "1");
                        itcPosition.SetElement("TextAng", "");

                        for (XmlNode e = pdsPosition.FirstChild; e != null; e = e.NextSibling)
                        {
                            if (e.NodeType == XmlNodeType.Element)
                                switch (e.LocalName)
                                {
                                    case "Nummer":
                                        itcPosition.SetElement("POSNR", e.InnerText);
                                        break;
                                    case "Masseinheit":
                                        itcPosition.SetElement("MENGE", e.InnerText);
                                        break;
                                    case "Menge":
                                        itcPosition.SetElement("ME", e.InnerText);
                                        break;
                                    case "Kurztext":
                                        itcPosition.SetElement("KTXT1", GetFirstTextLine(e.InnerText));
                                        break;
                                    case "Langtext":
                                        itcPosition.SetElement("TextAng", e.InnerText);
                                        break;
                                    case "Art":
                                    case "Fixpreis":
                                    case "Typ":
                                    case "Pauschalmenge":
                                    default:
                                        //HandleUnknownTag(e.LocalName, "convertPosition_Text");
                                        break;
                                }
                        }
             */
        }

        void convertPosition_Zuschlag(XmlElementHelper itcParent, int NaechsteTiefe, string ParentPosNr, XmlElement pdsPosition)
        {
            XmlElementHelper itcPosition = itcParent.AddElement("Position");
            itcPosition.Element.SetAttribute("RefKey", (++m_LastRefKey).ToString().PadLeft(6, '0'));
            itcPosition.SetElement("VK2JN", "1");
            itcPosition.SetElement("LOHNJN", "1");
            itcPosition.SetElement("MATJN", "1");
            itcPosition.SetElement("POSNR", ParentPosNr);
            itcPosition.SetElement("KTXT1", "");
            itcPosition.SetElement("MENGE", "1");
            itcPosition.SetElement("MITTELLOHN", "1");
            itcPosition.SetElement("AUFWAND", "1");
            itcPosition.SetElement("ME", "");
            itcPosition.SetElement("PE", "1");
            itcPosition.SetElement("GEBINDE", "1");
            itcPosition.SetElement("KZDRK1", "1");
            itcPosition.SetElement("FAKTOR1", "1");
            itcPosition.SetElement("FAKTOR2", "1");
            itcPosition.SetElement("FAKTOR3", "1");
            itcPosition.SetElement("TextAng", "");

            for (XmlNode e = pdsPosition.FirstChild; e != null; e = e.NextSibling)
            {
                if (e.NodeType == XmlNodeType.Element)
                    switch (e.LocalName)
                    {
                        case "Nummer":
                            itcPosition.SetElement("POSNR", e.InnerText);
                            break;
                        case "Menge":
                            itcPosition.SetElement("MENGE", e.InnerText);
                            itcPosition.SetElement("GEBINDE", "100");
                            itcPosition.SetElement("ME", "%");
                            break;
                        case "Kurztext":
                            itcPosition.SetElement("KTXT1", GetFirstTextLine(e.InnerText));
                            break;
                        case "Langtext":
                            itcPosition.SetElement("TextAng", e.InnerText);
                            break;
                        case "VkAngebotspreis":
                            itcPosition.SetElement("VK2MAT", ((XmlElement)e).GetAttribute("EinzelPreis"));
                            break;
                        /*
                        case "Masseinheit":
                        case "EkNettopreis":
                        case "Deckungsbeitraege":
                        case "Preiseinheit":
                        case "Leistungsfaktor":
                        case "Name":
                        case "Typ":
                        case "Art":
                        case "Fixpreis":
                        case "Pauschalmenge":
                        case "VerbraucherKennzeichen":
                        case "GaebImportMenge":
                        case "WahrscheinlicheMenge":
                        case "Arbeitsgruppe":
                        case "Lohnzeit":
                        case "LohnzeitEinfach":
                        case "SchwierigkeitsFaktor":
                        case "ZuschlagID":
                        case "ID":
                        case "Favorit":
                        case "Berechnungsart":
                        case "Festmenge":
                        case "BearbeitungsGrad":
                        case "Bearbeitung":
                        case "FestpreisTyp":
                        case "MehrwertsteuerBezeichnung":
                        case "MehrwertsteuerProzent":
                        case "TextDruckArt":
                            break;
                        default:
                            HandleUnknownTag(e.LocalName, "convertPosition_Zuschlag");
                            break;
                         */
                    }
            }
        }

        void HandleUnknownTag(string tagName, string functionName)
        {
            Messages.AppendLine("Unbekanntes Element " + tagName + " in Funktion " + functionName); 
        }

        static string GetMainModulePath()
        {
            System.Diagnostics.Process p = System.Diagnostics.Process.GetCurrentProcess();
            string bsPath = System.IO.Path.GetDirectoryName(p.MainModule.FileName);
            if (bsPath.Substring(bsPath.Length - 1) != "\\")
                bsPath += "\\";
            return bsPath;
        }

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

        public static string DoubleToCoreString(double dValParam, string bsDecimalSep = ",")
        {
            return dValParam.ToString("0.######").Replace(System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator, bsDecimalSep);
        }

        public static string KeyStrR(string s, int nLen)
        {
            int l = s.Length;
            if (l < nLen)
                return s.PadLeft(nLen).ToUpperInvariant();
            else
                return s.Substring(l - nLen).ToUpperInvariant();
        }

        public static string GetFirstTextLine(string s)
        {
            while ((s.Length > 0) && ((s[0] == '\r') || (s[0] == '\n')))
                s = s.Substring(1);
            if (s.IndexOf("\r") >= 0) s = s.Substring(0, s.IndexOf("\r"));
            if (s.IndexOf("\n") >= 0) s = s.Substring(0, s.IndexOf("\n"));
            return s;
        }
    }
}
