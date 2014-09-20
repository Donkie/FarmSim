using System.Xml;

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

    public static void Read(XmlReader xml)
    {
        while (xml.Read())
        {
            if (xml.NodeType != XmlNodeType.Whitespace)
                break;
        }
    }
}