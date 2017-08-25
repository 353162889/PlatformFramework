using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Launch
{
    public class SocketSender
    {
        private SocketClient _socketClient;
        private Thread _thread;
        private Queue<MsgPacket> _queuePacket;
        private bool _lostConnect;
        private System.Object lockObj = new System.Object();
        private Stream _stream;
        public SocketSender(SocketClient socketClient)
        {
            _socketClient = socketClient;
            _lostConnect = false;
            _queuePacket = new Queue<MsgPacket>();
            _stream = new MemoryStream();
            _thread = new Thread(new ThreadStart(Run));
            _thread.IsBackground = true;
            _thread.Start();
        }

        private void Run()
        {
            while(true)
            {
                if(_queuePacket.Count == 0)
                {
                    Thread.Sleep(1);
                    continue;
                }
                MsgPacket packet;
                lock(_queuePacket)
                {
                    packet = _queuePacket.Dequeue();
                }
                try
                {
                    Socket socket = _socketClient.Socket;
                    byte[] sendBytes = packet.Serialize(_stream);
                    int count = socket.Send(sendBytes, sendBytes.Length, SocketFlags.None);
                    if (count == 0)
                    {
                        SocketTools.LogError("SocketClientType:" + _socketClient.SocketClientType + "\n" + "socket send packet count is 0");
                        LostConnect();
                        break;
                    }
                }
                catch(Exception e)
                {
                    SocketTools.LogError("SocketClientType:" + _socketClient.SocketClientType + "\n" + e.ToString());
                    LostConnect();
                    break;
                }
            }
        }

        public void LostConnect()
        {
            lock(lockObj)
            {
                _lostConnect = true;
            }
        }

        /// <summary>
        /// 主线程中调用
        /// </summary>
        public void Update()
        {
            lock (lockObj)
            {
                if(_lostConnect)
                {
                    _socketClient.Disconnect();
                }
            }
        }

        public void SendMsg(MsgPacket packet)
        {
            lock(_queuePacket)
            {
                _queuePacket.Enqueue(packet);
            }
        }

        public void Close()
        {
            _socketClient = null;
            _stream = null;
            if(_thread != null)
            {
                _thread.Abort();
                _thread = null;
            }
        }
    }
}
