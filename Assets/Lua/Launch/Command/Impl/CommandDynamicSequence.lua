require 'Launch/Command/Impl/CommandSequence'

CommandDynamicSequence = Class(CommandSequence)

function CommandDynamicSequence:ctor(...)
	self.children = {}
	self.executeChild = nil
	local autoDestroy = select(1,...)
	self.autoDestroy = true
	if(autoDestroy ~= nil) then
		self.autoDestroy = autoDestroy
	end
end

--添加子命令
function CommandDynamicSequence:AddSubCommand(cmd)
	CommandDynamicSequence.superclass.AddSubCommand(self,cmd)
	self:Execute()
end

--执行
function CommandDynamicSequence:Execute(context)
	if(not self:IsExecuting()) then
		CommandDynamicSequence.superclass.Execute(self,context)
	end
end

function CommandDynamicSequence:OnExecuteDone(state)
	self.state = state
	self:OnExecuteFinish()
	self:OnDoneInvoke()
	if(self.parent ~= nil) then
		self.parent:OnChildDone(self)
	elseif(self.autoDestroy) then
		self:OnDestroy()
	end
end

