using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
namespace DummyClient
{



    using ServerCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Text;

    class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");



            C_PlayerInfoReq packet = new C_PlayerInfoReq() { playerId = 1001, name = "ABCD" };
            var skile = new C_PlayerInfoReq.Skile() { id = 101, level = 1, duration = 3.2f };
            skile.attributes.Add(new C_PlayerInfoReq.Skile.Attribute() { att = 77 });
            packet.skiles.Add(skile);
            packet.skiles.Add(new C_PlayerInfoReq.Skile() { id = 102, level = 2, duration = 2.5f });
            packet.skiles.Add(new C_PlayerInfoReq.Skile() { id = 103, level = 3, duration = 3.1f });
            packet.skiles.Add(new C_PlayerInfoReq.Skile() { id = 104, level = 4, duration = 1.5f });
            ArraySegment<byte> sendbuff = packet.Write();

            if (sendbuff != null)
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
