using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Launch
{
    public class CLog
    {
        public static LogRecorder logRecorder = new LogRecorder();
        public static void Log(string msg, string color = "#ffffff")
        {
            if (GameConfig.IsDebugInfo)
            {
                StringBuilder sb = new StringBuilder();
                if (string.IsNullOrEmpty(color))
                {
                    sb.Append(msg.ToString());
                }
                else
                {
                    sb.Append("<color=" + color + ">");
                    sb.Append(msg.ToString());
                    sb.Append("</color>");
                }

                Debug.Log(sb.ToString());
                logRecorder.Log(msg);
            }
        }

        public static void LogError(object msg)
        {
            if (GameConfig.IsDebugError)
            {
                UnityEngine.Debug.LogError(msg);
                logRecorder.LogError(msg);
            }
        }

        public static void LogWarn(object msg)
        {
            if (GameConfig.IsDebugWarn)
            {
                UnityEngine.Debug.LogWarning(msg);
                logRecorder.LogWarn(msg);
            }
        }
    }

    public class CLogColor
    {
        public static string Yellow = "#ff7f00";
        public static string Red = "#ff0000";
    }
}
