using System;

namespace Launch
{
	public abstract class CommandContainerBase : CommandBase
	{
		public event Action<CommandBase> On_ChildDone;

		/// <summary>
		/// 当前子对象执行完成
		/// </summary>
		/// <param name="command">Command.</param>
		public virtual void OnChildDone(CommandBase command)
		{
			On_ChildDoneCallback (command);
			OnChildDestroy (command);
		}

		/// <summary>
		/// 当前子对象执行完成的回调
		/// </summary>
		/// <param name="command">Command.</param>
		protected void On_ChildDoneCallback(CommandBase command)
		{
			if (On_ChildDone != null)
			{
				this.On_ChildDone (command);
			}
		}

		/// <summary>
		/// 子对象销毁时（常用于重用子对象）
		/// </summary>
		/// <param name="command">Command.</param>
		protected virtual void OnChildDestroy(CommandBase command)
		{
			command.OnDestroy ();
		}

		/// <summary>
		/// 清楚当前所有子对象
		/// </summary>
		public virtual void Clear()
		{
		}

		/// <summary>
		/// 当前对象销毁
		/// </summary>
		public override void OnDestroy ()
		{
			this.On_ChildDone = null;
			base.OnDestroy ();
		}
	}
}

