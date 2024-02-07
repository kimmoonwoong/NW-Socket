using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    struct JobTimerElem : IComparable<JobTimerElem>
    {
        public int exeTick;
        public Action action;
        public int CompareTo(JobTimerElem other)
        {
            return other.exeTick - this.exeTick; 
        }
    }
    class JobTimer
    {
        PriorityQueue<JobTimerElem> pq = new PriorityQueue<JobTimerElem> ();
        object _lock = new object();
        public static JobTimer Instance { get; } = new JobTimer();

        public void Push(Action action, int tickAffter= 0)
        {
            JobTimerElem job;
            job.exeTick = System.Environment.TickCount + tickAffter;
            job.action = action;

            lock (_lock)
            {
                pq.Push(job);
            }
        }

        public void Flush()
        {
            while (true)
            {
                int now = System.Environment.TickCount;
                JobTimerElem job;

                lock (_lock)
                {
                    if (pq.Count == 0) break;

                    if (pq.Peek().exeTick <= now)
                    {
                        pq.Peek().action.Invoke();
                        pq.Pop();
                    }
                    else break;
                }
            }
        }
    }
}
