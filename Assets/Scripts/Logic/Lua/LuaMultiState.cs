using LuaInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Launch
{
    //使用此类是，必须定义宏  MULTI_STATE  （luaState的一些操作需要宏才会执行）
    public class LuaMultiState : LuaState
    {
        public LuaMultiState(LuaFileUtils luaFileUtils)
        {
#if !MULTI_STATE
            CLog.LogError("not define symbols MULTI_STATE!");
#endif
            this.luaFileUtils = luaFileUtils;
            if (mainState == null)
            {
                mainState = this;
            }

            LuaException.Init();
            L = LuaNewState();
            stateMap.Add(L, this);
            OpenToLuaLibs();
            ToLua.OpenLibs(L);
            OpenBaseLibs();
            LuaSetTop(0);
            InitLuaPath();
        }

        protected override void Init()
        {
           
        }

        public virtual void BindGame(int id)
        {
            //在Eidtor下时,添加lua的游戏路径(相对路径)
            ((LuaMultiLoader)this.luaFileUtils).BindGame(id);

#if UNITY_EDITOR
            GameCfg gameCfg = GameConfig.GetGameCfg(id);
            string path = string.Format("{0}/{1}/Lua", Application.dataPath, gameCfg.rootDir);
            if (!Directory.Exists(path))
            {
                string msg = string.Format("luaDir path not exists: {0}", path);
                throw new LuaException(msg);
            }
            AddSearchPath(path);
#endif
        }

        protected override void InitLuaPath()
        {
            InitPackagePath();

            if (!luaFileUtils.beZip)
            {
#if UNITY_EDITOR

                if (!Directory.Exists(LuaConst.luaDir))
                {
                    string msg = string.Format("luaDir path not exists: {0}, configer it in LuaConst.cs", LuaConst.luaDir);
                    throw new LuaException(msg);
                }

                if (!Directory.Exists(LuaConst.toluaDir))
                {
                    string msg = string.Format("toluaDir path not exists: {0}, configer it in LuaConst.cs", LuaConst.toluaDir);
                    throw new LuaException(msg);
                }

                AddSearchPath(LuaConst.toluaDir);
                AddSearchPath(LuaConst.luaDir);

                InitCustomLuaPath();
#endif
            }
        }

        protected virtual void InitCustomLuaPath()
        {

        }
    }
}
