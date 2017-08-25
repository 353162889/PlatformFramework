using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Launch
{
    public class SingletonContainer : MonoBehaviour
    {
        public LuaMultiClient LuaClient { get; private set; }
        public LaunchCoroutineUtility CoroutineUtiltity { get; private set; }
        public LaunchWWWUtility WWWUtility { get; private set; }
        public UpdateScheduler Scheduler { get; private set; }
        public ResourceMgr ResourceMgr { get; private set; }
        public AssetBundleMgr AssetBundleMgr { get; private set; }
        public SocketClientMgr SocketClientMgr { get; private set; }

        void Awake()
        {
            CoroutineUtiltity = gameObject.AddComponentOnce<LaunchCoroutineUtility>();
            WWWUtility = gameObject.AddComponentOnce<LaunchWWWUtility>();
            Scheduler = gameObject.AddComponentOnce<UpdateScheduler>();
            this.ResourceMgr = gameObject.AddComponentOnce<ResourceMgr>();
            this.AssetBundleMgr = gameObject.AddComponentOnce<AssetBundleMgr>();
            LuaClient = gameObject.AddComponentOnce<LuaMultiClient>();
            this.SocketClientMgr = gameObject.AddComponentOnce<SocketClientMgr>();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
