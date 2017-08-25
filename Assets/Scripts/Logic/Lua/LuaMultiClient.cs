using LuaInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Launch
{
    public class LuaMultiClient : MonoBehaviour
    {
        private void Awake()
        {
            Init();
        }

        protected LuaMultiState luaState = null;
        protected LuaLooper looper = null;

        public virtual void BindGame(int id)
        {
            luaState.BindGame(id);
            
        }

        //启动游戏
        public virtual void StartGame()
        {
            luaState.Require("Main");
            CallFunction("Main");
        }

        protected virtual void InitState()
        {
            LuaMultiLoader luaFileLoader = new LuaMultiLoader();
            luaState = new LuaMultiState(luaFileLoader);
        }

        /// <summary>
        /// 启动lua虚拟机（外部接口）
        /// </summary>
        protected virtual void Init()
        {
            InitState();
            OpenLibs();
            luaState.LuaSetTop(0);
            Bind();
            luaState.Start();
            looper = gameObject.AddComponent<LuaLooper>();
            looper.luaState = luaState;
        }

        protected virtual void Bind()
        {
            LuaBinder.Bind(luaState);
            LuaCoroutine.Register(luaState, this);
        }

        /// <summary>
        /// 使用lua第三方包
        /// </summary>
        void OpenLibs()
        {
            luaState.OpenLibs(LuaDLL.luaopen_lpeg);
            luaState.OpenLibs(LuaDLL.luaopen_pb);
            OpenCJson();
        }

        //cjson 比较特殊，只new了一个table，没有注册库，这里注册一下
        private void OpenCJson()
        {
            luaState.LuaGetField(LuaIndexes.LUA_REGISTRYINDEX, "_LOADED");
            luaState.OpenLibs(LuaDLL.luaopen_cjson);
            luaState.LuaSetField(-2, "cjson");
            luaState.OpenLibs(LuaDLL.luaopen_cjson_safe);
            luaState.LuaSetField(-2, "cjson.safe");
        }

        public object[] DoFile(string filename)
        {
            return luaState.DoFile(filename);
        }
        public object[] CallFunction(string funcName, params object[] args)
        {
            LuaFunction func = luaState.GetFunction(funcName);
            if (func != null)
            {
                object[] result = func.Call(args);
                func.Dispose();
                func = null;
                return result;
            }
            return null;
        }
        public LuaTable CreateLuaTable()
        {
            return (LuaTable)luaState.DoString("return {}")[0];
        }

        public LuaTable CreateLuaTable(IDictionary objs)
        {
            var table = CreateLuaTable();

            foreach (var key in objs.Keys)
            {
                table[key.ToString()] = objs[key];
            }
            return table;
        }

        public LuaTable ToLuaTable(IDictionary objs)
        {
            return CreateLuaTable(objs);
        }


        void OnDestroy()
        {
            Close();
        }
        public void Close()
        {
            if (luaState != null)
            {
                luaState.Dispose();
                luaState = null;
            }
        }
    }
}
