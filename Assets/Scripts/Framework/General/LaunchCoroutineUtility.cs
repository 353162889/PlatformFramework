using System;
using UnityEngine;
using System.Collections;

namespace Launch
{
    public class LaunchCoroutineUtility : MonoBehaviour
    {
        public Coroutine WaitAndExecute(float delay, Action action)
        {
            return base.StartCoroutine(this.iWaitAndExecute(delay, action));
        }

        public Coroutine WaitOneFrame(Action action)
        {
            return base.StartCoroutine(this.iWaitOneFrame(action));
        }

        public Coroutine WaitFramesAndExecute(int count, Action action)
        {
            return base.StartCoroutine(this.iWaitFramesAndExecute(count, action));
        }

        private IEnumerator iWaitAndExecute(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action();
        }

        private IEnumerator iWaitOneFrame(Action action)
        {
            yield return new WaitForEndOfFrame();
            action();
        }

        private IEnumerator iWaitFramesAndExecute(int count, Action action)
        {
            for (int i = 0; i < count; ++i)
            {
                yield return new WaitForEndOfFrame();
            }
            action();
        }
    }
}

