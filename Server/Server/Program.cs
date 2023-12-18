using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using ServerCore;
namespace Server
{

    

    /*class GameInventorySession : PacketSession
    {
        static int cnt = 0;
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}, Count : {cnt}");

            
            *//*Packet packet = new Packet() { size = 100, packetId = 10 };


            ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            byte[] buffer = BitConverter.GetBytes(packet.size);
            byte[] buffer2 = BitConverter.GetBytes(packet.packetId);

            Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);

            ArraySegment<byte> sendbuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);
            

            Send(sendbuff);
*//*
            Thread.Sleep(5000);

            DisConnect();
            cnt++;

        }
        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + HeadSize);

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
    }*/

    class Program
    {

        static Listener chainglistener = new Listener();
        static Listener inventorylistener = new Listener();
        static void Main(string[] args)
        {
            PacketManager.Instance.Register();

            string host = Dns.GetHostName();
            IPHostEntry iPHost = Dns.GetHostEntry(host);
            IPAddress iPAddress = iPHost.AddressList[0];
            
            IPEndPoint chatingendPoint = new IPEndPoint(iPAddress, 7777);
            
            IPEndPoint inventoryendPoint = new IPEndPoint(iPAddress, 5555);
            // 비동기적으로 처리

            chainglistener.init(chatingendPoint, () => { return new ClientSession(); });
            //inventorylistener.init(inventoryendPoint, () => { return new GameInventorySession(); });
            Console.WriteLine("Listening...");
            while (true)
            {

            }
        }
    }
}