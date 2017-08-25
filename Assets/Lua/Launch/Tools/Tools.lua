
-- 输出日志--
function Log(...)
    Launch.CTLTools.Log(GetLogStr(...))
end
--有颜色的日志, 例如:LogColor("#ff0000","测试")--
function LogColor(color, ... )
    Launch.CTLTools.Log(GetLogStr(...),color)
end

-- 错误日志--
function LogError(...)
    Launch.CTLTools.LogError(GetLogStr(...))
end

-- 警告日志--
function LogWarn(...)
    Launch.CTLTools.LogWarn(GetLogStr(...))
end

function GetLogStr(...)
    local str = ""
    local arg = ""
    for i = 1, select('#', ...) do
        arg = select(i, ...)
        str = str .. "\t" .. tostring(arg)
    end
    return str
end

function DestroyGameObject(go)
    Launch.CTLTools.DestroyGameObject(go)
end

function AddChildToParent(child,parent,worldPosStay)
    Launch.CTLTools.AddChildToParent(child,parent,worldPosStay)
end

function table.contains(table, element)
  for key, value in pairs(table) do
    if value == element then
      return true
    end
  end
  return false
end 