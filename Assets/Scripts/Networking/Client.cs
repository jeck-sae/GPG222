using System;
using System.IO;
using System.Net.Sockets;
using Networking;
using Unity.VisualScripting;
using UnityEngine;

public class Client : MonoBehaviour
{
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

            var joinPacket = new PlayerJoinPacket(playerData.ID, 0, playerData.Name);
            SendPacket(joinPacket);
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
                        NetworkEvents.OnMovePacketReceived(movePacket);
                        break;
                    case PacketType.GameStart:
                        var gameStartPacket = new GameStartPacket();
                        gameStartPacket.Deserialize(br);
                        NetworkEvents.OnGameStartPacketReceived(gameStartPacket);
                        break;
                    case PacketType.LoadLevel:
                        var loadLevelPacket = new LoadLevelPacket();
                        loadLevelPacket.Deserialize(br);
                        NetworkEvents.OnLoadLevelPacketReceived(loadLevelPacket);
                        break;
                    case PacketType.PlayerJoin:
                        var joinPacket = new PlayerJoinPacket();
                        joinPacket.Deserialize(br);
                        NetworkEvents.OnPlayerJoinPacketReceived(joinPacket);
                        break;
                    case PacketType.PlayerReachedGoal:
                        var playerReachedGoalPacket = new PlayerReachedGoalPacket();
                        playerReachedGoalPacket.Deserialize(br);
                        NetworkEvents.OnPlayerReachedGoalPacketReceived(playerReachedGoalPacket);
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
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to send packet: " + e.Message);
        }
    }
}