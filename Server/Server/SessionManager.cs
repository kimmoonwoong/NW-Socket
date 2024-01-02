using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class SessionManager
    {
        public static SessionManager _sessionmanager = new SessionManager();

        public static SessionManager Instance { get { return _sessionmanager; } }

        object _lock = new object();
        int _sessionId = 0;
        Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
        public ClientSession Generator()
        {
            lock (_lock)
            {
                ClientSession session = new ClientSession();
                session.sessionId = ++_sessionId;
                _sessions.Add(_sessionId, session);
                return session;
            }
        }

        public ClientSession Find(int Id)
        {
            lock (_lock)
            {
                ClientSession session = null;
                _sessions.TryGetValue(Id, out session);
                return session;
            }
        }

        public void Remove(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session.sessionId);
            }
        }
    }
}
