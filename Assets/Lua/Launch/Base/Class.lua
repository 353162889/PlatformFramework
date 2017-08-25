local _Class={}
 
function Class(super)
	local Class_type={}
	Class_type.ctor=false
	Class_type.super=super
	Class_type.superclass = _Class[super]

	Class_type.new=function(...)
			local obj={}
			do
				local create
				create = function(c,...)
					if c.super then
						create(c.super,...)
					end
					if c.ctor then
						c.ctor(obj,...)
					end
				end
 
				create(Class_type,...)
			end
			setmetatable(obj,{ __index=_Class[Class_type] })
			return obj
		end
	local vtbl={}

	_Class[Class_type]=vtbl

	setmetatable(Class_type,{__newindex=
		function(t,k,v)
			vtbl[k]=v
		end
	})
 
	if super then
		setmetatable(vtbl,{__index=
			function(t,k)
				local ret=_Class[super][k]
				vtbl[k]=ret
				return ret
			end
		})
	end
 
	return Class_type
end
