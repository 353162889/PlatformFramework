using System;
using UnityEngine;
using System.Collections;
using Launch;

namespace Launch
{
	public class LaunchWWWUtility : MonoBehaviour
	{
		private Coroutine _coroutine;
		public void WWWRequest(string path,WWWForm form = null,Action<WWW> callback = null)
		{
			_coroutine = StartCoroutine (LoadWWWAsync(path,form,callback));
		}

		IEnumerator LoadWWWAsync(string path,WWWForm form, Action<WWW> callback)
		{
			WWW www;
			if (form != null)
			{
				www = new WWW (path,form);
			}
			else
			{
				www = new WWW (path);
			}
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				Debug.LogError("<color='#ff0000'>Load resource [" + path + "] failed .error:" + www.error + " </color>");
			}
			if (callback != null)
			{
                Action<WWW> temp = callback;
                callback = null;
                temp.Invoke (www);
			}
			_coroutine = null;
			if (www != null)
			{
				www.Dispose ();
				www = null;
			}
		}

        public void WWWRequest(string path,float time, WWWForm form = null, Action<string,WWW> callback = null)
        {
            _coroutine = StartCoroutine(LoadWWWAsync(path,time, form, callback));
        }

        IEnumerator LoadWWWAsync(string path,float time, WWWForm form, Action<string, WWW> callback)
        {
            WWW www;
            if (form != null)
            {
                www = new WWW(path, form);
            }
            else
            {
                www = new WWW(path);
            }
            float curTime = 0f;
            string errorInfo = "";
            while(!www.isDone)
            {
                curTime += Time.deltaTime;
                if(curTime > time)
                {
                    errorInfo = "Time Out";
                    break;
                }
                yield return null;
            }
            if(string.IsNullOrEmpty(errorInfo))
            {
                errorInfo = www.error;
            }
            if (!string.IsNullOrEmpty(errorInfo))
            {
                Debug.LogError("<color='#ff0000'>Load resource [" + path + "] failed .error:" + errorInfo + " </color>");
            }
            if (callback != null)
            {
                Action<string, WWW> temp = callback;
                callback = null;
                temp.Invoke(errorInfo,www);
            }
            _coroutine = null;
            if (www != null)
            {
                www.Dispose();
                www = null;
            }
        }

        public void StopLoadRes()
		{
			if (_coroutine != null)
			{
				StopCoroutine (_coroutine);
				_coroutine = null;
			}
		}
	}
}

