require 'Launch/Resource/LuaResourceMgr'
require 'Launch/UI/View/Cmd/CmdBaseView'
CmdInitView = Class(CmdBaseView)

function CmdInitView:Execute( context )
	CmdInitView.superclass.Execute(self,context)
	--下载资源，并且InitUI
	Log("CmdInitView:Execute")
	if(self.viewCtrl.State ~= ViewState.None) then
		--已经初始化的不再出事话
		self:OnExecuteDone(CmdExecuteState.Fail)
		return
	end
	--更新当前面板状态
	self.viewCtrl:UpdateState(ViewState.Initing)
	local onResLoaded = function (luaRes)
		self:OnResLoaded(luaRes)
	end
	self.path = self.viewCtrl:DependResource()
	self.callback = onResLoaded
	LuaResourceMgr.Load(self.path,self.callback)
	
end

function CmdInitView:OnResLoaded(luaRes)
	self.path = nil
	self.callback = nil
	self.viewCtrl:SetResources(luaRes)
	self.viewCtrl:InitUI()
	self:OnExecuteDone(CmdExecuteState.Success)
end

function CmdInitView:OnDestroy()
	CmdInitView.superclass.OnDestroy(self)
	if(self.path ~= nil) then
		LuaResourceMgr.RemoveListener(self.path,self.callback)
		self.path = nil
		self.callback = nil
	end
	--销毁
	Log("CmdInitView:OnDestroy")
end
