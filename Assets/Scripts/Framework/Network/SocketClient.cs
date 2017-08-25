using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System;

namespace Launch
{
    public enum SocketStatus
    {
        DisConnected,   //没有连接
        Connected,      //已连接
        Connecting      //连接中
    }

    public enum SocketClientType
    {
        Game
    }
    public class SocketClient
    {
        private SocketStatus _status;
        public SocketStatus Status { get { return _status; } private set {
                lock(lockObj)
                {
                    _status = value;
                }
            } }
        public SocketClientType SocketClientType { get; private set; }
        private volatile Socket _socket;
        public Socket Socket { get { return _socket; } }
        private SocketReceiver _receiver;
        private SocketSender _sender;
        private Action<bool> _callback;
        private Coroutine _connectCoroutine;
        private IAsyncResult _asyncResult;
        private System.Object lockObj = new System.Object();
        private Action<SocketClientType, MsgPacket> _packetHandler;
        private SocketClientMgr _mgr;
        public SocketClientMgr SocketClientMgr { get{ return _mgr; } }

        public SocketClient(SocketClientMgr mgr, SocketClientType socketClientType,Action<SocketClientType,MsgPacket> packetHandler = null)
        {
            Status = SocketStatus.DisConnected;
            this._mgr = mgr;
            this.SocketClientType = socketClientType;
            this._packetHandler = packetHandler;
        }

        public bool BeginConnect(string ip,int port,Action<bool> callback = null)
        {
            if(Status == SocketStatus.Connected)
            {
                SocketTools.LogError("socket is connected!");
                return false;
            }
            this.Close();
            IPEndPoint ipEndPoint;
            if(CheckEndToPoint(ip,port,out ipEndPoint))
            {
                _callback = callback;
                _connectCoroutine = this.SocketClientMgr.StartCoroutine(ConnectSocket(ipEndPoint));
                return true;
            }
            else
            {
                SocketTools.LogError("ip "+ ip +" is not correct format!");
                return false;
            }
        }

        private IEnumerator ConnectSocket(EndPoint endPoint)
        {
            _socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Status = SocketStatus.Connecting;
            _asyncResult = _socket.BeginConnect(endPoint, AsyncCallback, _socket);
            float curTime = 0f;
            float totalTime = 10f;
            while(curTime < totalTime && !_asyncResult.IsCompleted)
            {
                curTime += Time.deltaTime;
                yield return null;
            }
            bool finish = true;
            try
            {
                _socket.EndConnect(_asyncResult);
                _asyncResult = null;
            }
            catch(Exception e)
            {
                SocketTools.Log("SocketClientType:" + SocketClientType + "连接超时"+"\n"+e.ToString());
                finish = false;
            }
            finally
            {
                _asyncResult = null;
            }
            try
            {
                if (finish)
                {
                    Status = SocketStatus.Connected;
                    _receiver = new SocketReceiver(this);
                    _sender = new SocketSender(this);
                }
                else
                {
                    Status = SocketStatus.DisConnected;
                }
            }
            catch(Exception e)
            {
                SocketTools.LogError("SocketClientType:" + SocketClientType + "\n" + e.ToString());
                finish = false;
            }
            finally
            {
                if (_callback != null)
                {
                    Action<bool> temp = _callback;
                    _callback = null;
                    temp.Invoke(finish);
                }
            }
            yield return null;
        }

        private void AsyncCallback(IAsyncResult ar)
        {
            SocketTools.Log("AsyncCallback");
        }

        /// <summary>
        /// 主线程中调用
        /// </summary>
        public void Update()
        {
            if(_receiver != null)
            {
                _receiver.Update();
            }
            if(_sender != null)
            {
                _sender.Update();
            }
        }

        /// <summary>
        /// 主线程中调用
        /// </summary>
        /// <param name="packet"></param>
        public void HandlePacket(MsgPacket packet)
        {
            if(_packetHandler != null)
            {
                _packetHandler.Invoke(this.SocketClientType,packet);
            }
        }

        public void SendPacket(MsgPacket packet)
        {
            this._sender.SendMsg(packet);
        }

        //这个再主线程调用
        public void Disconnect()
        {
            //可以抛出事件
            SocketTools.Log("socketClient:"+SocketClientType+",Disconnect");
            this.Close();
        }

        public void Close()
        {
           
            lock (lockObj)
            {
                _callback = null;
                if (_connectCoroutine != null)
                {
                    this.SocketClientMgr.StopCoroutine(_connectCoroutine);
                    _connectCoroutine = null;
                }
                if (_socket != null)
                {
                    try
                    {
                        if (_asyncResult != null)
                        {
                            _socket.EndConnect(_asyncResult);
                            _asyncResult = null;
                        }
                        if (Status == SocketStatus.Connected)
                        {
                            _socket.Shutdown(SocketShutdown.Both);
                        }

                    }
                    catch (Exception e)
                    {
                        SocketTools.LogError(e.ToString());
                    }
                    finally
                    {
                        try
                        {
                            _socket.Close();
                        }
                        catch (Exception ex)
                        {
                            SocketTools.LogError(ex.ToString());
                        }
                        finally
                        {
                            _socket = null;
                            Status = SocketStatus.DisConnected;
                        }
                    }
                }
                if (_receiver != null)
                {
                    _receiver.Close();
                    _receiver = null;
                }
                if (_sender != null)
                {
                    _sender.Close();
                    _sender = null;
                }
            }
        }

        private bool CheckEndToPoint(string ip, int port, out IPEndPoint point)
        {
            point = null;
            IPAddress ipAddress;
            if (IPAddress.TryParse(ip, out ipAddress))
            {
                point = new IPEndPoint(ipAddress, port);
                return true;
            }
            return false;
        }
    }
}