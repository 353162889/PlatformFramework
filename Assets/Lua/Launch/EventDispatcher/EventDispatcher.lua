--事件对象
local EventEntity = {}
local EventOperate = {
	AddEvent = 1,
	RemoveEvent = 2
}
function EventEntity:New()
	local o = {}
	setmetatable(o,self)
	self.__index = self
	return o
end

function EventEntity:Init(eventID,func,eventOperate)
	self.eventID = eventID
	self.func = func
	self.operate = eventOperate
end

function EventEntity:Reset()
	self.eventID = nil
	self.func = nil
	self.operate = EventOperate.AddEvent
end
--end

--事件对象池
local EventPool = {}
function EventPool.GetEntity()
	local entity
	if(#EventPool > 0) then
		entity = EventPool[1]
		table.remove(EventPool,1)
		return entity
	else
		entity = EventEntity:New()
	end
	return entity
end

function EventPool.SaveEntity(entity)
	entity:Reset()
	if(#EventPool < 20) then
		table.insert(EventPool,entity)
	end
end
--end

--派发事件对象
local DispatchEntry = {}
function DispatchEntry:New()
	local o = {}
	setmetatable(o,self)
	self.__index = self
	return o
end

function DispatchEntry:Init(eventID,args)
	self.eventID = eventID
	self.args = args
end

function DispatchEntry:Reset()
	self.eventID = nil
	self.args = nil
end
--end

--事件对象池
local DispatchPool = {}
function DispatchPool.GetEntity()
	local entity
	if(#DispatchPool > 0) then
		entity = DispatchPool[1]
		table.remove(DispatchPool,1)
		return entity
	else
		entity = DispatchEntry:New()
	end
	return entity
end

function DispatchPool.SaveEntity(entity)
	entity:Reset()
	if(#DispatchPool < 20) then
		table.insert(DispatchPool,entity)
	end
end
--end

--事件实现
EventDispatcher = {}
function EventDispatcher:New()
	local o = {}
	o.mapEvents = {}
	o.delayEvents = {}
	o.delayDispatchevents = {}
	o.sendingID = nil
	o.destroy = false
	setmetatable(o,self)
	self.__index = self
	return o
end

function EventDispatcher:AddEvent(eventID,func)
	assert(type(func) == "function")
	if(self:IsDispatcher()) then
		local entity = EventPool.GetEntity()
		entity:Init(eventID,func,EventOperate.AddEvent)
		table.insert(self.delayEvents,entity)
	else
		local listEvents = self.mapEvents[eventID]
		if(listEvents == nil) then
			listEvents = {}
			self.mapEvents[eventID] = listEvents
		end
		if(not table.contains(listEvents,func)) then
			table.insert(listEvents,func)
		end
	end
end

function EventDispatcher:RemoveEvent(eventID,func)
	local log = false
	if(eventID == ClockEvent.OnSecond) then
		log = true
	end
	if(self:IsDispatcher()) then
		if(log) then
			LogColor("#ff0000","RemoveEvent,IsDispatcher")
		end
		local entity = EventPool.GetEntity()
		entity:Init(eventID,func,EventOperate.RemoveEvent)
		table.insert(self.delayEvents,entity)
	else
		if(log) then
			LogColor("#ff0000","RemoveEventStart")
		end
		local listEvents = self.mapEvents[eventID]
		if(listEvents ~= nil) then
			local index = -1
			for i=#listEvents,1,-1 do
				if(listEvents[i] == func) then
					if(log) then
						LogColor("#ff0000","RemoveEventFinish")
					end
					table.remove(listEvents,i)
				end
			end
		end
	end
end

function EventDispatcher:SendEvent(eventID,...)
	if(self:IsDispatcher()) then
		local dispatchEntity = DispatchPool.GetEntity()
		dispatchEntity:Init(eventID,{...})
		table.insert(self.delayDispatchevents,dispatchEntity)
		-- LogError("can not dispatch when current is dispatching!sendingID="..self.sendingID..",sendID="..eventID)
		return
	end
	local listEvents = self.mapEvents[eventID]
	self.sendingID = eventID
	if(listEvents ~= nil)then
		for k,v in pairs(listEvents) do
			if(not self.destroy) then
				trycall(v,...)
				-- v(...)
			end
		end
	end
	self.sendingID = nil
	if(not self.destroy) then
		while(#self.delayEvents > 0)
			do
			local entity = self.delayEvents[1]
			table.remove(self.delayEvents,1)
			if(entity.operate == EventOperate.AddEvent) then
				self:AddEvent(entity.eventID,entity.func)
			elseif(entity.operate == EventOperate.RemoveEvent) then
				self:RemoveEvent(entity.eventID,entity.func)
			end
			EventPool.SaveEntity(entity)
		end
	end
	if(#self.delayDispatchevents > 0) then
		local dispatchEntity = self.delayDispatchevents[1]
		table.remove(self.delayDispatchevents,1)
		self:SendEvent(dispatchEntity.eventID,unpack(dispatchEntity.args))
		DispatchPool.SaveEntity(dispatchEntity)
	end
end

function EventDispatcher:Dispose()
	self.destroy = true
	self.mapEvents = nil
	self.delayEvents = nil
end

function EventDispatcher:IsDispatcher()
	return (self.sendingID ~= nil)
end



