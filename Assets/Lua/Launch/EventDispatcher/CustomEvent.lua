require 'Launch/EventDispatcher/EventDispatcher'

function CustomEvent()
	local o = {}
	o.EventDispatcher = EventDispatcher:New()
	o.AddEvent = function(key, action)	
		o.EventDispatcher:AddEvent(key,action)
	end

	o.RemoveEvent = function (key,action)
		o.EventDispatcher:RemoveEvent(key,action)
	end 

	o.SendEvent = function (key, ...)
		o.EventDispatcher:SendEvent(key,...)
	end
	return o
end