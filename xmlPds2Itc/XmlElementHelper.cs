using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace xmlPds2Itc
{
    class XmlElementHelper
    {
        XmlElement e;
        public XmlElementHelper(XmlNode n)
        {
            e = (XmlElement)n;
        }

        public XmlElement Element { get { return e; } }

        public void SetElement(string bsName, string bsValue)
        {
            var elemList = e.GetElementsByTagName(bsName);
            if ((elemList != null) && (elemList.Count > 0))
            {
                elemList[0].InnerText = bsValue;
            }
            else
            {
                var newElem = e.OwnerDocument.CreateElement(bsName);
                newElem.InnerText = bsValue;
                e.AppendChild(newElem);
            }
        }

        public string GetElementText(string bsName)
        {
            var elemList = e.GetElementsByTagName(bsName);
            return (elemList != null) && (elemList.Count > 0) ? elemList[0].InnerText : "";
        }

        public void SetAttribute(string bsName, string bsValue)
        {
            e.SetAttribute(bsName, bsValue);
        }

        public XmlElementHelper AddElement(string bsName)
        {
            var newElem = e.OwnerDocument.CreateElement(bsName);
            e.AppendChild(newElem);
            return new XmlElementHelper(newElem);
        }

        public static XmlElementHelper InsertBefore(XmlElement elementAfter, string bsName)
        {
            XmlElement newElem = elementAfter.OwnerDocument.CreateElement(bsName);
            ((XmlElement)(elementAfter.ParentNode)).InsertBefore(newElem, elementAfter);
            return new XmlElementHelper(newElem);
        }

        public static XmlElementHelper InsertAfter(XmlElement elementBefore, string bsName)
        {
            XmlElement newElem = elementBefore.OwnerDocument.CreateElement(bsName);
            ((XmlElement)(elementBefore.ParentNode)).InsertAfter(newElem, elementBefore);
            return new XmlElementHelper(newElem);
        }

        public static bool SaveDocument(XmlDocument xDoc, string bsFileName)
        {
            try
            {
                XmlTextWriter xtw = new XmlTextWriter(bsFileName, Encoding.UTF8);
                xtw.Formatting = Formatting.Indented;
                xDoc.WriteTo(xtw);
                xtw.Flush();
                xtw.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
