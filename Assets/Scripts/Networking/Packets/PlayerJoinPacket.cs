using System.IO;

public class PlayerJoinPacket : BasePacket
{
    public string playerId;
    public int spriteId;
    public string playerName;

    public PlayerJoinPacket()
    {
        type = PacketType.PlayerJoin;
    }

    public PlayerJoinPacket(string playerId, int spriteId, string playerName)
    {
        this.playerId = playerId;
        this.spriteId = spriteId;
        this.playerName = playerName;
        this.type = PacketType.PlayerJoin;
    }

    public override byte[] Serialize()
    {
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter bw = new BinaryWriter(ms))
        {
            bw.Write((int)type);           // Write packet type
            bw.Write(playerId);           // Player's unique ID
            bw.Write(spriteId);           // Sprite index
            bw.Write(playerName);         // Player's display name
            return ms.ToArray();
        }
    }

    public override void Deserialize(BinaryReader br)
    {
        playerId = br.ReadString();
        spriteId = br.ReadInt32();
        playerName = br.ReadString();
    }
}
