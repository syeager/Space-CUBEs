// Steve Yeager
// 

using System.Xml;
using System.IO;

public static class XMLTextWriterExtension
{
    public static void WriteElementStringLine(this XmlTextWriter writer, string localName, string value)
    {
        writer.WriteElementString(localName, value);
        writer.WriteWhitespace("\r\n");
    }


    public static void WriteStartElementLine(this XmlTextWriter writer, string localName)
    {
        writer.WriteStartElement(localName);
        writer.WriteWhitespace("\r\n");
    }


    public static void WriteEndElementLine(this XmlTextWriter writer)
    {
        writer.WriteEndElement();
        writer.WriteWhitespace("\r\n");
    }


    public static void WriteStartDocumentLine(this XmlTextWriter writer)
    {
        writer.WriteStartDocument();
        writer.WriteWhitespace("\r\n");
    }
}