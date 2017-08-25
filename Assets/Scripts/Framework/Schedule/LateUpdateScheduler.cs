using System;
using UnityEngine;

namespace Launch
{
	public class LateUpdateScheduler : Scheduler<LateUpdateScheduler>
	{
		void LateUpdate()
		{
			this.OnTick (Time.deltaTime);
		}
	}
}

