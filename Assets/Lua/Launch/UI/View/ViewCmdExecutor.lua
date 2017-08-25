require 'Launch/Command/Impl/CommandDynamicSequence'
require 'Launch/UI/View/Cmd/CmdCloseView'
require 'Launch/UI/View/Cmd/CmdDestroyView'
require 'Launch/UI/View/Cmd/CmdInitView'
require 'Launch/UI/View/Cmd/CmdOpenView'

ViewCmdType = {}
ViewCmdType.CmdInitView = "CmdInitView"
ViewCmdType.CmdOpenView = "CmdOpenView"
ViewCmdType.CmdCloseView = "CmdCloseView"
ViewCmdType.CmdDestroyView = "CmdDestroyView"

ViewCmdFactory = {}
ViewCmdFactory.DicView = {
	["CmdInitView"]=CmdInitView,
	["CmdOpenView"]=CmdOpenView,
	["CmdCloseView"]=CmdCloseView,
	["CmdDestroyView"]=CmdDestroyView
}

function ViewCmdFactory.CreateCmd(cmdName,viewCtrl,...)
	local curClass = ViewCmdFactory.DicView[cmdName]
	if(curClass ~= nil) then
		local cmd = curClass.new(cmdName)
		cmd:Init(viewCtrl,...)
		return cmd
	end
	return nil
end

ViewCmdExecutor = Class(CommandDynamicSequence)
function ViewCmdExecutor:Init(viewCtrl)
	self.viewCtrl = viewCtrl
end

function ViewCmdExecutor:AddSubCommand( cmd )
	if(self:HandleCmd(cmd)) then
		Log("AddViewCmd:"..cmd.cmdView)
		ViewCmdExecutor.superclass.AddSubCommand(self,cmd)
	end
end

function ViewCmdExecutor:HandleCmd(cmd)
	if(cmd == nil) then return false end
	if(self:IsExecuting()) then
		--如果面板正在销毁中，添加了命令，那么当前销毁命令失败(中断销毁命令)
		if(self.executeChild.cmdView == ViewCmdType.CmdDestroyView) then
			self:SkipExecuteChild()
		end

		--如果添加新命令，删除已有的销毁命令
		if(self.children ~= nil) then
			for i=#self.children,1,-1 do
				local child = self.children[i]
				if(child.cmdView == ViewCmdType.CmdDestroyView) then
					table.remove(self.children,i)
					self:OnChildDestroy(child)
				end
			end
		end
	end
	
	return true
end
