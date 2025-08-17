using System.IO;

public class PingPacket : BasePacket
{
    public string ping = "";

    public PingPacket()
    {
        type = PacketType.ping;
    }

    public override byte[] Serialize()
    {
        using MemoryStream ms = new MemoryStream();
        using BinaryWriter bw = new BinaryWriter(ms);
        bw.Write((int)type);
        return ms.ToArray();
    }
   
    public override void Deserialize(BinaryReader br)
    {  
    }
}
