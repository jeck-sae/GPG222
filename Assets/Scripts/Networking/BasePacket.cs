using System.IO;
using UnityEngine;

public abstract class BasePacket
{
    public PacketType type;

    public abstract byte[] Serialize();

    public abstract void Deserialize(BinaryReader br);

    public static BasePacket DeserializePacket(BinaryReader br)
    {
        var type = (PacketType)br.ReadInt32();
        
        switch (type)
        {
            case PacketType.Move:
                var movePacket = new MovePacket();
                movePacket.Deserialize(br);
                return movePacket;
            case PacketType.LoadLevel:
                var loadLevelPacket = new LoadLevelPacket();
                loadLevelPacket.Deserialize(br);
                return loadLevelPacket;
            case PacketType.PlayerJoin:
                var playerJoinPacket = new PlayerJoinPacket();
                playerJoinPacket.Deserialize(br);
                return playerJoinPacket;
            case PacketType.PlayerReachedGoal:
                var playerReachedGoalPacket = new PlayerReachedGoalPacket();
                playerReachedGoalPacket.Deserialize(br);
                return playerReachedGoalPacket;
            case PacketType.ping:
                var pingPacket = new PingPacket();
                pingPacket.Deserialize(br);
                return pingPacket;
            default:
                Debug.LogError("Unknown packet type");
                break;
        }
        return null;
    }
}
