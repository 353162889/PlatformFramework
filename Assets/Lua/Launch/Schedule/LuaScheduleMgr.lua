require 'Launch/Tools/Tools'
require 'Launch/Schedule/LuaScheduleWay'
LuaScheduleMgr = {}
local this = LuaScheduleMgr

function this.InitWay(scheduleWay)
	this.scheduleWay = scheduleWay
end

function this.AddScheduler( onTimer , delay, times)
	if(this.scheduleWay ~= nil) then
		this.scheduleWay.AddScheduler(onTimer,delay,times)
	else
		LogError("ScheduleWay is nil,can not AddScheduler")
	end
end

function this.RemoveScheduler( onTimer )
	if(this.scheduleWay ~= nil) then
		this.scheduleWay.RemoveScheduler(onTimer)
	else
		LogError("ScheduleWay is nil,can not RemoveScheduler")
	end
	
end