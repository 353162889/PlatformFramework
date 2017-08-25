require 'Launch/UI/View/Cmd/CmdBaseView'
require 'Launch/Command/Impl/CommandSequence'

CmdOpenView = Class(CmdBaseView)

function CmdOpenView:Execute( context )
	CmdOpenView.superclass.Execute(self,context)
	--完成
	Log("CmdOpenView:Execute")
	local curState = self.viewCtrl.State
	if(curState ~= ViewState.Initing and curState ~= ViewState.Closed and curState ~= ViewState.Destroying) then
		self:OnExecuteDone(CmdExecuteState.Fail)
		return
	end
	--更新当前面板状态
	self.viewCtrl:UpdateState(ViewState.Opening)
	--面板打开参数
	self.viewCtrl:Enter(unpack(self.param))
	--播放打开面板动画
	local anims = self.viewCtrl:BuildOpenAnims()
	if(anims ~= nil and #anims > 0) then
		self.cmdSequence = CommandSequence.new()
		for i=1,#anims do
			self.cmdSequence:AddSubCommand(anims[i])
		end
		local onAnimDone = function (cmd)
			self.viewCtrl:OnOpenAnimDone()
			self:OnExecuteDone(CmdExecuteState.Success)
		end
		self.cmdSequence:AddDoneFunc(onAnimDone)
		self.cmdSequence:Execute(nil)
		
	else
		self.viewCtrl:OnOpenAnimDone()
		self:OnExecuteDone(CmdExecuteState.Success)
	end
end

function CmdOpenView:OnExecuteFinish()
	if(self.state == CmdExecuteState.Success) then
		self.viewCtrl:UpdateState(ViewState.Opened)
	end
	CmdOpenView.superclass.OnExecuteFinish(self)
end

function CmdOpenView:OnDestroy()
	CmdOpenView.superclass.OnDestroy(self)
	--销毁
	if(self.cmdSequence ~= nil) then
		self.cmdSequence:OnDestroy()
		self.cmdSequence = nil
	end
	Log("CmdOpenView:OnDestroy")
end
