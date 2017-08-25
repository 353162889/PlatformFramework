require 'Launch/UI/View/Cmd/CmdBaseView'
require 'Launch/Schedule/LuaScheduleMgr'
CmdDestroyView = Class(CmdBaseView)


function CmdDestroyView:Execute( context )
	CmdDestroyView.superclass.Execute(self,context)
	--完成
	Log("CmdDestroyView:Execute")
	local curState = self.viewCtrl.State
	if(curState == ViewState.Destroying or curState == ViewState.None) then
		self:OnExecuteDone(CmdExecuteState.Fail)
		return
	end
	self.viewCtrl:UpdateState(ViewState.Destroying)
	self.autoDestoryTime = nil
	if(self.param ~= nil and #self.param > 0) then
		self.autoDestoryTime = self.param[1]
	end
	if(self.autoDestoryTime ~= nil and self.autoDestoryTime > 0) then
		self.onDelay = function (delay)
			self.onDelay = nil
			self:OnExecuteDone(CmdExecuteState.Success)
		end
		LuaScheduleMgr.AddScheduler(self.onDelay,self.autoDestoryTime,1)
	else
		self:OnExecuteDone(CmdExecuteState.Success)
	end
end

function CmdDestroyView:OnDestroy()
	CmdDestroyView.superclass.OnDestroy(self)
	--销毁
	if(self.onDelay ~= nil) then
		LuaScheduleMgr.RemoveScheduler(self.onDelay)
		self.onDelay = nil
	end
	Log("CmdDestroyView:OnDestroy")
end
