using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Server
{


    using ServerCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Text;

    public enum PacketID
    {

        PlayerInfoReq = 0,

        Test = 1,

    }


    class PlayerInfoReq
    {
        public byte Testbyte;
        public long playerId;
        public string name;
        public class Skile
        {
            public int id;
            public short level;
            public float duration;
            public class Attribute
            {
                public int att;
                public bool Write(Span<byte> s, ref ushort count)
                {
                    bool success = true;

                    success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), att);
                    count += sizeof(int);

                    return success;
                }

                public void Read(ReadOnlySpan<byte> s, ref ushort count)
                {

                    this.att = BitConverter.ToInt32(s.Slice(count, s.Length - count));
                    count += sizeof(int);

                }
            }

            public List<Attribute> attributes = new List<Attribute>();

            public bool Write(Span<byte> s, ref ushort count)
            {
                bool success = true;

                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), id);
                count += sizeof(int);


                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), level);
                count += sizeof(short);


                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), duration);
                count += sizeof(float);


                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.attributes.Count);
                count += sizeof(ushort);
                foreach (Attribute attribute in this.attributes)
                    success &= attribute.Write(s, ref count);

                return success;
            }

            public void Read(ReadOnlySpan<byte> s, ref ushort count)
            {

                this.id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
                count += sizeof(int);


                this.level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
                count += sizeof(short);


                this.duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
                count += sizeof(float);


                this.attributes.Clear();
                ushort attributeLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
                count += sizeof(ushort);
                for (int i = 0; i < attributeLen; i++)
                {
                    Attribute attribute = new Attribute();
                    attribute.Read(s, ref count);
                    attributes.Add(attribute);
                }

            }
        }

        public List<Skile> skiles = new List<Skile>();


        /*
            * 모든 Packet에서 쓰는 것, 읽는 것 모두 동일하게 작업하므로 클래스의 메서드화
            */
        public ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;

            Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            count += (ushort)Marshal.SizeOf(count);
            /*
                * 여기서 Array.Copy가 아닌 TryWriteBytes를 쓰는 이유
                * Array.Copy를 사용하면 new를 통해 새로운 객체를 만들어서 복사하는 작업을 함
                * 하지만 TryWriteByte는 span 클래스, 즉 어떤 array에 직접 쓰는 것으로 작업을 한다.
                * 그렇기에 TryWriteByte가 더 효율적이다.
                */
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.PlayerInfoReq);
            count += sizeof(ushort);


            segment[segment.Offset + count] = this.Testbyte;
            count += sizeof(byte);


            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), playerId);
            count += sizeof(long);


            ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
            count += nameLen;
            count += sizeof(ushort);



            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.skiles.Count);
            count += sizeof(ushort);
            foreach (Skile skile in this.skiles)
                success &= skile.Write(s, ref count);


            success &= BitConverter.TryWriteBytes(s, count);


            if (!success) return null;

            return SendBufferHelper.Close(count);

        }
        public void Read(ArraySegment<byte> buffer)
        {
            ushort count = 0;

            ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(buffer.Array, buffer.Offset, buffer.Count); ;

            count += sizeof(ushort);
            count += sizeof(ushort);

            this.Testbyte = buffer.Array[buffer.Offset + count];
            count += sizeof(byte);


            this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
            count += sizeof(long);


            ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);
            this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
            count += nameLen;


            this.skiles.Clear();
            ushort skileLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);
            for (int i = 0; i < skileLen; i++)
            {
                Skile skile = new Skile();
                skile.Read(s, ref count);
                skiles.Add(skile);
            }

        }
    }

    class ClientSession : PacketSession
    {
        static int cnt = 0;
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}, Count : {cnt}");


            //Packet packet = new Packet() { size = 100, packetId = 10 };


            /*ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            byte[] buffer = BitConverter.GetBytes(packet.size);
            byte[] buffer2 = BitConverter.GetBytes(packet.packetId);

            Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);

            ArraySegment<byte> sendbuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);


            Send(sendbuff);
*/
            Thread.Sleep(5000);

            DisConnect();
            cnt++;

        }
        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort count = 0;

            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += 2;
            ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;

            switch ((PacketID)packetId)
            {
                case PacketID.PlayerInfoReq:

                    PlayerInfoReq p = new PlayerInfoReq();
                    p.Read(buffer);
                    Console.WriteLine($"PlayerId : {p.playerId}, PlayerName : {p.name}");
                    
                    foreach(PlayerInfoReq.Skile skile in p.skiles)
                    {
                        Console.WriteLine($"ID : {skile.id}, Level : {skile.level}, duration : {skile.duration}");
                    }
                    break;
            }
            Console.WriteLine($"PacketSize : {size}, PacketId : {packetId}");

        }


        public override void OnDisConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisConnected : {endPoint}");
        }

        public override void OnSend(int numOfBuffer)
        {
            Console.WriteLine($"Transferred bytes: {numOfBuffer}");
        }
    }
}
