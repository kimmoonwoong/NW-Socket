using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace DummyClient
{
    class ClientMain
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry iPHost = Dns.GetHostEntry(host);
            IPAddress iPAddress = iPHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(iPAddress, 7777);



            while (true)
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    //입장 문의
                    socket.Connect(endPoint);

                    Console.WriteLine("Connected to" + socket.RemoteEndPoint.ToString());
                    for (int i = 0; i < 5; i++)
                    {
                        byte[] sendbuff = Encoding.UTF8.GetBytes($"Hello World! {i}\n");
                        int sendbyte = socket.Send(sendbuff);
                    }
                    byte[] recvbuff = new byte[1024];
                    int recvbyte = socket.Receive(recvbuff);
                    string recvData = Encoding.UTF8.GetString(recvbuff, 0, recvbyte);
                    Console.WriteLine("From Server" + recvData);
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Thread.Sleep(1000);
            }
        }
    }
}