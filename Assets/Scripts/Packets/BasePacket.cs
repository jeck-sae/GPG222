using System.IO;

public abstract class BasePacket
{
    public PacketType type;

    public abstract byte[] Serialize();

    public abstract void Deserialize(BinaryReader br);
}
