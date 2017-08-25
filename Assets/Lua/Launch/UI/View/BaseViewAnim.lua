require 'Launch/Command/CommandBase'
BaseViewAnim = Class(CommandBase)

function BaseViewAnim:Execute( context )
	BaseViewAnim.superclass.Execute(self,context)
	self:OnPlay()
end

function BaseViewAnim:OnDestroy()
	self:OnStop()
	BaseViewAnim.superclass.OnDestroy(self)
end

function BaseViewAnim:FinishPlay()
	self:OnExecuteDone(CmdExecuteState.Success)
end

function BaseViewAnim:OnPlay()
	-- body
end

function BaseViewAnim:OnStop()
	-- body
end