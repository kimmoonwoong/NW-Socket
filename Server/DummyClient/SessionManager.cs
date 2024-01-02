using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
{
    class SessionManager
    {
        static SessionManager _sessionmanager = new SessionManager();

        public static SessionManager Instance { get { return _sessionmanager; } }

        List<ServerSession> _sessions = new List<ServerSession>();
        object _lock = new object();

        public void SendForEach()
        {
            lock (_lock)
            {
                foreach (ServerSession session in _sessions)
                {
                    C_Chat packet = new C_Chat();
                    packet.chat = $"Hello Server !!";
                    ArraySegment<byte> segment = packet.Write();

                    session.Send(segment);
                }
            }
        }
        public ServerSession Generator()
        {
            lock (_lock)
            {
                ServerSession session = new ServerSession();
                _sessions.Add(session);

                return session;
            }
        }

    }
}
