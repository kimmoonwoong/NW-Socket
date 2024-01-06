using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class JobQueue : IJobQueue
    {
        Queue<Action> _jobqueue = new Queue<Action>();
        object _lock = new object();
        bool _flush = false;
        public void Push(Action job)
        {
            bool flush = false;
            lock (_lock)
            {
                _jobqueue.Enqueue(job);
                if (!_flush)
                {
                    _flush = flush = true;
                }
            }

            if(flush)
            {
                Flush();
            }
        }
        public void Flush()
        {
            while (true)
            {
                Action action = Pop();
                if (action == null) return;
                action.Invoke();
            }
        }

        Action Pop()
        {
            lock (_lock)
            {
                if (_jobqueue.Count > 0) return _jobqueue.Dequeue();
                else
                {
                    _flush = false;
                    return null;
                }
            }
        }
    }
}
