using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using ServerCore;
namespace Server
{


    class Program
    {

        static Listener chainglistener = new Listener();
        static Listener inventorylistener = new Listener();
        public static GameRoom Room = new GameRoom();
        static void Main(string[] args)
        {
            
            string host = Dns.GetHostName();
            IPHostEntry iPHost = Dns.GetHostEntry(host);
            IPAddress iPAddress = iPHost.AddressList[0];
            
            IPEndPoint chatingendPoint = new IPEndPoint(iPAddress, 7777);
            
            IPEndPoint inventoryendPoint = new IPEndPoint(iPAddress, 5555);
            // 비동기적으로 처리

            chainglistener.init(chatingendPoint, () => { return SessionManager.Instance.Generator(); });
            //inventorylistener.init(inventoryendPoint, () => { return new GameInventorySession(); });
            Console.WriteLine("Listening...");
            while (true)
            {

            }
        }
    }
}