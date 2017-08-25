require 'System.Timer'

function Main()
	print("Start Game2")
	Game2.Test.Debug("Game2.Debug")
	local index = 0
	local timer = Timer.New(function ()
		print("Game2,index:"..index)
		index = index + 1
	end,1,-1,false)
	timer:Start()
end