using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore;
namespace Server
{
    class GameRoom : IJobQueue
    {

        List<ClientSession> _sessions = new List<ClientSession>();
        
        JobQueue _jobqueue = new JobQueue();
        public void Push(Action job)
        {
            _jobqueue.Push(job);
        }
        public void Enter(ClientSession session)
        {
            _sessions.Add(session);
            session.room = this;
        }

        public void Lever(ClientSession session)
        {
            _sessions.Remove(session);
            
        }
        /*
         * Brodcast하는 부분이 동시 접속자가 많을수록 lock으로 멀티 쓰레드를 관리하면
         * 그만큼 늦어지는 단점이 있다.
         * 그래서 job queue를 생각해봐야 한다.
         * 
         */
        public void Brodcast(ClientSession clientSession, string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerId = clientSession.sessionId;
            packet.chat = $"{chat} I am {packet.playerId}";
            ArraySegment<byte> segment = packet.Write();
            foreach(ClientSession session in _sessions)
            {
                session.Send(segment);
            }
        }
    }
}
