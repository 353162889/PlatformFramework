using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

namespace Launch
{
	public class LaunchDownloadMgr:MonoBehaviour
	{
		public readonly float TICK_INTERVAL = 1f;

		private LaunchDownloadThread _downloadThread;
		private Queue<LaunchDownloadTask> _pendingTasks;
		private Queue<LaunchDownloadTask> _finishedTasks;
		private Queue<LaunchDownloadTask> _tempTasks;

		public string CurFile{get{ return _downloadThread.CurFile;}}
		public long CurReceivedLength{ get{return _downloadThread.CurReceivedLength; }}
		public long CurTotalLength{get{ return _downloadThread.CurTotalLength;}}

        private UpdateScheduler _scheduler;

        private void Awake()
        {
            _pendingTasks = new Queue<LaunchDownloadTask>();
            _finishedTasks = new Queue<LaunchDownloadTask>();
            _tempTasks = new Queue<LaunchDownloadTask>();
            _downloadThread = new LaunchDownloadThread(_pendingTasks, _finishedTasks);
            _scheduler = gameObject.AddComponentOnce<UpdateScheduler>();
        }

        private void OnDestroy()
        {
            StopService();
        }

        //先设置版本号
        public void SetVersionCode(long versionCode)
        {
            _downloadThread.SetVersionCode(versionCode);
        }

		public void StartService()
		{
            if (_scheduler.HasScheduler(Tick)) return;
            _downloadThread.Start();
            _scheduler.AddScheduler(Tick, TICK_INTERVAL);
		}

		public void StopService()
		{
            _scheduler.RemoveScheduler(Tick);
            _downloadThread.Stop();
		}

		private void Tick(float time)
		{
			_tempTasks.Clear ();
			lock(_finishedTasks)
			{
				while (_finishedTasks.Count > 0)
				{
					_tempTasks.Enqueue (_finishedTasks.Dequeue());
				}
			}
            //LH.Log("Tick,tempTasks:"+_tempTasks.Count);
			while (_tempTasks.Count > 0)
			{
				LaunchDownloadTask task = _tempTasks.Dequeue ();
				if(task != null)
				{
					task.onFinish(task);
				}
			}
		}

		public void AsyncDownloadList(List<string> list,string remoteDir,string localDir,Action<LaunchDownloadTask> onFinish,List<string> exts = null)
		{
			lock(_pendingTasks)
			{
				int count = list.Count;
				string url;
				for(int i = 0; i<count; ++i)
				{
					url = list[i];
					LaunchDownloadTask task = new LaunchDownloadTask();
					task.file = url;
					if(exts != null && exts.Count > i)
						url += exts[i];
					task.url = remoteDir+url;
					task.localPath = localDir+url;
					task.onFinish = onFinish;
					_pendingTasks.Enqueue(task);
//					Debug.Log ("<color='#ff0000'>[正在下载文件: " + task.url + "</color>");
				}

				if(_downloadThread.isWaitting)
				{
					_downloadThread.Notify();	
				}
			}
		}

		public void AsyncDownload(string url,string localPath,Action<LaunchDownloadTask> onFinish)
		{
			LaunchDownloadTask task = new LaunchDownloadTask();
			task.url = url;
			task.localPath = localPath;
			task.onFinish = onFinish;

			EnqueueTask(task);
		}

		private void EnqueueTask(LaunchDownloadTask task)
		{
			lock(_pendingTasks)
			{
				_pendingTasks.Enqueue(task);

				if(_downloadThread.isWaitting)
				{
					_downloadThread.Notify();	
				}
			}
		}
	}

}
