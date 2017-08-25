require 'Launch/Command/CommandContainerBase'
--选择执行器
CommandSelector = Class(CommandContainerBase)
function CommandSelector:ctor()
end

function CommandSelector:AddCondition(select,success,fail)
	self.select = select
	self.success = success
	self.fail = fail
	self.select.parent = self
	if(self.success ~= nil) then
		self.success.parent = self
	end
	if(self.fail ~= nil) then
		self.fail.parent = self
	end
end

function CommandSelector:OnChildDone(cmd)
	if(cmd == self.select) then
		if(self.select.state == CmdExecuteState.Success) then
			if(self.success ~= nil) then
				self.success:Execute(self.context)
			else
				self:OnExecuteDone(CmdExecuteState.Success)
			end
		else
			if(self.fail ~= nil) then
				self.fail:Execute(self.context)
			else
				self:OnExecuteDone(CmdExecuteState.Success)
			end
		end
	else
		CommandSelector.superclass.OnExecuteDone(self,cmd.state)
	end
end

function CommandSelector:Execute(context)
	CommandSelector.superclass.Execute(self,context)
	self.select:Execute(self.context)
end


function CommandSelector:OnDestroy()
	CommandSelector.superclass.OnDestroy(self)
	if(self.success ~= nil) then
		self:OnChildDestroy(self.success)
	end
	if(self.fail ~= nil) then
		self:OnChildDestroy(self.fail)
	end
end