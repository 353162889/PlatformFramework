using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Net;
using System.IO;

namespace Launch
{
	public class LaunchDownloadThread 
	{	
		public readonly string TEMP_SUFFIX = ".temp";
		private readonly int TIME_OUT = 10000;

		private bool _isStop;
		private Thread _thread;
		private Queue<LaunchDownloadTask> _pendingTasks;
		private Queue<LaunchDownloadTask> _finishedTasks;
		private volatile bool _isWaitting;

		private LaunchDownloadTask _curTask;

		private string _curFile;
		public string CurFile{get{ return _curFile;}}
		private long _receivedLength;
		public long CurReceivedLength{ get{return _receivedLength; }}
		private long _totalLength;
		public long CurTotalLength{get{ return _totalLength;}}
        private long _versionCode;

        public LaunchDownloadThread(Queue<LaunchDownloadTask> comsumeQueue,Queue<LaunchDownloadTask> produceQueue)
		{
			_curFile = "";
			_receivedLength = 0;
			_totalLength = 1;
			_pendingTasks = comsumeQueue;
			_finishedTasks = produceQueue;
			_isWaitting = false;
            _isStop = true;
		}

        public void SetVersionCode(long versionCode)
        {
            _versionCode = versionCode;
        }

		public bool isWaitting
		{
			get
			{
				return _isWaitting;
			}
		}

		public void Start()
		{
			if(!_isStop || _thread != null)
			{
				return;
			}
			_isStop = false;
			_thread = new Thread(new ThreadStart(RunDownloading));
			_thread.Start();
		}

		public void Stop()
		{
			_isStop = true;
			if(_thread == null)
			{
				return;
			}


			lock(_pendingTasks)
			{
				if(this._isWaitting)
				{
					this.Notify();
				}
			}
			_thread = null;
		}

		public void Wait()
		{
			Monitor.Wait(_pendingTasks);
		}

		public void Notify()
		{
			Monitor.Pulse(_pendingTasks);
		}

		private void RunDownloading()
		{
			for(;;)
			{
				if(_isStop)
				{
					break;
				}
				if(_curTask == null)
				{			
					lock(_pendingTasks)
					{
						int num = _pendingTasks.Count;
						if(num > 0)
						{
							_curTask = _pendingTasks.Dequeue();
						}
						else
						{
							_isWaitting = true;
							this.Wait();
							_isWaitting = false;
						}
					}
				}


				if(!_isStop && _curTask != null)
				{
                    //Debug.Log("StartDownload");
                    Download(_curTask);
                    //Debug.Log("EndDownload");
                }
			}
		}

		private void Download(LaunchDownloadTask task)
		{
			try
			{
				Uri uri = new Uri(task.url);
				_curFile = task.file;
//				Debug.Log("Download1");
				HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri);
//				Debug.Log("Download2");
				request.Timeout = TIME_OUT;
				request.ReadWriteTimeout = TIME_OUT;
				HttpWebResponse response = (HttpWebResponse) request.GetResponse();
//				Debug.Log("Download3");
				long totalLength = response.ContentLength;
				response.Close();
				request.Abort();

				long receivedLength = 0L;
				long toDownloadLength = totalLength;

				string tempFileName = GetTempCacheName(task.localPath);
				if(File.Exists(tempFileName))
				{
					using(FileStream fileStream = new FileStream(tempFileName,FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.ReadWrite))
					{
						receivedLength = fileStream.Length;
						toDownloadLength = totalLength - receivedLength;
						fileStream.Close();
					}
				}
				task.fileLength = totalLength;
				task.receivedLength = receivedLength;
				_totalLength = totalLength;
				_receivedLength = receivedLength;

				HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(uri);
				request.Timeout = TIME_OUT;
				request2.ReadWriteTimeout = TIME_OUT;
				bool transferOkay = true;
				if(toDownloadLength > 0L)
				{	
					request2.AddRange((int)receivedLength,(int)totalLength);
					HttpWebResponse response2 = (HttpWebResponse) request2.GetResponse();
					transferOkay = this.ReadBytesFromResponse(task,response2);
					response2.Close();
				}
				request2.Abort();
				if(transferOkay) 
				{
					this.OnDownloadFinished(task,null);
				}
			}
			catch(Exception ex)
			{
				this.OnDownloadFinished(task,ex);
			}
		}

		private bool ReadBytesFromResponse(LaunchDownloadTask task,WebResponse response)
		{
			bool okay= false;
			try
			{
				int toDownloadLength = (int)(task.fileLength - task.receivedLength);
				byte[] array = new byte[toDownloadLength];
				int bytesReceived = 0;
				using(Stream responseStream = response.GetResponseStream())
				{
					int bytesRead;
					string temp = GetTempCacheName(task.localPath);
					CheckDirExistsForFile(temp);
					do
					{
						bytesRead = responseStream.Read(array,bytesReceived,toDownloadLength-bytesReceived);
						if(bytesRead > 0)
						{
							byte[] array2 = new byte[bytesRead];
							Buffer.BlockCopy(array,bytesReceived,array2,0,array2.Length);
							using(FileStream fileStream = new FileStream(temp,FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.ReadWrite))
							{
								fileStream.Position = task.receivedLength;
								fileStream.Write(array2,0,array2.Length);
								fileStream.Flush();
								fileStream.Close();
							}

							bytesReceived += bytesRead;
							task.receivedLength += bytesRead;
							_receivedLength =task.receivedLength;
						}
					}while(bytesRead != 0);
					okay = true;
				}
			}
			catch(Exception ex)
			{
				this.OnDownloadFinished(task,ex);
				okay = false;
			}
			return okay;
		}

        public void CheckDirExistsForFile(string file)
        {
            int index = file.LastIndexOf("/");
            string dir = file.Substring(0, index);
            DirectoryInfo info = new DirectoryInfo(dir);
            if (!info.Exists)
            {
                info.Create();
            }
        }

        private void OnDownloadFinished(LaunchDownloadTask task,Exception ex)
		{
//			Debug.Log ("OnDownloadFinished");
			if(ex != null)
			{	
				if(ex.Message.Contains("Sharing violation on path"))
				{
					return;
				}
				LaunchDownloadError error;
				if(ex.Message.Contains("ConnectFailure") || ex.Message.Contains("NameResolutionFailure") || ex.Message.Contains("No route to host"))
				{
					error = LaunchDownloadError.ServerMaintenance;
				}
				else if(ex.Message.Contains("404"))
				{
					error = LaunchDownloadError.NotFound;
				}
				else if(ex.Message.Contains("403"))
				{
					error = LaunchDownloadError.ServerMaintenance;
				}
				else if(ex.Message.Contains("Disk full"))
				{
					error = LaunchDownloadError.DiskFull;
				}
				else if(ex.Message.Contains("time out") || ex.Message.Contains("Error getting response stream"))
				{
					error = LaunchDownloadError.Timeout;
				}
				else
				{
					error = LaunchDownloadError.Unknown;
				}
				NotifyResult(task,LaunchDownloadStatus.Fail,error);	
				Thread.Sleep(1000);  // Downloading thread sleep for 3000ms when a exception occurred in the process of downloading 
			}
			else
			{
				CheckMD5AfterDownload(task);
			}
		}

		private void CheckMD5AfterDownload(LaunchDownloadTask task)
		{
			string temp = GetTempCacheName(task.localPath);
			bool okay = true;
			if(!string.IsNullOrEmpty(task.md5))
			{
				string md5 = LaunchMD5Hash.GetFileMD5(temp);
				if(md5 != task.md5)
				{
					Debug.LogError("Downloaded file with wrong md5 value! FilePath="+task.localPath);
					File.Delete(temp);
					okay = false;  //delete it and download again
				}
			}

			if(okay)
			{
				//if the file downloaded is a packed file by zip,unzip it
				if(task.localPath.EndsWith(".zip"))
				{
                    NotifyResult(task, LaunchDownloadStatus.Fail, LaunchDownloadError.NotSupportZip);
                    Thread.Sleep(3000);
                    //if(unzipPackedFile(task))
                    //{
                    //	NotifyResult(task,LaunchDownloadStatus.Complete);
                    //	_curTask = null;
                    //}
                    //else
                    //{
                    //	NotifyResult(task,LaunchDownloadStatus.Fail,task.errorCode);
                    //	Thread.Sleep(3000);
                    //}
                }
				else
				{
					//set name of downloaded file from  temp to localpath
					if(File.Exists(task.localPath))
					{
						File.Delete(task.localPath);
					}
					File.Move(temp,task.localPath);
					NotifyResult(task,LaunchDownloadStatus.Complete);
					_curTask = null;
				}
			}
		}

		//private bool unzipPackedFile(LaunchDownloadTask task)
		//{
		//	bool isSuccess = true;
		//	string zipTemp = GetTempCacheName(task.localPath);

		//	int index = task.localPath.LastIndexOf("/");
		//	string unzipDir = Path.GetDirectoryName(task.localPath);
		//	DirectoryInfo info = new DirectoryInfo(unzipDir);
		//	if(!info.Exists)
		//	{
		//		info.Create();
		//	}

		//	try
		//	{
		//		using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipTemp)))  
		//		{  
		//			ZipEntry theEntry;  
		//			while ((theEntry = s.GetNextEntry()) != null)  
		//			{  
		//				string directoryName = Path.GetDirectoryName(theEntry.FileName);  
		//				string fileName = Path.GetFileName(theEntry.FileName);
		//				string filePath = Path.Combine(unzipDir,theEntry.FileName);
		//				DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(unzipDir,directoryName));
		//				if(!dirInfo.Exists)
		//				{
		//					dirInfo.Create();
		//				}

		//				if(File.Exists(filePath))
		//				{
		//					File.Delete(filePath);
		//				}

		//				if (fileName != String.Empty)  
		//				{  
		//					using (FileStream streamWriter = File.Create(filePath))
		//					{  
		//						int size = 2048;  
		//						byte[] data = new byte[2048];  
		//						while (true)  
		//						{  
		//							size = s.Read(data, 0, data.Length);  
		//							if (size > 0)  
		//							{  
		//								streamWriter.Write(data, 0, size);  
		//							}  
		//							else  
		//							{  
		//								break;  
		//							}  
		//						}  
		//					}  
		//				}  
		//			}

		//		}
		//	}
		//	catch(Exception ex)
		//	{
		//		if(ex.Message.Contains("Disk full"))
		//		{
		//			task.errorCode = LaunchDownloadError.DiskFull;
		//		}
		//		else
		//		{
		//			task.errorCode = LaunchDownloadError.Unknown;
		//		}
		//		task.status = LaunchDownloadStatus.Fail;
		//		isSuccess = false;
		//	}
		//	return isSuccess;
		//}

		private void NotifyResult(LaunchDownloadTask task,LaunchDownloadStatus status,LaunchDownloadError error = LaunchDownloadError.Unknown)
		{
			task.status = status;
			task.errorCode = error;
			lock(_finishedTasks)
			{
				_finishedTasks.Enqueue(task);
			}
		}


		private string GetTempCacheName(string savePath)
		{
			return savePath+TEMP_SUFFIX+ _versionCode;
		}
	}
}
