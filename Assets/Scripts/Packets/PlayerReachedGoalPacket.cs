using System.IO;

public class PlayerReachedGoalPacket : BasePacket
{
    public string playerId;
    public float timeTaken;

    public PlayerReachedGoalPacket()
    {
        type = PacketType.PlayerReachedGoal;
    }

    public PlayerReachedGoalPacket(string playerId, float timeTaken)
    {
        this.playerId = playerId;
        this.timeTaken = timeTaken;
        this.type = PacketType.PlayerReachedGoal;
    }

    public override byte[] Serialize()
    {
        using MemoryStream ms = new MemoryStream();
        using BinaryWriter bw = new BinaryWriter(ms);
        bw.Write((int)type);
        bw.Write(playerId);
        bw.Write(timeTaken);
        return ms.ToArray();
    }

    public override void Deserialize(BinaryReader br)
    {
        playerId = br.ReadString();
        timeTaken = br.ReadSingle();
    }
}
