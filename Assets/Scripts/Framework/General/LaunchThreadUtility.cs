using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Launch
{
    public class ThreadAsync
    {
        public readonly Thread Thread;
        public bool IsDone { get; private set; }
        public string Error { get; private set; }
        public ThreadAsync(Thread thread)
        {
            this.Thread = thread;
            this.IsDone = false;
            this.Error = null;
        }

        public void SetIsDone(bool isDone)
        {
            this.IsDone = isDone;
        }

        public void SetError(string error)
        {
            this.Error = error;
        }
    }

    public class LaunchThreadUtility : Singleton<LaunchThreadUtility>
    {
        public delegate string ThreadAction(object obj);

        public ThreadAsync DoSomething(ThreadAction action,object obj = null)
        {
            ThreadAsync async = null;
            Thread thread = new Thread(()=> {
                if (action != null)
                {
                    ThreadAction temp = action;
                    action = null;
                    string error = temp.Invoke(obj);
                    async.SetError(error);
                    async.SetIsDone(true);
                }
            });
            async = new ThreadAsync(thread);
            thread.Start();
            return async;
        }
    }
}
