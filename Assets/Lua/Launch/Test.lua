Test = {}


function BB( ... )
	-- body
end

function Test:New()
	local o = {Callbacks = {}}
	self.__index = self
	setmetatable(o,self)
	return o
end

function Test:Load(path,callback)
	if(self.Callbacks[path] == nil) then self.Callbacks[path] = {} end
	if(self.Callbacks[path][callback] == nil) then self.Callbacks[path][callback] = {} end

	local onCallback = function ()
		table.remove(self.Callbacks[path][callback],onCallback)
		print("移除")
	end
	table.insert(self.Callbacks[path][callback],onCallback)
	self:Print()
	self:Call(path,onCallback)
end

function Test:Call(path,callback)
	callback()
end

function Test:Print()
	print("length:"..#self.Callbacks["a"][BB])
end



tt = {}
tt[111] = 1
tt[2] = 2
print(tt)

local test = Test:New()
test:Load("a",BB)
-- test:Print()
print("length:"..#test.Callbacks["a"][BB])