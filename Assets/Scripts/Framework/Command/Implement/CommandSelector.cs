using System;

namespace Launch
{
	public class CommandSelector : CommandContainerBase
	{
		protected CommandBase _selectWork;
		protected CommandBase _succeedWork;
		protected CommandBase _failWork;

		public void AddConditionWork(CommandBase selectWork,CommandBase succeedWork,CommandBase failWork)
		{
			_selectWork = selectWork;
			_succeedWork = succeedWork;
			_failWork = failWork;

			_selectWork.Parent = this;
			if (_succeedWork != null)
			{
				_succeedWork.Parent = this;
			}
			if (_failWork != null)
			{
				_failWork.Parent = this;
			}
		}

        public override void OnChildDone (CommandBase child)
		{
			if (child == this._selectWork) 
			{
				if (this._selectWork.State == CmdExecuteState.Success) 
				{
					if (_succeedWork != null)
					{
						_succeedWork.Execute (_context);
					}
					else
					{
						this.OnExecuteDone (CmdExecuteState.Success);
					}
				} 
				else 
				{
					if (_failWork != null)
					{
						_failWork.Execute (_context);
					}
					else
					{
                        this.OnExecuteDone(CmdExecuteState.Success);
                    }
				}
			} 
			else 
			{
                this.OnExecuteDone(child.State);
			}
		}

        public override void Execute(ICommandContext context)
        {
            base.Execute(context);
            _selectWork.Execute(context);
        }

		public override void OnDestroy ()
		{
			base.OnDestroy ();
			if (_succeedWork != null)
			{
				_succeedWork.OnDestroy ();
			}
			if (_failWork != null)
			{
				_failWork.OnDestroy ();
			}
		}
	}
}

