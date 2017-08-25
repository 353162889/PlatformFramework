using System;
using UnityEngine;
using System.Collections;


namespace Launch
{
    public class StatisticsMgr : MonoBehaviour
    {
        private string _userId = "";
        private string _userName = "";
        private string _url;
        private bool _isOpen = false;
        private string _preErrorMsg;
        private string _platform;

        public void SetUserId(string userId, string userName)
        {
            _userId = userId;
            _userName = userName;
            PlayerPrefs.SetString("LastUserId", _userId.ToString());
            PlayerPrefs.SetString("LastUserName", _userName);
            PlayerPrefs.Save();
        }

        public void SetUrl(string url)
        {
            _url = url;
        }

        public void SetIsOpen(bool isOpen)
        {
            _isOpen = isOpen;
        }

        public StatisticsMgr()
        {
            _userId = PlayerPrefs.GetString("LastUserId", "unknown");
            _userName = PlayerPrefs.GetString("LastUserName", "unknown");
        }

        public void RegisterErrorLog()
        {
            if (Application.isMobilePlatform || _isOpen)
            {
                Application.logMessageReceived += OnUnityLog;
            }
        }

        private void OnUnityLog(string message, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                string errorMsg = message + "\n" + stackTrace;
                SendErrorLog(errorMsg);
            }
        }

        private void SendErrorLog(string errorMsg)
        {
            if(string.IsNullOrEmpty(_preErrorMsg) || (string.IsNullOrEmpty(_preErrorMsg) && errorMsg.Length != _preErrorMsg.Length))
            {
                CLog.logRecorder.LogError(errorMsg);

                if (!string.IsNullOrEmpty(_url))
                {
                    WWWForm form = new WWWForm();
                    string result = string.Format("userId={0},userName={1}\n{2}", _userId, _userName, errorMsg);
                    form.AddField("data", result);
                    StartCoroutine(SendErrorLog(_url,form));
                }
            }
        }

        private IEnumerator SendErrorLog(string url,WWWForm form)
        {
            WWW www = new WWW(_url, form);
            yield return www;
            www.Dispose();
        }
    }
}
