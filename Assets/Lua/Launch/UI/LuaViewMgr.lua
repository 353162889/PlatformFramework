require 'Launch/UI/LuaUGUIViewWay'
LuaViewMgr = {}
local this = LuaViewMgr

this.UIRoot = nil
this.ViewContainer = nil
this.viewWay = nil
function this.InitWay(viewWay)
	this.viewWay = viewWay
	this.UIRoot = this.viewWay.UIRoot
	this.ViewContainer = this.viewWay.ViewContainer
end

function this.Open(viewName,...)
	this.viewWay.Open(viewName,...)
end

function this.Close(viewName)
	this.viewWay.Close(viewName)
end

function this.Destroy(viewName)
	this.viewWay.Destroy(viewName)
end

function this.SortInParent()
	this.viewWay.SortInParent()
end

function this.AddViewToParent(viewCtrl)
	this.viewWay.AddViewToParent(viewCtrl)
end