using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using ServerCore;

namespace DummyClient
{

    /*class GameInventorySession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");


            Packet packet = new Packet() { size = 4, packetId = 7 };


            ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            byte[] buffer = BitConverter.GetBytes(packet.size);
            byte[] buffer2 = BitConverter.GetBytes(packet.packetId);

            Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);

            ArraySegment<byte> sendbuff = SendBufferHelper.Close(packet.size);
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
    }*/

    class ClientMain
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry iPHost = Dns.GetHostEntry(host);
            IPAddress iPAddress = iPHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(iPAddress, 7777);
            IPEndPoint inventoryendPoint = new IPEndPoint(iPAddress, 5555);
            int cnt = 0;
            while (true)
            {
                try
                {
                    Connector connector = new Connector();
                    if (cnt % 2 == 0)
                    {
                        connector.Connect(endPoint, () => { return new ServerSession(); });
                    }
                    /*else
                    {
                        connector.Connect(inventoryendPoint, () => { return new GameInventorySession(); });
                    }*/
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                cnt++;
                Thread.Sleep(5000);
            }
        }
    }
}