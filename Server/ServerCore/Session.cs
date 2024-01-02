using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace ServerCore
{
    public abstract class PacketSession: Session
    {
        public static readonly int HeadSize = 2;
        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            int processLen = 0;

            while (true)
            {
                // 최소한 헤더는 파싱 할 수 있어야 한다.
                if (buffer.Count < HeadSize) break;

                ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);

                if (buffer.Count < size) break;

                // 패킷 재조립
                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, size));

                processLen += size;
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + size, buffer.Count - size);

            }
            return processLen;
        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }

    public abstract class Session
    {
        Socket socket;
        int _disconnected = 0;
        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs(); // Event(메서드) 실행(Action과도 같음)
        Queue<ArraySegment<byte>> sendQueue = new Queue<ArraySegment<byte>>(); // Register에 누군가 들어가 있으면 queue에 쌓아 놓았다가 한번에 보냄
        List<ArraySegment<byte>> pendinglist = new List<ArraySegment<byte>>(); // 메세지 모음
        SocketAsyncEventArgs recyArgs = new SocketAsyncEventArgs();

        RecvBuffer recvBuffer = new RecvBuffer(1024);

        object _lock = new object(); // 멀티 쓰레드 환경에서 동기화 문제를 해결하기 위해 사용


        public abstract void OnConnected(EndPoint endPoint);// 연결 됐을 때
        public abstract int OnRecv(ArraySegment<byte> buffer);  // 성공적으로 받았을 때
        public abstract void OnSend(int numOfBuffer); // 성공적으로 보냈을 때
        public abstract void OnDisConnected(EndPoint endPoint);
        public void init(Socket _socket)
        {
            socket = _socket;
            recyArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecyComplited); // Receivy가 성공적으로 끝나면 OnRecyComplited 메서드를 실행            
            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendComplit); // Send가 성공적으로 끝나면 OnSendComplit 메서드 실행

            RegisterRecy(recyArgs); // 받는 크기가 정해져있고, 그 순간마다 달라지는 것은 없기에 실행시켜도 문제 없음

        }

        public void Clear()
        {
            lock (_lock)
            {
                pendinglist.Clear();
                sendQueue.Clear();
            }
        }
        public void DisConnect() // 연결 해제
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1) // 여러 사용자가 동시다발적으로 disconnect를 할 수 있기에 해제 중이면 return시킴
                return;


            OnDisConnected(socket.RemoteEndPoint);
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            Clear();
        }
        #region 네트워크 Recevy
        void RegisterRecy(SocketAsyncEventArgs args)
        {
            if (_disconnected == 1) return;

            recvBuffer.Clean();
            ArraySegment<byte> segment = recvBuffer.RecvSegment;
            recyArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            try
            {
                bool pending = socket.ReceiveAsync(args); // 비동기적으로 Recive
                if (!pending) OnRecyComplited(null, args); // 만약 바로 들어갈 수 있다면 컴플리트 실행. 안 되더라도 위에서 Complit이 되면 Action되게 설정
            }
            catch(Exception e)
            {
                Console.WriteLine($"Recv Failed {e.ToString}");
            }
        }
        void OnRecyComplited(object sent, SocketAsyncEventArgs args)
        {
            if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success) // 성공적으로 끝났고, byte가 0이 아니면(byte가 0인 경우는 유저가 의도적으로 서버를 공격할 때를 의미) 
            {
                try
                {
                    //Write 커서 이동
                    if(recvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        DisConnect();
                        return;
                    }

                    //컨텐츠 쪽으로 데이터를 넘기고 얼마나 처리했는지 받는다.
                    

                    int processLen = OnRecv(recvBuffer.DataSegment);
                    if(processLen < 0 || processLen > recvBuffer.DataSize)
                    {
                        DisConnect();
                        return;
                    }

                    //Read 커서 이동
                    if(recvBuffer.OnRead(processLen) == false)
                    {
                        DisConnect();
                        return;
                    }


                    RegisterRecy(args);
                }
                catch(Exception e) 
                {
                    Console.WriteLine("Recy Error" + e.ToString());

                }
            }
            else
            {
                DisConnect(); // 오류가 있는 상황이기에 DisConnect(연결 해제)
            }
        }
        #endregion

        #region 네트워크 Send
        public void Send(ArraySegment<byte> data)
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
            if (_disconnected == 1)
                return;

            while(sendQueue.Count() > 0)
            {
                ArraySegment<byte> buff = sendQueue.Dequeue();
                pendinglist.Add(buff);
            }
            sendArgs.BufferList = pendinglist;

            try
            {
                bool pending = socket.SendAsync(sendArgs);
                if (!pending)
                    OnSendComplit(null, sendArgs);
            }
            catch(Exception e)
            {
                Console.WriteLine($"Send Failed {e.ToString}");
            }

        }
        void OnSendComplit(object send, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    sendArgs.BufferList = null;
                    pendinglist.Clear();

                    OnSend(sendArgs.BytesTransferred);

                    
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
