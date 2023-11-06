using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Packet
    {
        public ushort size;
        public ushort packetId;
    }

    class PlayerinfoReq : Packet
    {
        public long playerId;
    }

    class PlayerinfoOk : Packet
    {
        public int hp;
        public int attack;
    }

    public enum PacketID
    {
        PlayerinfoReq = 1,
        PlayerinfoOk = 2,
    }

    class ClientSession : PacketSession
    {
        static int cnt = 0;
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}, Count : {cnt}");


            Packet packet = new Packet() { size = 100, packetId = 10 };


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
                case PacketID.PlayerinfoReq:
                    long playerId = BitConverter.ToInt64(buffer.Array, buffer.Offset + count);
                    count += 8;
                    Console.WriteLine($"PlayerId : {playerId}");
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
