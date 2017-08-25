Test1View = Class(BaseView)

function Test1View:InitUI()
	Test1View.superclass.InitUI(self)
	LogColor("#ff0000","Test1View:InitUI")
end

function Test1View:DestroyUI()
	Test1View.superclass.DestroyUI(self)
	LogColor("#ff0000","Test1View:DestroyUI")
end

function Test1View:BindEvent()
	Test1View.superclass.BindEvent(self)
	LogColor("#ff0000","Test1View:BindEvent")
end

function Test1View:UnBindEvent()
	Test1View.superclass.UnBindEvent(self)
	LogColor("#ff0000","Test1View:UnBindEvent")
end

function Test1View:OnEnter( ... )
	Test1View.superclass.OnEnter(self,...)
	LogColor("#ff0000","Test1View:OnEnter:"..select('#',...))
	if (select('#',...) > 0) then
		LogColor("#ff0000","param:"..select(1,...))
	end
end

function Test1View:OnExit()
	Test1View.superclass.OnExit(self)
	LogColor("#ff0000","Test1View:OnExit")
end

function Test1View:OnEnterFinish()
	Test1View.superclass.OnEnterFinish(self)
	LogColor("#ff0000","Test1View:OnEnterFinish")
end

function Test1View:OnExitFinish()
	Test1View.superclass.OnExitFinish(self)
	LogColor("#ff0000","Test1View:OnExitFinish")
end