﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class Launch_CTLScheduleWrap
{
	public static void Register(LuaState L)
	{
		L.BeginStaticLibs("CTLSchedule");
		L.RegFunction("AddScheduler", AddScheduler);
		L.RegFunction("RemoveScheduler", RemoveScheduler);
		L.EndStaticLibs();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddScheduler(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 4);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 1);
			Launch.SchedulerHandler arg1 = null;
			LuaTypes funcType2 = LuaDLL.lua_type(L, 2);

			if (funcType2 != LuaTypes.LUA_TFUNCTION)
			{
				 arg1 = (Launch.SchedulerHandler)ToLua.CheckObject(L, 2, typeof(Launch.SchedulerHandler));
			}
			else
			{
				LuaFunction func = ToLua.ToLuaFunction(L, 2);
				arg1 = DelegateFactory.CreateDelegate(typeof(Launch.SchedulerHandler), func) as Launch.SchedulerHandler;
			}

			float arg2 = (float)LuaDLL.luaL_checknumber(L, 3);
			int arg3 = (int)LuaDLL.luaL_checknumber(L, 4);
			bool o = Launch.CTLSchedule.AddScheduler(arg0, arg1, arg2, arg3);
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RemoveScheduler(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 1);
			Launch.SchedulerHandler arg1 = null;
			LuaTypes funcType2 = LuaDLL.lua_type(L, 2);

			if (funcType2 != LuaTypes.LUA_TFUNCTION)
			{
				 arg1 = (Launch.SchedulerHandler)ToLua.CheckObject(L, 2, typeof(Launch.SchedulerHandler));
			}
			else
			{
				LuaFunction func = ToLua.ToLuaFunction(L, 2);
				arg1 = DelegateFactory.CreateDelegate(typeof(Launch.SchedulerHandler), func) as Launch.SchedulerHandler;
			}

			Launch.CTLSchedule.RemoveScheduler(arg0, arg1);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

