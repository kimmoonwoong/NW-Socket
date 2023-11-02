using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using ServerCore;
namespace Server
{

    class Knight
    {
        public int hp;
        public int attack;
    }

    class GameChatingSession : Session
    {
        static int cnt = 0;
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}, Count : {cnt}");


            Knight knight = new Knight() { hp = 100, attack = 10 };


            ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            byte[] buffer = BitConverter.GetBytes(knight.hp);
            byte[] buffer2 = BitConverter.GetBytes(knight.attack);

            Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);

            ArraySegment<byte> sendbuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);


            Send(sendbuff);

            Thread.Sleep(1000);

            DisConnect();
            cnt++;

        }

        public override void OnDisConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisConnected : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count); // 받은 메세지를 출력
            Console.WriteLine("From Client : " + recData); // 
            return buffer.Count;
        }

        public override void OnSend(int numOfBuffer)
        {
            Console.WriteLine($"Transferred bytes: {numOfBuffer}");
        }
    }

    class GameInventorySession : Session
    {
        static int cnt = 0;
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}, Count : {cnt}");

            
            Knight knight = new Knight() { hp = 100, attack = 10 };


            ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            byte[] buffer = BitConverter.GetBytes(knight.hp);
            byte[] buffer2 = BitConverter.GetBytes(knight.attack);

            Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);

            ArraySegment<byte> sendbuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);
            

            Send(sendbuff);

            Thread.Sleep(1000);

            DisConnect();
            cnt++;

        }

        public override void OnDisConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisConnected : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count); // 받은 메세지를 출력
            Console.WriteLine("From Client : " + recData); // 
            return buffer.Count;
        }

        public override void OnSend(int numOfBuffer)
        {
            Console.WriteLine($"Transferred bytes: {numOfBuffer}");
        }
    }

    class Program
    {
        static Listener chainglistener = new Listener();
        static Listener inventorylistener = new Listener();
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry iPHost = Dns.GetHostEntry(host);
            IPAddress iPAddress = iPHost.AddressList[0];
            
            IPEndPoint chatingendPoint = new IPEndPoint(iPAddress, 7777);
            
            IPEndPoint inventoryendPoint = new IPEndPoint(iPAddress, 5555);
            // 비동기적으로 처리

            chainglistener.init(chatingendPoint, () => { return new GameChatingSession(); });
            inventorylistener.init(inventoryendPoint, () => { return new GameInventorySession(); });
            Console.WriteLine("Listening...");
            while (true)
            {

            }
        }
    }
}