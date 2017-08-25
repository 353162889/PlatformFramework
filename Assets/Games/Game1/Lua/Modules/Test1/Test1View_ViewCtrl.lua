require 'Modules/Test1/Test1View'
Test1View_ViewCtrl = Class(BaseViewCtrl)

function Test1View_ViewCtrl:BuildViews()
	local list = {}
	table.insert(list,Test1View.new(self.MainGO,self))
	return list
end

function Test1View_ViewCtrl:BuildOpenAnims()
	local list = {}
	table.insert(list,TestViewAnim.new())
	return list
end

function Test1View_ViewCtrl:BuildCloseAnims()
	local list = {}
	table.insert(list,TestViewAnim.new())
	return list
end

function Test1View_ViewCtrl:SortViewLevel()
	--可以从配置中读出也可以子类改变
	return 3
end
