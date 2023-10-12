using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
namespace ServerCore
{
    class Session
    {
        Socket socket;
        int _disconnected = 0;
        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
        Queue<byte[]> sendQueue = new Queue<byte[]>();
        List<ArraySegment<byte>> pendinglist = new List<ArraySegment<byte>>();

        object _lock = new object();
        public void init(Socket _socket)
        {
            socket = _socket;
            SocketAsyncEventArgs recyArgs = new SocketAsyncEventArgs();
            recyArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecyComplited);
            
            recyArgs.SetBuffer(new byte[1024], 0, 1024);
            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendComplit);

            RegisterRecy(recyArgs);

        }

        public void DisConnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
        #region 네트워크 Recevy
        void RegisterRecy(SocketAsyncEventArgs args)
        {
            bool pending = socket.ReceiveAsync(args);
            if (!pending) OnRecyComplited(null, args);

        }
        void OnRecyComplited(object sent, SocketAsyncEventArgs args)
        {
            if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    string recData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine("From Client : " + recData);
                    RegisterRecy(args);
                }
                catch(Exception e) 
                {
                    Console.WriteLine("Recy Error" + e.ToString());

                }
            }
            else
            {
                DisConnect();
            }
        }
        #endregion

        #region 네트워크 Send
        public void Send(byte[] data)
        {
            lock (_lock)
            {
                sendQueue.Enqueue(data);
                if (pendinglist.Count() == 0)
                {
                    RegisterSend();
                }
            }
        }


        void RegisterSend()
        {
            while(sendQueue.Count() > 0)
            {
                byte[] buff = sendQueue.Dequeue();
                pendinglist.Add(new ArraySegment<byte>(buff, 0, buff.Length));
            }
            sendArgs.BufferList = pendinglist;
            bool pending = socket.SendAsync(sendArgs);
            if (!pending)
                OnSendComplit(null, sendArgs);


        }
        void OnSendComplit(object send, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    sendArgs.BufferList = null;
                    pendinglist.Clear();

                    Console.WriteLine($"Transferred bytes: {sendArgs.BytesTransferred}");

                    if(sendQueue.Count > 0)
                        RegisterSend();
                    
                }
                else
                {
                    DisConnect();
                }
            }
        }
        #endregion
    }
}
