using System.IO;

public class MovePacket : BasePacket
{
    public string playerId;
    public float x;
    public float y;

    public MovePacket()
    {
        type = PacketType.Move;
    }

    public MovePacket(string playerId, float x, float y)
    {
        this.playerId = playerId;
        this.x = x;
        this.y = y;
        this.type = PacketType.Move;
    }

    public override byte[] Serialize()
    {
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter bw = new BinaryWriter(ms))
        {
            bw.Write((int)type);       // Write packet type
            bw.Write(playerId);        // Write player ID
            bw.Write(x);               // Write x
            bw.Write(y);               // Write y
            return ms.ToArray();
        }
    }

    public override void Deserialize(BinaryReader br)
    {
        playerId = br.ReadString();
        x = br.ReadSingle();
        y = br.ReadSingle();
    }
}
