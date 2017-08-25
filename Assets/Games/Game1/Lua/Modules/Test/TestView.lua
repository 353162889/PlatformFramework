TestView = Class(BaseView)

function TestView:InitUI()
	TestView.superclass.InitUI(self)
	LogColor("#ff0000","TestView:InitUI")
	self.img1 = self.transform:Find("Image1"):GetComponent("CanvasRender")
	self.img2 = self.transform:Find("Image2"):GetComponent("CanvasRender")
end

function TestView:DestroyUI()
	TestView.superclass.DestroyUI(self)
	LogColor("#ff0000","TestView:DestroyUI")
end

function TestView:BindEvent()
	TestView.superclass.BindEvent(self)
	LogColor("#ff0000","TestView:BindEvent")
end

function TestView:UnBindEvent()
	TestView.superclass.UnBindEvent(self)
	LogColor("#ff0000","TestView:UnBindEvent")
end

function TestView:OnEnter( ... )
	TestView.superclass.OnEnter(self,...)

	LogColor("#ff0000","TestView:OnEnter:"..select('#',...))
	if (select('#',...) > 0) then
		LogColor("#ff0000","param:"..select(1,...))
	end
end

function TestView:OnExit()
	TestView.superclass.OnExit(self)
	LogColor("#ff0000","TestView:OnExit")
end

function TestView:OnEnterFinish()
	TestView.superclass.OnEnterFinish(self)
	LogColor("#ff0000","TestView:OnEnterFinish")
end

function TestView:OnExitFinish()
	TestView.superclass.OnExitFinish(self)
	LogColor("#ff0000","TestView:OnExitFinish")
end