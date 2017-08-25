using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Launch
{
    public static class CTLResource
    {
        public static void GetResource(int gameId,string path, ResourceMgr.ResourceHandler callback)
        {
            GameRunner runner;
            if(TryGameRunner(gameId,out runner))
            {
                runner.STContainer.ResourceMgr.GetResource(path, callback);
            }
        }

        public static void ReleaseUnUseRes(int gameId)
        {
            GameRunner runner;
            if (TryGameRunner(gameId, out runner))
            {
                runner.STContainer.ResourceMgr.ReleaseUnUseRes();
            }
        }

        public static void RemoveListener(int gameId, string path, ResourceMgr.ResourceHandler callback)
        {
            GameRunner runner;
            if (TryGameRunner(gameId, out runner))
            {
                runner.STContainer.ResourceMgr.RemoveListener(path,callback);
            }
        }

        public static void RemoveAllListener(int gameId, string path)
        {
            GameRunner runner;
            if (TryGameRunner(gameId, out runner))
            {
                runner.STContainer.ResourceMgr.RemoveAllListener(path);
            }
        }

        private static bool TryGameRunner(int gameId,out GameRunner gameRunner)
        {
            return CTLTools.TryGameRunner(gameId, out gameRunner);
        }
    }
}
