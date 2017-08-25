using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Launch
{
    public static class CTLSchedule
    {
        public static bool AddScheduler(int gameId, SchedulerHandler handler, float delay, int times = 0)
        {
            GameRunner runner;
            if(TryGameRunner(gameId, out runner))
            {
                return runner.STContainer.Scheduler.AddScheduler(handler, delay, times);
            }
            return false;
        }

        public static void RemoveScheduler(int gameId,SchedulerHandler handler)
        {
            GameRunner runner;
            if (TryGameRunner(gameId, out runner))
            {
                runner.STContainer.Scheduler.RemoveScheduler(handler);
            }
        }

        private static bool TryGameRunner(int gameId, out GameRunner gameRunner)
        {
            return CTLTools.TryGameRunner(gameId, out gameRunner);
        }
    }
}
