using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Launch
{
    public class GameRunner : IRunner
    {
        private bool _isRunning;
        private int _id;
        public GameObject GO { get; private set; }
        public SingletonContainer STContainer { get; private set; }
        public GameRunner(int id)
        {
            _isRunning = false;
            this._id = id;
        }

        public int GetID()
        {
            return _id ;
        }

        public virtual void Run(params object[] param)
        {
            if (_isRunning)
            {
                CLog.LogError("GameRunner,id="+ _id +" is running,can not run again!");
                return;
            }
            _isRunning = true;
            GameCfg gameCfg = GameConfig.GetGameCfg(this._id);
            if (GO == null)
            {
                GO = new GameObject(gameCfg.name);
                GameObject.DontDestroyOnLoad(GO);
            }
            Transform uiTrans = AppManager.Instance.GameStarter.transform.FindChild(gameCfg.ui+"_Template");
            if (uiTrans != null)
            {
                GameObject ui = GameObject.Instantiate(uiTrans.gameObject);
                CTLTools.AddChildToParent(ui, GO,false);
                ui.transform.Find("UICamera").GetComponent<Camera>().depth = gameCfg.uiDepth ;
                ui.SetActive(true);
                ui.name = "UI";
            }
            else
            {
                CLog.LogError("can not find UI template in GameStarter!UI:"+gameCfg.ui);
            }

            STContainer = GO.AddComponent<SingletonContainer>();
            STContainer.ResourceMgr.Init(GameConfig.IsResourceLoadMode,string.Format("Assets/{0}/Res",gameCfg.rootDir));
            STContainer.LuaClient.BindGame(gameCfg.id);
            STContainer.LuaClient.StartGame();

        }

        public virtual void Stop()
        {
            if (GO != null)
            {
                GameObject.Destroy(GO);
                GO = null;
            }
            _isRunning = false;
        }
    }
}
