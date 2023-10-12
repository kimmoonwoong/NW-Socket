using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace ServerCore
{
    class ServerCoreMain
    {
        static Listener listener = new Listener();
        static void OnAcceptHandler(Socket clintsocket)
        {
            try
            {
                byte[] sendbuff = Encoding.UTF8.GetBytes("Welcome TO MMORPT Server");
                Session session = new Session();
                session.init(clintsocket);

                session.Send(sendbuff);

                Thread.Sleep(1000);

                session.DisConnect();
                session.DisConnect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry iPHost = Dns.GetHostEntry(host);
            IPAddress iPAddress = iPHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(iPAddress, 7777);
            // 비동기적으로 처리

            listener.init(endPoint, OnAcceptHandler);
            Console.WriteLine("Listening...");
            while (true)
            {

            }
        }
    }
}