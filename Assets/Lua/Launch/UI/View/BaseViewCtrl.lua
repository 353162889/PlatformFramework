require 'Launch/Tools/Tools'
require 'Launch/UI/View/BaseView'
require 'Launch/UI/View/BaseViewAnim'
require 'Launch/UI/View/ViewCmdExecutor'

ViewState = {
	None = 1,
	Initing = 2,
	Opening = 3,
	Opened = 4,
	Closing = 5,
	Closed = 6,
	Destroying = 7
}

--主要控制View的打开关闭逻辑
BaseViewCtrl = Class()

function BaseViewCtrl:ctor(viewName)
	--传入当前面板的名称
	self.viewName = viewName
	self.executor = ViewCmdExecutor.new(false)
	self.executor:Init(self)
	self.State = ViewState.None
end

function BaseViewCtrl:SetDestroyCallback(destroyCallback)
	self.destroyCallback = destroyCallback
end

function BaseViewCtrl:UpdateState(state)
	self.State = state
end

function BaseViewCtrl:SetResources(luaRes)
	if(self.luaRes ~= nil) then
		self.luaRes:Release()
	end
	self.luaRes = luaRes
	self.luaRes:Retain()
end

function BaseViewCtrl:Open(...)
	if(self.State == ViewState.None) then
		local cmdInit = ViewCmdFactory.CreateCmd(ViewCmdType.CmdInitView,self)
		self.executor:AddSubCommand(cmdInit)
	end
	local cmdOpen = ViewCmdFactory.CreateCmd(ViewCmdType.CmdOpenView,self,...)
	self.executor:AddSubCommand(cmdOpen)
end

function BaseViewCtrl:Close(autoDestoryTime)
	local cmdClose = ViewCmdFactory.CreateCmd(ViewCmdType.CmdCloseView,self)
	self.executor:AddSubCommand(cmdClose)
	if(autoDestoryTime ~= nil and autoDestoryTime > 0) then
		self:Destroy(autoDestoryTime)
		return 
	end
	autoDestoryTime = self:DestroyTime()
	if(autoDestoryTime ~= nil and autoDestoryTime > 0) then
		self:Destroy(autoDestoryTime)
	end
end

function BaseViewCtrl:Destroy(autoDestoryTime)
	local cmdDestroy = ViewCmdFactory.CreateCmd(ViewCmdType.CmdDestroyView,self,autoDestoryTime)
	local onDestroyDone = function (cmd)
		self:OnDestroyDone(cmd)
	end
	cmdDestroy:AddDoneFunc(onDestroyDone)
	self.executor:AddSubCommand(cmdDestroy)
end

function BaseViewCtrl:OnDestroyDone(cmd)
	if(cmd.State == CmdExecuteState.Fail) then return end
	if(self.destroyCallback ~= nil) then
		local temp = self.destroyCallback
		self.destroyCallback = nil
		temp(self)
	end
	-- body
	if(self.luaRes ~= nil) then
		self.luaRes:Release()
		self.luaRes = nil
	end
	if(self.listSubViews ~= nil) then
		for i=1,#self.listSubViews do
			self.listSubViews[i]:UnBindEvent()
		end
		for i=1,#self.listSubViews do
			self.listSubViews[i]:DestroyUI()
		end
	end

	if(self.MainGO ~= nil) then
		DestroyGameObject(self.MainGO)
	end
	self.executor:OnDestroy()
	self:UpdateState(ViewState.None)
end

function BaseViewCtrl:SetMainGOActive(active)
	if(self.MainGO ~= nil) then
		self.MainGO:SetActive(active)
	end
end

function BaseViewCtrl:InitUI()
	-- body
	if(self.MainGO ~= nil) then
		DestroyGameObject(self.MainGO)
	end
	self.MainGO = self.luaRes:GetObj(self:DependResource())
	--先将MainGO的active设置为false
	self:SetMainGOActive(false)
	--将MainGO挂载到UI对象下面
	self:AddGOToParent()
	self.listSubViews = self:BuildViews()
	if(self.listSubViews ~= nil) then
		for i=1,#self.listSubViews do
			self.listSubViews[i]:InitUI()
		end
		for i=1,#self.listSubViews do
			self.listSubViews[i]:BindEvent()
		end
	end
end

function BaseViewCtrl:Enter( ... )
	self:SetMainGOActive(true)
	--将MainGO排序
	self:SortInParent()
	if(self.listSubViews ~= nil) then
		for i=1,#self.listSubViews do
			self.listSubViews[i]:OnEnter(...)
		end
	end
end

function BaseViewCtrl:Exit()
	if(self.listSubViews ~= nil) then
		for i=1,#self.listSubViews do
			self.listSubViews[i]:OnExit()
		end
	end
end

function BaseViewCtrl:OnOpenAnimDone()
	if(self.listSubViews ~= nil) then
		for i=1,#self.listSubViews do
			self.listSubViews[i]:OnEnterFinish()
		end
	end
end

function BaseViewCtrl:OnCloseAnimDone()
	if(self.listSubViews ~= nil) then
		for i=1,#self.listSubViews do
			self.listSubViews[i]:OnExitFinish()
		end
	end
	self:SetMainGOActive(false)
end

function BaseViewCtrl:BuildOpenAnims()
	self.listOpenAnims = nil
end

function BaseViewCtrl:BuildCloseAnims()
	self.listCloseAnims = nil
end

function BaseViewCtrl:BuildViews()
	LogError("must impl BuildViews way")
	return nil
end

function BaseViewCtrl:DependResource()
	return "Views/"..self.viewName..".prefab"
end

function BaseViewCtrl:AddGOToParent()
	if(self.MainGO ~= nil) then
		--将子对象添加到父对象容器里面
		LuaViewMgr.AddViewToParent(self)
	end
end

function BaseViewCtrl:SortInParent()
	LuaViewMgr.SortInParent()
end

function BaseViewCtrl:DestroyTime()
	return nil
end

function BaseViewCtrl:SortViewLevel()
	--可以从配置中读出也可以子类改变
	return 1
end