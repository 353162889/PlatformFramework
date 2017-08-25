require 'Modules/Test/TestView'
TestView_ViewCtrl = Class(BaseViewCtrl)

function TestView_ViewCtrl:BuildViews()
	local list = {}
	table.insert(list,TestView.new(self.MainGO,self))
	return list
end

function TestView_ViewCtrl:BuildOpenAnims()
	local list = {}
	table.insert(list,TestViewAnim.new())
	return list
end

function TestView_ViewCtrl:BuildCloseAnims()
	local list = {}
	table.insert(list,TestViewAnim.new())
	return list
end

function TestView_ViewCtrl:SortViewLevel()
	--可以从配置中读出也可以子类改变
	return 2
end
