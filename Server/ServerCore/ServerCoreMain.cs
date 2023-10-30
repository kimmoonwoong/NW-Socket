using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace ServerCore
{
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            byte[] sendbuff = Encoding.UTF8.GetBytes("Welcome TO MMORPT Server");

            //ss
            Send(sendbuff);

            Thread.Sleep(1000);

            DisConnect();

        }

        public override void OnDisConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisConnected : {endPoint}");
        }

        public override void OnRecv(ArraySegment<byte> buffer)
        {
            string recData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count); // 받은 메세지를 출력
            Console.WriteLine("From Client : " + recData); // 
        }

        public override void OnSend(int numOfBuffer)
        {
            Console.WriteLine($"Transferred bytes: {numOfBuffer}");
        }
    }

    class ServerCoreMain
    {
        static Listener listener = new Listener();
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry iPHost = Dns.GetHostEntry(host);
            IPAddress iPAddress = iPHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(iPAddress, 7777);
            // 비동기적으로 처리

            listener.init(endPoint, () => { return new GameSession(); });
            Console.WriteLine("Listening...");
            while (true)
            {
                    
            }
        }
    }
}