require 'System.Timer'
require 'Launch/Base/Class'
require 'Launch/Resource/LuaResourceMgr'
require 'Launch/Schedule/LuaScheduleMgr'
require 'Launch/UI/LuaViewMgr'
require 'Modules/Test/TestView_ViewCtrl'
require 'Modules/Test1/Test1View_ViewCtrl'
require 'Modules/Test/TestViewAnim'
require 'Launch/Tools/Tools'


function Main()
	print("Start Game1")
	local gameId = 1
	--初始化资源加载器
	LuaResourceWay.Init(gameId)
	LuaResourceMgr.InitWay(LuaResourceWay)

	--初始化时间加载器
	LuaScheduleWay.Init(gameId)
	LuaScheduleMgr.InitWay(LuaScheduleWay)

	--初始化UI管理器
	LuaUGUIViewWay.Init(gameId)
	LuaViewMgr.InitWay(LuaUGUIViewWay)

	-- LuaViewMgr.Open("TestView","test","test1")
	local randomOperate = function ()
		local r = math.random(0,4)
		if(r >= 0 and r < 1) then
			LuaViewMgr.Open("Test1View","test","test1")
		elseif(r >= 1 and r < 2) then
			LuaViewMgr.Close("Test1View")
		elseif(r >= 2 and r < 3) then
			LuaViewMgr.Open("TestView","test","test1")
		elseif(r >= 3 and r < 4) then
			LuaViewMgr.Close("TestView")
		end
	end
	LuaViewMgr.Open("Test1View")
	LuaViewMgr.Open("TestView")
	
	-- LuaViewMgr.Close("TestView")
	-- LuaViewMgr.Open("TestView")
	-- LuaViewMgr.Close("Test1View")
	-- LuaViewMgr.Open("Test1View")
	-- local onDelay = function (f)
	-- 	randomOperate()
	-- end
	-- LuaScheduleMgr.AddScheduler(onDelay,1,0)
	
	-- local onDelay1 = function (f)
	-- 	randomOperate()
	-- end
	-- LuaScheduleMgr.AddScheduler(onDelay1,0.3,0)
end
