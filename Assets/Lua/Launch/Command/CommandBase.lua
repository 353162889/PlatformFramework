CmdExecuteState = {
	Success = 1,
	Fail = 2
}


CommandBase = Class()

--这个方法的重写不需要调用CommandBase.superclass.ctor(self)如此的调用
function CommandBase:ctor()
	self.onDone = {}
	self.parent = nil
	self.context = nil
	self.state = CmdExecuteState.Success
end

--[[function CommandBase:New()
	local o = {}
	o.onDone = {}
	o.parent = nil
	o.context = nil
	o.state = CmdExecuteState.Success
	setmetatable(o,self)
	self.__index = self
	return o
end]]

--添加执行完成的回调  格式如:func(cmd) end
function CommandBase:AddDoneFunc(func)
	if(not table.contains(self.onDone,func)) then
		table.insert(self.onDone,func)
	end
end
--移除执行完成的回调
function CommandBase:RemoveDoneFunc(func)
	local index = -1
	for i,v in ipairs(self.onDone) do
		if(v == func) then
			index = i
		end
	end
	if(index > 0) then
		table.remove(self.onDone,index)
	end
end

--开始执行命令
function CommandBase:Execute(context)
	self.state = CmdExecuteState.Success
	self.context = context
end

--命令执行完成（调用了这个就表示命令已经执行完成）
function CommandBase:OnExecuteDone(state)
	self.state = state
	self:OnExecuteFinish()
	self:OnDoneInvoke()
	if(self.parent ~= nil) then
		self.parent:OnChildDone(self)
	else
		self:OnDestroy()
	end
end

--自己执行完成，在告诉父对象之前
function CommandBase:OnExecuteFinish()
end

function CommandBase:OnDoneInvoke()
	if(self.onDone ~= nil) then
		for k,v in pairs(self.onDone) do
			if(v ~= nil) then
				v(self)
			end
		end
	end
end

--销毁
function CommandBase:OnDestroy()
	self.parent = nil
	self.context = nil
	self.onDone = nil
end