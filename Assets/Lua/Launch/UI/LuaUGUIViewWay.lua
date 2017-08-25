require 'Launch/UI/View/BaseViewCtrl'

CTLView = Launch.CTLView

LuaUGUIViewWay = {}
local this = LuaUGUIViewWay

this.UIRoot = nil
this.UIRootTrans = nil
this.ViewContainer = nil
this.ViewContainerTrans = nil
this.DicViewContainer = nil
this.DicViewCtrl = nil
this.ListViewCtrl = nil

function this.Init(gameId)
	this.UIRoot = Launch.CTLTools.GetUIRoot(gameId)
	this.UIRootTrans = this.UIRoot.transform
	this.ViewContainerTrans = this.UIRootTrans:Find("Canvas")
	this.ViewContainer = this.ViewContainerTrans.gameObject
	this.DicViewContainer = {}
	this.DicViewCtrl = {}
	this.ListViewCtrl = {}
end

function this.Open(viewName,...)
	local viewCtrl = this.DicViewCtrl[viewName]
	if(viewCtrl == nil) then
		viewCtrl = this.CreateViewCtrl(viewName)
		this.DicViewCtrl[viewName] = viewCtrl
		table.insert(this.ListViewCtrl,viewCtrl)
		viewCtrl:SetDestroyCallback(this.OnDestroy)
	else
		--将当前view设置到最前面
		local removed = false
		for i=#this.ListViewCtrl,1,-1 do
			if(this.ListViewCtrl[i].viewName == viewName) then
				removed = true
				table.remove(this.ListViewCtrl,i)
				break
			end
		end
		if(removed) then
			table.insert(this.ListViewCtrl,viewCtrl)
		end
	end
	viewCtrl:Open(...)
end

--如果不想按配置来做处理，必须按照固定的命名规则
function this.CreateViewCtrl(viewName)
	--这里可以在配置中添加类名，如果没有类名，寻找默认类名
	return loadstring("return "..viewName.."_ViewCtrl.new('"..viewName.."')")()
end

function this.Close(viewName)
	local viewCtrl = this.DicViewCtrl[viewName]
	if(viewCtrl ~= nil) then
		--获取自动销毁时间
		local autoDestroyTime = 0.5
		viewCtrl:Close(autoDestroyTime)
	end
end

function this.Destroy(viewName)
	local viewCtrl = this.DicViewCtrl[viewName]
	if(viewCtrl ~= nil) then
		viewCtrl:Destroy()
	end
end

function this.OnDestroy(viewCtrl)
	--销毁后移除
	this.DicViewCtrl[viewCtrl.viewName] = nil
	for i=#this.ListViewCtrl,1,-1 do
		if(this.ListViewCtrl[i].viewName == viewCtrl.viewName) then
			table.remove(this.ListViewCtrl,i)
			break
		end
	end
end

function this.SortInParent()
	for i=1,#this.ListViewCtrl do
		--更新Carvas的sortLayer
		local viewCtrl = this.ListViewCtrl[i]
		local go = viewCtrl.MainGO
		if(go ~= nil) then
			local level = viewCtrl:SortViewLevel()
			if(viewCtrl.curLevel ~= level) then
				--改变父容器
				this.AddViewToParent(viewCtrl)
			end
			local container = this.DicViewContainer[level]
			CTLView.UpdateUGUISort(container,go,level,i)
			--更新当前子对象的排列
			-- CTLView.UpdateUGUILayer(go,level,i)
		end
	end
end

function this.AddViewToParent(viewCtrl)
	local level = viewCtrl:SortViewLevel()
	local container = this.DicViewContainer[level]
	if(container == nil) then
		container = CTLView.CreateUGUIViewContainer(this.ViewContainer,level)
		this.DicViewContainer[level] = container
	end
	viewCtrl.curLevel = level
	AddChildToParent(viewCtrl.MainGO,container)
end

