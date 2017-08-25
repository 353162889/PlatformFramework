
CTLResource = Launch.CTLResource
--资源加载器，会将资源封装成LuaResource返回
LuaResourceWay = {}
local this = LuaResourceWay

function LuaResourceWay.Init(gameId)
	this.gameId = gameId
	this.Callbacks = {}
end

function LuaResourceWay.Load(path,callback)
	if(this.Callbacks[path] == nil) then this.Callbacks[path] = {} end
	if(this.Callbacks[path][callback] == nil) then this.Callbacks[path][callback] = {} end

	local onCallback = function (res)
		table.remove(this.Callbacks[path][callback],onCallback)
		if(#this.Callbacks[path][callback] == 0) then
			this.Callbacks[path][callback] = nil
		end
		local luaRes = LuaResource.new(res)
		callback(luaRes)
	end
	table.insert(this.Callbacks[path][callback],onCallback)
	CTLResource.GetResource(this.gameId,path,onCallback)
end

function LuaResourceWay.RemoveListener(path,callback)
	if(this.Callbacks[path][callback] == nil) then return end
	local listCallback = this.Callbacks[path][callback]
	for i=1,#listCallback do
		CTLResource.GetResource(this.gameId,path,listCallback[i])
	end
	this.Callbacks[path][callback] = nil
end

function LuaResourceWay.RemoveAllListener(path)
	if(this.Callbacks[path] == nil) then return end
	for k,v in pairs(this.Callbacks[path]) do
		this:RemoveListener(path,key)
	end
	this.Callbacks[path] = nil
end