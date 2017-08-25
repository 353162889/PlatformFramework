using UnityEngine;
using System.Collections.Generic;
using System;

namespace Launch
{
	public enum LaunchDownloadStatus
	{
		Init,
		Fail,
		Complete
	}

	public enum LaunchDownloadError
	{	
		NetworkDisconnect,
		NotFound,
		ServerMaintenance,
		DiskFull,
		Timeout,
		Unknown,
        NotSupportZip,
	}

	public class LaunchDownloadTask
	{ 
		public string file;
		//address
		public string url;      
		public string localPath; 
		public string md5;

		//callback
		//public Action onProgress;
		public Action<LaunchDownloadTask> onFinish;

		//length 
		public long fileLength;
		public long receivedLength;

		//
		public LaunchDownloadStatus  status;   //0 success 1 error 
		public LaunchDownloadError   errorCode;
	}
}
