using System;
using System.Collections.Generic;
using UnityEngine;

namespace Launch
{
    public class SocketClientMgr : MonoBehaviour
    {
        public delegate void MsgPacketCallback(MsgPacket packet);
        private System.Object lockObj = new System.Object();
        private Dictionary<SocketClientType, SocketClient> _map = new Dictionary<SocketClientType, SocketClient>();
        private Dictionary<ushort, Type> _mapProto = new Dictionary<ushort, Type>();
        private Dictionary<SocketClientType, Dictionary<ushort, List<MsgPacketCallback>>> _mapListener = new Dictionary<SocketClientType, Dictionary<ushort, List<MsgPacketCallback>>>();

        private void OnDestroy()
        {
            Dispose();
        }


        private void Update()
        {
            var v = _map.GetEnumerator();
            while(v.MoveNext())
            {
                v.Current.Value.Update();
            }
        }

        private void PacketHandler(SocketClientType socketClientType,MsgPacket packet)
        {
            Dictionary<ushort, List<MsgPacketCallback>> map;
            _mapListener.TryGetValue(socketClientType,out map);
            if (map == null) return;
            List<MsgPacketCallback> list;
            map.TryGetValue(packet.ID, out list);
            if (list == null) return;
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Invoke(packet);
            }
            //SocketTools.Log(string.Format("receive,packetID:{0} packetStatus:{1} packetData:{2}", packet.ID, packet.Status, ((Msg.Info)packet.ProtoBufObj).msg));
        }

        public void BeginConnect(string ip,int port,SocketClientType socketClientType,Action<bool> callback = null)
        {
            SocketClient socketClient;
            _map.TryGetValue(socketClientType, out socketClient);
            if(socketClient == null)
            {
                socketClient = new SocketClient(this,socketClientType, PacketHandler);
                _map.Add(socketClientType, socketClient);
            }
            socketClient.BeginConnect(ip, port, callback);
        }

        public void DisConnect(SocketClientType socketClientType)
        {
            SocketClient socketClient;
            _map.TryGetValue(socketClientType, out socketClient);
            if (socketClient != null)
            {
                socketClient.Disconnect();
            }
        }

        public void RegisterListener(SocketClientType socketClientType, ushort ID, MsgPacketCallback callback)
        {
            Dictionary<ushort, List<MsgPacketCallback>> map;
            _mapListener.TryGetValue(socketClientType, out map);
            if(map == null)
            {
                map = new Dictionary<ushort, List<MsgPacketCallback>>();
                _mapListener.Add(socketClientType, map);
            }
            List<MsgPacketCallback> list;
            map.TryGetValue(ID, out list);
            if(list == null)
            {
                list = new List<MsgPacketCallback>();
                map.Add(ID,list);
            }
            if(!list.Contains(callback))
            {
                list.Add(callback);
            }
        }

        //public bool UnRegisterListener(SocketClientType socketClientType,ushort ID, MsgPacketCallback callback)
        //{
        //    Dictionary<ushort, List<MsgPacketCallback>> map;
        //    _mapListener.TryGetValue(socketClientType, out map);
        //    if (map == null) return false;
        //    List<MsgPacketCallback> list;
        //    map.TryGetValue(ID, out list);
        //    if (list == null) return false;
        //    return list.Remove(callback);
        //}

        public void SendMsg(SocketClientType socketClientType,ushort ID,object protoBuf)
        {
            SocketClient socketClient;
            _map.TryGetValue(socketClientType, out socketClient);
            if(socketClient != null && socketClient.Status == SocketStatus.Connected)
            {
                MsgPacket packet = new MsgPacket(ID, protoBuf);
                socketClient.SendPacket(packet);
            }
            else
            {
                SocketTools.LogError("socketClientType:"+socketClientType+" is not connected!");
            }
        }

        public void SendMsg(SocketClientType socketClientType,ushort ID,byte[] buff)
        {
            SocketClient socketClient;
            _map.TryGetValue(socketClientType, out socketClient);
            if (socketClient != null && socketClient.Status == SocketStatus.Connected)
            {
                MsgPacket packet = new MsgPacket(ID, buff);
                socketClient.SendPacket(packet);
            }
            else
            {
                SocketTools.LogError("socketClientType:" + socketClientType + " is not connected!");
            }
        }

        public void RegisterProtoType(ushort ID ,Type type)
        {
            lock(lockObj)
            {
                if(_mapProto.ContainsKey(ID))
                {
                    SocketTools.Log("has register protoType!ID:"+ID+",Type:"+type);
                }
                else
                {
                    _mapProto.Add(ID, type);
                }
            }
        }

        public Type GetProtoType(ushort ID)
        {
            lock(lockObj)
            {
                Type type;
                _mapProto.TryGetValue(ID, out type);
                return type;
            }
        }

        public void Dispose()
        {
            foreach (var item in _map)
            {
                item.Value.Disconnect();
            }
            _map.Clear();
            _mapProto.Clear();
        }

        private void OnApplicationQuit()
        {
            Dispose();
        }
    }
}
