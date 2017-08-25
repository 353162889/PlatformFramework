require 'Launch/Command/CommandBase'
CommandContainerBase = Class(CommandBase)

function CommandContainerBase:ctor()
	self.OnChildDoneFuncs = {}
end

--[[function CommandContainerBase:New()
	local o = CommandBase:New()
	o.OnChildDone = {}
	setmetatable(o,self)
	self.__index = self
	return o
end]]

--添加执行子命令完成的回调  格式如:func(cmd) end
function CommandBase:AddChildDoneFunc(func)
	if(not table.contains(self.OnChildDoneFuncs,func)) then
		table.insert(self.OnChildDoneFuncs,func)
	end
end
--移除子命令完成的回调
function CommandBase:RemoveChildDoneFunc(func)
	local index = -1
	for i,v in ipairs(self.OnChildDoneFuncs) do
		if(v == func) then
			index = i
		end
	end
	if(index > 0) then
		table.remove(self.OnChildDoneFuncs,index)
	end
end

function CommandContainerBase:OnChildDone(cmd)
	self:OnChildDoneInvoke(cmd)
end

function CommandContainerBase:OnChildDoneInvoke(cmd)
	if(self.OnChildDoneFuncs ~= nil) then
		for k,v in pairs(self.OnChildDoneFuncs) do
			if(v ~= nil) then
				v(cmd)
			end
		end
	end
end

function CommandContainerBase:OnExecuteDone(state)
	CommandContainerBase.superclass.OnExecuteDone(self,state)
end

--子对象销毁时（常用于重用子对象）
function CommandContainerBase:OnChildDestroy(cmd)
	cmd:OnDestroy()
end

function CommandContainerBase:OnDestroy()
	self.OnChildDoneFuncs = nil
	CommandContainerBase.superclass.OnDestroy(self)
end