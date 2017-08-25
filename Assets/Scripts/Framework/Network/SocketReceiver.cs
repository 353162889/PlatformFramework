using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Launch
{
    public class SocketReceiver
    {
        private SocketClient _socketClient;
        private Queue<MsgPacket> _queuePacket;
        private Queue<MsgPacket> _queuePacketHandler;
        private Thread _thread;
        private Stream _stream;
        private byte[] packetHeadBuff;
        private byte[] zeroBuff;
        private byte[] packetLengthBuff;
        private byte[] packetIDBuff;
        private byte[] packetStatusBuff;
       
        public SocketReceiver(SocketClient socketClient)
        {
            _socketClient = socketClient;
            _queuePacket = new Queue<MsgPacket>();
            _queuePacketHandler = new Queue<MsgPacket>();
            _stream = new MemoryStream();
            packetHeadBuff = new byte[MsgPacket.PacketHeadSize];
            zeroBuff = new byte[0];
            packetLengthBuff = new byte[MsgPacket.PacketLengthBit];
            packetIDBuff = new byte[MsgPacket.PacketIDBit];
            packetStatusBuff = new byte[MsgPacket.PacketStatusBit];
            _thread = new Thread(new ThreadStart(Run));
            _thread.IsBackground = true;
            _thread.Start();
        }

        private void Run()
        {
            while(true)
            {
                Socket socket = _socketClient.Socket;
                try
                {
                    //获取到数据包头
                    int headCount = socket.Receive(packetHeadBuff, packetHeadBuff.Length, SocketFlags.None);
                    if(headCount == 0)
                    {
                        SocketTools.LogError("SocketClientType:" + _socketClient.SocketClientType + "\n" + "receive headData is 0");
                        LostConnect();
                        break;
                    }
                    else
                    {
                        MsgPacket packet = new MsgPacket(packetHeadBuff, packetLengthBuff, packetIDBuff, packetStatusBuff);
                        int bodyLength = packet.Length - MsgPacket.PacketHeadSize;
                        if(bodyLength < 0)
                        {
                            SocketTools.LogError("SocketClientType:" + _socketClient.SocketClientType + "\n" + "packet body data is <0");
                            LostConnect();
                            break;
                        }
                        byte[] buff = zeroBuff;
                        if (bodyLength != 0)
                        {
                            buff = new byte[bodyLength];
                            int bodyCount = socket.Receive(buff, buff.Length, SocketFlags.None);
                            if (bodyCount == 0)
                            {
                                SocketTools.LogError("SocketClientType:" + _socketClient.SocketClientType + "\n" + "receive bodyData is 0");
                                LostConnect();
                                break;
                            }
                        }
                        Type type = _socketClient.SocketClientMgr.GetProtoType(packet.ID);
                        packet.Deserialize(type,buff, _stream);
                        lock(_queuePacket)
                        {
                            _queuePacket.Enqueue(packet);
                        }
                    }
                }
                catch(Exception e)
                {
                    SocketTools.LogError("SocketClientType:"+_socketClient.SocketClientType + "\n"+e.ToString());
                    LostConnect();
                    break;
                }
                
            }
        }

        private void LostConnect()
        {
            lock (_queuePacket)
            {
                _queuePacket.Enqueue(null);
            }
        }

        /// <summary>
        /// 主线程中调用
        /// </summary>
        public void Update()
        {
            lock(this._queuePacket)
            {
                while(_queuePacket.Count > 0)
                {
                    _queuePacketHandler.Enqueue(_queuePacket.Dequeue());
                }
            }
            while(_queuePacketHandler.Count > 0)
            {
                MsgPacket packet = _queuePacketHandler.Dequeue();
                if (packet != null)
                {
                    _socketClient.HandlePacket(packet);
                }
                else
                {
                    _socketClient.Disconnect();
                }
            }
        }

        public void Close()
        {
            _socketClient = null;
            if(_thread != null)
            {
                _thread.Abort();
                _thread = null;
            }
        }
    }
}
