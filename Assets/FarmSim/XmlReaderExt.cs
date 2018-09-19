using System.Xml;

namespace Assets.FarmSim
{
    public static class XmlReaderExt
    {
        public static bool ReadToNextElement(this XmlReader xml)
        {
            while (xml.Read())
            {
                if (xml.NodeType == XmlNodeType.Element)
                    return true;
            }

            return false;
        }

        public static bool NotAtEnd(this XmlReader xml)
        {
            return xml.NodeType != XmlNodeType.EndElement && !xml.EOF;
        }

        public static void ReadAndMove(this XmlReader xml)
        {
            xml.Read();
            xml.MoveToContent();
        }
    }
}
