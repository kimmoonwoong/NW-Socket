using System;
using System.Xml;

namespace PacketGenerator
{
    class Program
    {
        static string genPackets;
        static ushort packetId;
        static string enumPackets;
        static void Main(string[] args)
        {
            string pdlPath = "../../../PDL.xml";

            if(args.Length >= 1)
                pdlPath = args[0];


            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true,
            };


            using (XmlReader r = XmlReader.Create(pdlPath, settings))
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

                 string fileText =  string.Format(PacketFormat.fileFormat, enumPackets, genPackets);

                File.WriteAllText("GenPackets.cs", fileText);
            }
        }
        public static void ParsePacket(XmlReader r)
        {
            if (r.NodeType == XmlNodeType.EndElement) return;

            if (r.Name.ToLower() != "packet") return;

            string packetname = r["name"];
            if (packetname == null) return;

            Tuple<string, string, string> t =  ParsMembers(r);
            genPackets += string.Format(PacketFormat.packetFormat, packetname, t.Item1, t.Item2, t.Item3);
            enumPackets += string.Format(PacketFormat.packetEnumFormat, packetname, packetId++);
        }



        //{1} : 멤버 변수
        //{2} : 멤버 변수 Read
        //{3} : 멤버 변수 Write
        public static Tuple<string, string, string> ParsMembers(XmlReader r)
        {
            string packetname = r["name"];
            int depth = r.Depth + 1;

            string membercode = "";
            string readcode = "";
            string writecode = "";
            while (r.Read()) {
                if (r.Depth != depth) break;

                string memberName = r["name"];
                if(memberName == null) return null;

                if (string.IsNullOrEmpty(membercode) == false)
                    membercode += Environment.NewLine;
                if (string.IsNullOrEmpty(readcode) == false)
                    readcode += Environment.NewLine;
                if (string.IsNullOrEmpty(writecode) == false)
                    writecode += Environment.NewLine;

                string memerType = r.Name.ToLower();
                switch(memerType)
                {
                    case "byte":
                    case "sbyte":
                        membercode += string.Format(PacketFormat.memerFormat, memerType, memberName);
                        readcode += string.Format(PacketFormat.readByteFormat, memberName, memerType);
                        writecode += string.Format(PacketFormat.writeByteFormat, memberName, memerType);
                        break;
                    case "bool":
                    case "int":
                    case "short":
                    case "ushort":
                    case "long":
                    case "float":
                    case "double":
                        membercode += string.Format(PacketFormat.memerFormat, memerType, memberName);
                        readcode += string.Format(PacketFormat.readFormat, memberName, ToMemeberType(memerType), memerType);
                        writecode += string.Format(PacketFormat.writeFormat, memberName, memerType);
                        break;
                    case "string":
                        membercode += string.Format(PacketFormat.memerFormat, memerType, memberName);
                        readcode += string.Format(PacketFormat.readStringFormat, memberName);
                        writecode += string.Format(PacketFormat.writeStringFormat, memberName);

                        break;
                    case "list":
                        Tuple<string, string, string> t = ParseList(r);
                        membercode += t.Item1;
                        readcode += t.Item2;
                        writecode += t.Item3;
                        break;
                    default: break;
                }
            }

            membercode = membercode.Replace("\n", "\n\t");
            readcode = readcode.Replace("\n", "\n\t\t");
            writecode = writecode.Replace("\n", "\n\t\t");
            return new Tuple<string, string, string>(membercode, readcode, writecode);
        }

        public static Tuple<string, string, string> ParseList(XmlReader r)
        {
            string listname = r["name"];
            if(string.IsNullOrEmpty(listname))
            {
                Console.WriteLine("List without name");
                return null;
            }

            Tuple<string, string, string> t = ParsMembers(r);

            string membercode = string.Format(PacketFormat.memberListFormat,
                FirstCharToUpper(listname), FirstCharToLower(listname),
                t.Item1, t.Item2, t.Item3);

            string readcode = string.Format(PacketFormat.readListFormat,
                FirstCharToUpper(listname),
                FirstCharToLower(listname));
            string writecode = string.Format(PacketFormat.writeListFormat,
                FirstCharToUpper(listname),
                FirstCharToLower(listname));

            return new Tuple<string , string, string>(membercode, readcode, writecode);

        }
        public static string ToMemeberType(string memberType)
        {
            switch(memberType)
            {
                case "bool":
                    return "ToBoolean";
                case "int":
                    return "ToInt32";
                case "short":
                    return "ToInt16";
                case "ushort":
                    return "ToUInt16";
                case "long":
                    return "ToInt64";
                case "float":
                    return "ToSingle";
                case "double":
                    return "ToDouble";
                    break;
                default:
                    return "";
                    break;
            }
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return input[0].ToString().ToUpper() + input.Substring(1);
        }
        public static string FirstCharToLower(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return input[0].ToString().ToLower() + input.Substring(1);
        }

    }
}