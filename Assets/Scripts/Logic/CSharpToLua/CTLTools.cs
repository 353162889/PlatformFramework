using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Launch
{
    public static class CTLTools
    {
        public static void Log(string msg, string color)
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

                CLog.Log(sb.ToString());
            }
        }
        public static void Log(string msg)
        {
            if (GameConfig.IsDebugInfo)
            {
                CLog.Log(msg);
            }
        }

        public static void LogWarn(string msg)
        {
            CLog.LogWarn(msg);
        }

        public static void LogError(string msg)
        {
            CLog.LogError(msg);
        }

        public static GameObject GetGameRoot(int gameId)
        {
            GameRunner runner;
            if(TryGameRunner(gameId, out runner))
            {
                return runner.GO;
            }
            return null;
        }

        public static GameObject GetUIRoot(int gameId)
        {
            GameObject gameStarter = GetGameRoot(gameId);
            Transform trans = gameStarter.transform.FindChild("UI");
            if(trans == null)
            {
                CLog.Log("can not find gameId:"+gameId+" UI");
            }
            return (trans == null ? null : trans.gameObject);
        }

        public static void AddChildToParent(GameObject child,GameObject parent,bool stayWorldPos = false)
        {
            if (child == null || parent == null)
            {
                CLog.LogError("AddChildToParent child or parent can not null!");
                return;
            }
            child.transform.parent = parent.transform;
            if(!stayWorldPos)
            {
                child.transform.localPosition = Vector3.zero;
                child.transform.localRotation = Quaternion.identity;
                child.transform.localScale = Vector3.one;
            }
        }

        public static void DestroyGameObject(GameObject go)
        {
            GameObject.Destroy(go);
        }

        public static bool TryGameRunner(int gameId, out GameRunner gameRunner)
        {
            gameRunner = null;
            IRunner runner = AppManager.Instance.GetRunner(gameId);
            if (runner is GameRunner)
            {
                gameRunner = (GameRunner)runner;
                return true;
            }
            else
            {
                CLog.LogError("gameId " + gameId + " is not GameRunner!");
                return false;
            }
        }
    }
}
