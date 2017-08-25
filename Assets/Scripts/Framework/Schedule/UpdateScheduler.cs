using System;
using UnityEngine;

namespace Launch
{
	public class UpdateScheduler : Scheduler<UpdateScheduler>
	{
		void Update()
		{
			this.OnTick (Time.deltaTime);
		}
	}
}

