using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    /*
     * Connector를 만드는 이유
     * 분산서버를 만든다면 각각의 서버에서 다른 서버로 연결하기 위해선
     * Connector가 필요함
     */
    public class Connector
    {

        Func<Session> _sessionFactory;

        public void Connect(IPEndPoint endPoint, Func<Session> _sessionFactory) { 
            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnConnectComplit;
            args.RemoteEndPoint = endPoint;

            args.UserToken = socket;

            this._sessionFactory = _sessionFactory;

            RegisterConnect(args);
        }

        void RegisterConnect(SocketAsyncEventArgs args)
        {
            Socket socket =  args.UserToken as Socket;
            if(socket == null ) { return; }
            bool _pending = socket.ConnectAsync(args);

            if (!_pending) OnConnectComplit(null, args);
        }

        void OnConnectComplit(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                Session session = _sessionFactory.Invoke();
                session.init(args.ConnectSocket);
                session.OnConnected(args.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine($"OnConnectComplit Fail {args.SocketError}");
            }
        }
    }
}
