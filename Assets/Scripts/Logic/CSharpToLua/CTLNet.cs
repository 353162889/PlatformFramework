using LuaInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Launch
{
    public static class CTLNet
    {
        public delegate void NetMsgHander(int msgId, int status, LuaByteBuffer data);

        public static void RegisterNetMsg(int gameId,int msgId,NetMsgHander handler)
        {
            GameRunner gameRunner;
            CTLTools.TryGameRunner(gameId, out gameRunner);
            if(gameRunner != null)
            {
                gameRunner.STContainer.SocketClientMgr.RegisterListener(SocketClientType.Game,(ushort)msgId, (MsgPacket packet) => {
                    handler.Invoke((int)packet.ID, (int)packet.Status, new LuaByteBuffer(packet.Buff));
                });
            }
        }

        public static void SendMsg(int gameId,int msgId, LuaByteBuffer data)
        {
            GameRunner gameRunner;
            CTLTools.TryGameRunner(gameId, out gameRunner);
            if (gameRunner != null)
            {
                gameRunner.STContainer.SocketClientMgr.SendMsg(SocketClientType.Game, (ushort)msgId, data.buffer);
            }
        }

        public static void ConnectServer(int gameId,string ip,int port,Action<bool> callback)
        {
            GameRunner gameRunner;
            CTLTools.TryGameRunner(gameId, out gameRunner);
            if (gameRunner != null)
            {
                gameRunner.STContainer.SocketClientMgr.BeginConnect(ip,port,SocketClientType.Game, callback);
            }
        }
    }
}
