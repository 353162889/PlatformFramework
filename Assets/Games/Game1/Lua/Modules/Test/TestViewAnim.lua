
TestViewAnim = Class(BaseViewAnim)

function TestViewAnim:OnPlay()
	LogColor("#ff0000","TestViewAnim:OnPlay")
	self.onDelay = function (t)
		self.onDelay = nil
		self:FinishPlay()
	end
	LuaScheduleMgr.AddScheduler(self.onDelay,0.5,1)
end

function TestViewAnim:OnStop()
	LogColor("#ff0000","TestViewAnim:OnStop")
	if(self.onDelay ~= nil) then
		LuaScheduleMgr.RemoveScheduler(self.onDelay)
		self.onDelay = nil
	end
end
