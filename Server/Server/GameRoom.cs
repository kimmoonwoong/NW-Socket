using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class GameRoom
    {
        List<ClientSession> _sessions = new List<ClientSession>();
        object _lock = new object();
        public void Enter(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Add(session);
                session.room = this;
            }
        }

        public void Lever(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session);
            }
        }

        public void Brodcast(ClientSession clientSession, string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerId = clientSession.sessionId;
            packet.chat = $"{chat} I am {packet.playerId}";
            ArraySegment<byte> segment = packet.Write();
            lock (_lock)
            {
                foreach(ClientSession session in _sessions)
                {
                    session.Send(segment);
                }
            }
        }
    }
}
