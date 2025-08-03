using System;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using Networking;
using System.Collections;

public class Client : MonoBehaviour
{
    public static Client Instance { get; private set; }

    public event Action OnConnect;

    private Socket socket;
    private int port = 6969;
    public bool connected = false;

    public PlayerData playerData { get; private set; }

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
            this.playerData = playerData;
            
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

            using var rms = new MemoryStream(receivedBuffer);
            using var br = new BinaryReader(rms);

            Debug.Log("outside");
            while (rms.Position < rms.Length)
            {
                Debug.Log("Deser");
                var packet = BasePacket.DeserializePacket(br);
                switch (packet.type)
                {
                    case PacketType.Move:
                        Debug.Log("Move");
                        NetworkEvents.OnMovePacketReceived(packet as MovePacket);
                        break;
                    case PacketType.LoadLevel:
                        Debug.Log("LoadLevel");
                        NetworkEvents.OnLoadLevelPacketReceived(packet as LoadLevelPacket);
                        break;
                    case PacketType.PlayerJoin:
                        Debug.Log((packet as PlayerJoinPacket).playerName + " joined!");
                        NetworkEvents.OnPlayerJoinPacketReceived(packet as PlayerJoinPacket);
                        break;
                    case PacketType.PlayerReachedGoal:
                        NetworkEvents.OnPlayerReachedGoalPacketReceived(packet as PlayerReachedGoalPacket);
                        break;
                    case PacketType.ping:
                        Debug.Log("ping");
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

    private IEnumerator SendPosition()
    {
        yield return new WaitForSeconds(0.1f);
        if (connected && playerData != null)
        {
            try
            {
                Vector3 pos = transform.position;

                var MovePacket = new MovePacket(playerData.ID, pos.x, pos.y);
                SendPacket(MovePacket);
            }
            catch
            { }
        }
        StartCoroutine(SendPosition());
    }
}
