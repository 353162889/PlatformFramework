using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Launch
{
    public interface IRunner
    {
        int GetID();
        void Run(params object[] param);
        void Stop();
    }

    public class AppManager : Singleton<AppManager>
    {
        public GameObject GameStarter { get; private set; }
        private Dictionary<int, IRunner> _mapRunners;
        public AppManager()
        {
            _mapRunners = new Dictionary<int, IRunner>();
        }

        public void SetGameStarter(GameObject starter)
        {
            GameStarter = starter;
        }

        public void RegisterRunner(IRunner runner)
        {
            if(!_mapRunners.ContainsKey(runner.GetID()))
            {
                _mapRunners.Add(runner.GetID(), runner);
            }
            else
            {
                CLog.LogError("runnerID:"+ runner.GetID() + " has exist!");
            }
        }

        public void Run(int id,params object[] param)
        {
            IRunner runner;
            _mapRunners.TryGetValue(id, out runner);
            if(runner != null)
            {
                runner.Run(param);   
            }
            else
            {
                CLog.LogError("[run]can not find runnerID:" + id);
            }
        }

        public void Stop(int id)
        {
            IRunner runner;
            _mapRunners.TryGetValue(id, out runner);
            if (runner != null)
            {
                runner.Stop();
            }
            else
            {
                CLog.LogError("[stop]can not find runnerID:" + id);
            }
        }

        public IRunner GetRunner(int id)
        {
            IRunner runner;
            if(_mapRunners.TryGetValue(id,out runner))
            {
                return runner;
            }
            return null;
        }
    }
}