using System;

namespace Launch
{
	public enum CmdExecuteState
	{
		Success,
		Fail
	}

	public class CommandBase : IPoolable
	{
		public event Action<CommandBase> On_Done;

		public CommandContainerBase Parent{get;set;}

		public CmdExecuteState State{ get; protected set;}

		protected ICommandContext _context;


		public void Execute()
		{
			this.Execute (null);
		}

		public virtual void Execute(ICommandContext context)
		{
			this.State = CmdExecuteState.Success;
			this._context = context;
		}

		protected virtual void OnExecuteDone(CmdExecuteState state)
		{
			this.State = state;
			OnExecuteFinish ();
			OnDoneInvoke ();

			if (Parent != null)
			{
				Parent.OnChildDone (this);
			}
			else
			{
				this.OnDestroy ();
			}
		}
		/// <summary>
		/// 自己执行完成，在告诉父对象之前
		/// </summary>
		protected virtual void OnExecuteFinish()
		{
		}
		/// <summary>
		/// 回调执行完成的监听
		/// </summary>
		protected void OnDoneInvoke()
		{
			if (On_Done != null)
			{
				On_Done.Invoke (this);
			}
		}

		public virtual void OnDestroy()
		{
			this._context = null;
			this.Parent = null;
			On_Done = null;
		}

		public virtual void Reset()
		{
			OnDestroy ();
		}
	}
}

