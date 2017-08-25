require 'Launch/Tools/Tools'
require 'Launch/Resource/LuaResource'
require 'Launch/Resource/LuaResourceWay'
LuaResourceMgr = {}
local this = LuaResourceMgr
function this.InitWay(resourceWay)
	this.resourceWay = resourceWay
end

--加载资源,path:路径,callback：回调，方法为  function Test(LuaResource) end，返回LuaResource结构
function this.Load(path,callback)
	if(this.resourceWay ~= nil) then
		this.resourceWay.Load(path,callback)
	else
		LogError("ResourceWay is nil,can not load res:",path)
	end
end

function this.RemoveListener(path,callback)
	if(this.resourceWay ~= nil) then
		this.resourceWay.RemoveListener(path,callback)
	else
		LogError("ResourceWay is nil,can not load res:",path)
	end
end

function this.RemoveAllListener(path)
	if(this.resourceWay ~= nil) then
		this.resourceWay.RemoveAllListener(path,callback)
	else
		LogError("ResourceWay is nil,can not RemoveAllListener:",path)
	end
end