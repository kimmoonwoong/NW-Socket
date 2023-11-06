using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
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

    class PlayerinfoOk: Packet
    {
        public int hp;
        public int attack;
    }

    public enum PacketID
    {
        PlayerinfoReq = 1,
        PlayerinfoOk = 2,
    }
    class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");



            PlayerinfoReq packet = new PlayerinfoReq() {packetId = (ushort)PacketID.PlayerinfoReq, playerId = 1001 };


            ArraySegment<byte> s = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;

            count += 2;

            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.packetId);
            count += 2;

            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), packet.playerId);
            count += 8;

            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count);


            ArraySegment<byte> sendbuff = SendBufferHelper.Close(count);
            
            if(success)
                Send(sendbuff);

        }

        public override void OnDisConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisConnected : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count); // 받은 메세지를 출력
            Console.WriteLine("From Server : " + recData); // 
            return buffer.Count;
        }

        public override void OnSend(int numOfBuffer)
        {
            Console.WriteLine($"Transferred bytes: {numOfBuffer}");
        }
    }
}
