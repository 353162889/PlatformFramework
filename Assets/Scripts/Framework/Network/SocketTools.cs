using System;
using System.Collections;
using UnityEngine;

namespace Launch
{
    public class SocketTools
    {
        public static void Log(string msg)
        {
            CLog.Log(msg);
        }

        public static void LogWarn(string msg)
        {
            CLog.LogWarn(msg);
        }

        public static void LogError(string msg)
        {
            CLog.LogError(msg);
        }
    }
}
