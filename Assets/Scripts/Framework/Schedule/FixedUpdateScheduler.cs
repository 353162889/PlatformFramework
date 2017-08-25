using System;
using UnityEngine;

namespace Launch
{
	public class FixedUpdateScheduler : Scheduler<FixedUpdateScheduler>
	{
		void FixedUpdate()
		{
			this.OnTick (Time.fixedDeltaTime);
		}
	}
}

