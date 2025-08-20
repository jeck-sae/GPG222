using System.IO;

public class PlayerLeftPacket : BasePacket
{
    public string playerId;
    public PlayerLeftPacket() { type = PacketType.PlayerLeft; }
    public PlayerLeftPacket(string id) { type = PacketType.PlayerLeft; playerId = id; }


    public override byte[] Serialize()
    {
        using MemoryStream ms = new MemoryStream();
        using BinaryWriter bw = new BinaryWriter(ms);
        bw.Write((int)type);
        bw.Write(playerId);
        return ms.ToArray();

    }

    public override void Deserialize(BinaryReader br)
    {
        playerId = br.ReadString();
    }
}
