require 'Launch/Command/CommandContainerBase'
--顺序执行器，失败停止
CommandSequence = Class(CommandContainerBase)

function CommandSequence:ctor()
	self.children = {}
	self.executeChild = nil
end

function CommandSequence:ChildCount()
	return #self.children
end

--是否正在执行命令
function CommandSequence:IsExecuting()
	return self.executeChild ~= nil
end

--添加子命令
function CommandSequence:AddSubCommand(cmd)
	assert(cmd ~= nil)
	cmd.parent = self
	table.insert(self.children,cmd)
end

--执行
function CommandSequence:Execute(context)
	CommandSequence.superclass.Execute(self,context)
	self:Next()
end

--跳过当前子命令
function CommandSequence:SkipExecuteChild()
	if(self:IsExecuting()) then
		self:OnChildDestroy(self.executeChild)
		self.executeChild = nil
	end
	self:Execute(self.context)
end

function CommandSequence:OnChildDone(cmd)
	self:OnChildDoneInvoke(cmd)
	self.state = cmd.state
	self:OnChildDestroy(cmd)
	self.executeChild = nil
	if(self.state == CmdExecuteState.Fail) then
		self:OnExecuteDone(CmdExecuteState.Fail)
	else
		self:Next()
	end
end

function CommandSequence:Next()
	if(#self.children > 0) then
		self.executeChild = self.children[1]
		table.remove(self.children,1)
		self.executeChild:Execute(self.context)
	else
		self:OnExecuteDone(CmdExecuteState.Success)
	end
end

function CommandSequence:OnExecuteDone(state)
	CommandSequence.superclass.OnExecuteDone(self,state)
end

function CommandSequence:Clear()
	if(self:IsExecuting()) then
		self:OnChildDestroy(self.executeChild)
		self.executeChild = nil
	end

	if(self.children ~= nil) then
		while(#self.children > 0)
		do
			local child = self.children[1];
			table.remove(self.children,1)
			self:OnChildDestroy(child)
		end
	end
	
	
end

function CommandSequence:OnDestroy()
	self:Clear()
	CommandSequence.superclass.OnDestroy(self)
end
