using System;
using System.IO;

namespace Launch
{
    public class MsgPacket
    {
        private static byte[] Buff0 = new byte[0];
        public static int PacketLengthBit = 4;
        public static int PacketIDBit = 2;
        public static int PacketStatusBit = 2;
        public static int PacketHeadSize = PacketLengthBit + PacketIDBit + PacketStatusBit;
        public int Length { get ; private set; }
        public ushort ID { get; private set; }
        public ushort Status { get; private set; }
        public byte[] Buff { get; private set; }
        private object _protoBufObj;
        public object ProtoBufObj { get {
                if(_protoBufObj == null)
                {
                    SocketTools.Log("ProtoBufObj 没有注册异步解析类型，无法反序列化当前proBuf对象，请用Buff自己序列化(或当前注册type无法反序列化)");
                }
                return _protoBufObj;
            } private set { _protoBufObj = value; } }

        public MsgPacket(byte[] headBytes,byte[] lengthBytes,byte[] idBytes,byte[] statusBytes)
        {
            MemoryStream stream = new MemoryStream(headBytes);
            stream.Position = 0;
            stream.Read(lengthBytes, 0, lengthBytes.Length);
            Length = BitConverter.ToInt32(lengthBytes, 0);
            stream.Read(idBytes, 0, idBytes.Length);
            ID = BitConverter.ToUInt16(idBytes,0);
            stream.Read(statusBytes, 0, statusBytes.Length);
            Status = BitConverter.ToUInt16(statusBytes,0);
        }

        //发送时初始化
        public MsgPacket(ushort ID,byte[] buff)
        {
            this.ID = ID;
            this.Status = 0;
            this.Buff = buff;
        }

        //发送时初始化
        public MsgPacket(ushort ID,object protoBuff)
        {
            this.ID = ID;
            this.Status = 0;
            this.ProtoBufObj = protoBuff;
        }

        public byte[] Serialize(Stream stream)
        {
            stream.Position = 0;
            if(this.Buff == null)
            {
                if(this._protoBufObj != null)
                {
                    ProtoBuf.Serializer.NonGeneric.Serialize(stream, this._protoBufObj);
                    long count = stream.Position;
                    this.Buff = new byte[count];
                    stream.Position = 0;
                    stream.Read(this.Buff, 0, this.Buff.Length);
                }
                else
                {
                    this.Buff = Buff0;
                }
            }
            this.Length = this.Buff.Length + PacketHeadSize;
            byte[] result = new byte[this.Length];

            stream.Position = 0;
            byte[] lengthBytes = BitConverter.GetBytes(Length);
            stream.Write(lengthBytes,0,lengthBytes.Length);
            byte[] idBytes = BitConverter.GetBytes(ID);
            stream.Write(idBytes, 0, idBytes.Length);
            byte[] statusBytes = BitConverter.GetBytes(Status);
            stream.Write(statusBytes, 0, statusBytes.Length);
            stream.Write(this.Buff, 0, this.Buff.Length);

            stream.Position = 0;
            stream.Read(result,0,result.Length);
            return result;
        }

        public void Deserialize(Type type, byte[] buff,Stream stream)
        {
            this.Buff = buff;
            if (type != null && ProtoBuf.Serializer.NonGeneric.CanSerialize(type))
            {
                try
                {
                    stream.Position = 0;
                    stream.Write(buff, 0, buff.Length);
                    //如果有注册
                    stream.Position = 0;
                    this._protoBufObj = ProtoBuf.Serializer.NonGeneric.Deserialize(type, stream);
                }
                catch(Exception e)
                {
                    SocketTools.LogError("Deserialize fail,can not match type:" + type+" and PacketID:"+this.ID);
                }
               
            }
        }
    }
}
