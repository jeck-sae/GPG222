using System.IO;

public enum PacketType {  }

public abstract class BasePacket
{
    public PacketType type;

    public byte[] Serialize()
    {
        return null;
    }

    public void Deserialize(BinaryReader br)
    {
        
    }
}
