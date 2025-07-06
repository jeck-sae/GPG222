using System;
using System.IO;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;

public class Client : MonoBehaviour
{
    public event Action<BasePacket> OnPacketSent;
    public event Action<BasePacket> OnPacketReceived;
    public event Action OnConnect;

    private Socket socket;
    private int port = 6969;
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
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    void Update()
    {
        ReceivePackets();
    }


    public void ConnectToServer(string ipAddress, PlayerData playerData)
    {
        if (connected)
            return;

        try
        {
            socket.Connect(ipAddress, port);
            connected = true;
            OnConnect?.Invoke();
            Debug.LogError("Connected to server!");
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

            while (rms.Position < rms.Length)
            {
                var type = (PacketType)br.ReadInt32();
                switch (type)
                {
                    case PacketType.Move:
                        var movePacket = new MovePacket();
                        movePacket.Deserialize(br);
                        //move player
                        OnPacketReceived?.Invoke(movePacket);
                        break;
                    case PacketType.GameStart:
                        break;
                    case PacketType.LoadLevel:
                        break;
                    case PacketType.PlayerJoin:
                        var joinPacket = new PlayerJoinPacket();
                        joinPacket.Deserialize(br);
                        Debug.Log($"Player joined: {joinPacket.playerName} ({joinPacket.spriteId}) [{joinPacket.playerId}]");
                        OnPacketSent?.Invoke(joinPacket);
                        break;
                    case PacketType.PlayerReachedGoal:
                        break;
                    case PacketType.ping:
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