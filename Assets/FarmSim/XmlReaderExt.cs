using System.Xml;

namespace Assets.FarmSim
{
    public class XmlReaderExt
    {
        public static bool ReadToNextElement(XmlReader xml)
        {
            while (xml.Read())
            {
                if (xml.NodeType == XmlNodeType.Element)
                    return true;
            }

            return false;
        }

        public static bool SafeCheckEndElement(XmlReader xml)
        {
            return xml.NodeType != XmlNodeType.EndElement && !xml.EOF;
        }

        public static void Read(XmlReader xml)
        {
            /*while (xml.Read())
            {
                if (xml.NodeType != XmlNodeType.Whitespace)
                    break;
            }*/
            xml.Read();
            xml.MoveToContent();
        }
    }
}
