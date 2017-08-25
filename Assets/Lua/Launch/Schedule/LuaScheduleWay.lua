LuaScheduleWay = {}
local this = LuaScheduleWay

CTLSchedule = Launch.CTLSchedule
function LuaScheduleWay.Init(gameId)
	this.gameId = gameId
end

function LuaScheduleWay.AddScheduler( onTimer , delay, times)
	CTLSchedule.AddScheduler(this.gameId,onTimer,delay,times)
end

function LuaScheduleWay.RemoveScheduler( onTimer )
	CTLSchedule.RemoveScheduler(this.gameId,onTimer)
end