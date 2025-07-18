using System.IO;

public class LoadLevelPacket : BasePacket
{
    public int levelId;

    public LoadLevelPacket()
    {
        type = PacketType.LoadLevel;
    }

    public LoadLevelPacket(int levelId)
    {
        this.levelId = levelId;
        type = PacketType.LoadLevel;
    }

    public override byte[] Serialize()
    {
        using MemoryStream ms = new MemoryStream();
        using BinaryWriter bw = new BinaryWriter(ms);
        bw.Write((int)type);
        bw.Write(levelId);
        return ms.ToArray();
    }

    public override void Deserialize(BinaryReader br)
    {
        levelId = br.ReadInt32();
    }
}
