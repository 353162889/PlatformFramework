--主要控制View的逻辑
BaseView = Class()

function BaseView:ctor( go ,viewCtrl)
	self.gameObject = go
	self.viewCtrl = viewCtrl
	self.transform = self.gameObject.transform
end

function BaseView:BuildSubView()
	return nil
end

function BaseView:InitUI()
	self.listSubView = self:BuildSubView()
	if(self.listSubView ~= nil) then
		for i=1,#self.listSubView do
			self.listSubView:InitUI()
		end
	end
end

function BaseView:DestroyUI()
	if(self.listSubView ~= nil) then
		for i=1,#self.listSubView do
			self.listSubView:DestroyUI()
		end
	end
end

function BaseView:BindEvent()
	if(self.listSubView ~= nil) then
		for i=1,#self.listSubView do
			self.listSubView:BindEvent()
		end
	end
end

function BaseView:UnBindEvent()
	if(self.listSubView ~= nil) then
		for i=1,#self.listSubView do
			self.listSubView:UnBindEvent()
		end
	end
end

function BaseView:OnEnter( ... )
	if(self.listSubView ~= nil) then
		for i=1,#self.listSubView do
			self.listSubView:OnEnter(...)
		end
	end
end

function BaseView:OnExit()
	if(self.listSubView ~= nil) then
		for i=1,#self.listSubView do
			self.listSubView:OnExit()
		end
	end
end

function BaseView:OnEnterFinish()
	if(self.listSubView ~= nil) then
		for i=1,#self.listSubView do
			self.listSubView:OnEnterFinish()
		end
	end
end

function BaseView:OnExitFinish()
	if(self.listSubView ~= nil) then
		for i=1,#self.listSubView do
			self.listSubView:OnExitFinish()
		end
	end
end
