require 'Launch/UI/View/Cmd/CmdBaseView'
CmdCloseView = Class(CmdBaseView)

function CmdCloseView:Execute( context )
	CmdCloseView.superclass.Execute(self,context)
	--完成
	Log("CmdCloseView:Execute")
	local curState = self.viewCtrl.State
	if(curState ~= ViewState.Opening and curState ~= ViewState.Opened) then
		self:OnExecuteDone(CmdExecuteState.Fail)
		return
	end
	self.viewCtrl:UpdateState(ViewState.Closing)
	self.viewCtrl:Exit()
	local anims = self.viewCtrl:BuildCloseAnims()
	if(anims ~= nil and #anims > 0) then
		self.cmdSequence = CommandSequence.new()
		for i=1,#anims do
			self.cmdSequence:AddSubCommand(anims[i])
		end
		local onAnimDone = function (cmd)
			self.viewCtrl:OnCloseAnimDone()
			self:OnExecuteDone(CmdExecuteState.Success)
		end
		self.cmdSequence:AddDoneFunc(onAnimDone)
		self.cmdSequence:Execute(nil)
		
	else
		self.viewCtrl:OnCloseAnimDone()
		self:OnExecuteDone(CmdExecuteState.Success)
	end
end

function CmdCloseView:OnExecuteFinish()
	
	if(self.state == CmdExecuteState.Success) then
		self.viewCtrl:UpdateState(ViewState.Closed)
	end
end

function CmdCloseView:OnDestroy()
	CmdCloseView.superclass.OnDestroy(self)
	--销毁
	if(self.cmdSequence ~= nil) then
		self.cmdSequence:OnDestroy()
		self.cmdSequence = nil
	end
	Log("CmdCloseView:OnDestroy")
end

