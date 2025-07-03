using System;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
    public event Action<BasePacket> OnPacketSent;
    public event Action<BasePacket> OnPacketReceived;
    public event Action OnConnect;

    private Socket socket;
    private int port = 3000;
    private bool connected = false;
    
    public static Client Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Update()
    {
        ReceivePackets();
    }


    public void ConnectToServer(string ipAddress, PlayerData playerData)
    {
        if (connected) 
            return;
        
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            socket.Connect(ipAddress, port);
            socket.Blocking = false;
            connected = true;
            OnConnect?.Invoke();
            Debug.Log("Connected to server!");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to connect to " + ipAddress + ": " + e.Message);
        }
    }

    private void ReceivePackets()
    {
        if (!connected)
            return;

        while (socket.Available > 0)
        {
            byte[] receivedBuffer = new byte[socket.Available];
            socket.Receive(receivedBuffer);
            
            var rms = new MemoryStream(receivedBuffer);
            var br = new BinaryReader(rms);

            var type = (PacketType)br.ReadInt32();
            
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                switch (type)
                {
                    case PacketType:
                        //MessagePacket messagePacket = new MessagePacket().Deserialize(receivedBuffer);
                        //do stuff
                        
                        OnPacketReceived?.Invoke(null);
                        break;
                    default:
                        Debug.LogError("Unknown packet type received.");
                        break;
                }
            }
        }
    }
    
    public void SendPacket(BasePacket packet)
    {
        var buffer = packet.Serialize();

        try
        {
            socket.Send(buffer);
            OnPacketSent?.Invoke(packet);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to send packet: " + e.Message);
        }
    }
}