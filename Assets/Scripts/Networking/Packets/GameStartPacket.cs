using System.IO;

public class GameStartPacket : BasePacket
{
    public GameStartPacket()
    {
        type = PacketType.GameStart;
    }

    public override byte[] Serialize()
    {
        using MemoryStream ms = new MemoryStream();
        using BinaryWriter bw = new BinaryWriter(ms);
        bw.Write((int)type); // Only packet type needed
        return ms.ToArray();
    }

    public override void Deserialize(BinaryReader br)
    {
        
    }
}
