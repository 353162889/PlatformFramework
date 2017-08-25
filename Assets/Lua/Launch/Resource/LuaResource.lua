--lua中的资源对象，可重写（可继承，也可不继承，只要有这些方法即可）
LuaResource = Class()

function LuaResource:ctor( ... )
	--self.Res是资源管理器Resource
	self.Res = select(1,...)
end

function LuaResource:Retain()
	self.Res:Retain()
end

function LuaResource:Release()
	self.Res:Release()
end

function LuaResource:GetObj(name)
	return Launch.Resource.GetGameObject(self.Res,name)
end