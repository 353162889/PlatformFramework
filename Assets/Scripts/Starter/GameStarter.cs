using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace Launch
{
    public class GameStarter : MonoBehaviour
    {
        private void Awake()
        {
            GameObject.DontDestroyOnLoad(gameObject);
            AppManager.Instance.SetGameStarter(gameObject);
            if(Debug.isDebugBuild)
            {
                gameObject.AddComponent<ConsoleLogger>();
            }
            GameConfig.Load();
            List<GameCfg> games = GameConfig.GetGames();
            for (int i = 0; i < games.Count; i++)
            {
                GameCfg cfg = games[i];
                Type type = Type.GetType(cfg.runClass);
                IRunner runner = (IRunner)Activator.CreateInstance(type, cfg.id);
                AppManager.Instance.RegisterRunner(runner);
            }
            AppManager.Instance.Run(1);
            //AppManager.Instance.Run(2);
        }
    }
}