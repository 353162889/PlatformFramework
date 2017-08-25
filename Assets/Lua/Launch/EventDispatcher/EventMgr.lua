require 'Launch/EventDispatcher/EventDispatcher'
EventMgr = {}
local this = EventMgr

this.EventDic = {}
this.ViewEventDic = {}
this.EventDispatcher = EventDispatcher:New()

local function KeyFormat(...)
	local temp = {...}
	local key = ""
	for i,v in pairs(temp) do
		key = key .. "\t" .. tostring(v)
	end
	return key
end


function this.AddEvent(key, action)	
	this.EventDispatcher:AddEvent(key,action)
end

function this.RemoveEvent(key,action)
	this.EventDispatcher:RemoveEvent(key,action)
end 

function this.SendEvent(key, ...)
	this.EventDispatcher:SendEvent(key,...)
end