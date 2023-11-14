using System;
using System.Xml;

namespace PacketGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true,
            };


            using (XmlReader r = XmlReader.Create("PDL.xml", settings))
            {
                r.MoveToContent();

                while (r.Read())
                {
                    if(r.Depth == 1 && r.NodeType == XmlNodeType.Element)
                    {
                        ParsePacket(r);
                    }
                    Console.WriteLine(r.Name + " " + r["name"]);
                }
            }
        }
        public static void ParsePacket(XmlReader r)
        {
            if (r.NodeType == XmlNodeType.EndElement) return;

            if (r.Name.ToLower() != "packet") return;

            string packetname = r["name"];
            if (packetname == null) return;

            ParsMembers(r);
        }
        
        public static void ParsMembers(XmlReader r)
        {
            string packetname = r["name"];
            int depth = r.Depth + 1;
            while (r.Read()) {
                if (r.Depth != depth) break;

                string memberName = r["name"];
                if(memberName == null) return;

                string memerType = r.Name.ToLower();
                switch(memberName)
                {
                    case "bool":
                    case "byte":
                    case "int":
                    case "short":
                    case "ushort":
                    case "long":
                    case "string":
                    case "float":
                    case "double":
                    case "list":
                        break;
                    default: break;
                }
            }
        }
    }
}