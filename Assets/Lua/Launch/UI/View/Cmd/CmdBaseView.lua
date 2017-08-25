require 'Launch/Command/CommandBase'
CmdBaseView = Class(CommandBase)

function CmdBaseView:ctor( cmdView )
	self.cmdView = cmdView
end

function CmdBaseView:Init(viewCtrl,...)
	self.viewCtrl = viewCtrl
	self.param = {...}
end

function CmdBaseView:OnDestroy()
	self.viewCtrl = nil
	self.param = nil
	CmdBaseView.superclass.OnDestroy(self)
end
