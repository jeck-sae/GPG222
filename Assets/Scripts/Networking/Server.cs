using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using Sirenix.OdinInspector;
using UnityEngine;

public class Server : MonoBehaviour
{
    Socket serverSocket;
    int port = 6969;

    [ShowInInspector] List<ConnectionInfo> connectionInfo;

    private string currentLevel;
    private List<string> winners;
    private float levelStartTime;

    void Start()
    {
        connectionInfo = new List<ConnectionInfo>();
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
        serverSocket.Listen(5);
        serverSocket.Blocking = false;
    }

    void Update()
    {
        try
        {
            ConnectionInfo CI = new ConnectionInfo() { socket = serverSocket.Accept() };
            connectionInfo.Add(CI);
            Debug.LogError("Client connected: ");
            StartCoroutine(PingClient(CI));
        }
        catch
        {
        }

        try
        {

            for (int i = 0; i < connectionInfo.Count; i++)
            {
                if (connectionInfo[i].socket.Available > 0)
                {
                    byte[] buffer = new byte[connectionInfo[i].socket.Available];
                    connectionInfo[i].socket.Receive(buffer);

                    var rms = new MemoryStream(buffer);
                    var br = new BinaryReader(rms);

                    var packet = BasePacket.DeserializePacket(br);

                    HandlePacket(packet, connectionInfo[i]);

                    for (int j = 0; j < connectionInfo.Count; j++)
                    {
                        if (j != i)
                        {
                            connectionInfo[j].socket.Send(packet.Serialize());
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    void HandlePacket(BasePacket packet, ConnectionInfo sender)
    {

        switch (packet.type)
        {
            case PacketType.PlayerJoin:
                PlayerJoinPacket joinPKT = packet as PlayerJoinPacket;

                // Save the new player's data
                PlayerData pd = new PlayerData(joinPKT.playerId, joinPKT.playerName);
                sender.playerdata = pd;
                Debug.LogError(sender.playerdata.Name + " joined");

                // Send info of already connected players to the new one
                foreach (var alreadyConnected in connectionInfo)
                {
                    if (alreadyConnected.playerdata.ID == pd.ID)
                        continue;

                    PlayerJoinPacket joinPacket = new PlayerJoinPacket(
                        alreadyConnected.playerdata.ID, 0, alreadyConnected.playerdata.Name);
                    sender.socket.Send(joinPacket.Serialize());
                }

                // Send the current active level to the new player
                if (!string.IsNullOrWhiteSpace(currentLevel))
                {
                    LoadLevelPacket levelPacket = new LoadLevelPacket(currentLevel);
                    sender.socket.Send(levelPacket.Serialize());
                }
                break;


            case PacketType.LoadLevel:
                LoadLevelPacket loadPacket = packet as LoadLevelPacket;

                // Reset game state
                currentLevel = loadPacket.levelId;
                winners = new();
                levelStartTime = Time.time;
                break;


            case PacketType.PlayerReachedGoal:
                PlayerReachedGoalPacket reachedGoalPacket = packet as PlayerReachedGoalPacket;

                // Calculate the time taken server side
                reachedGoalPacket.timeTaken = Time.time - levelStartTime;
                winners.Add(reachedGoalPacket.playerId);

                if (winners.Count >= connectionInfo.Count)
                    currentLevel = string.Empty;
                break;
        }
    }


    IEnumerator PingClient(ConnectionInfo player)
    {
        PingPacket packet = new PingPacket();
        yield return new WaitForSeconds(1f);
        var buffer = packet.Serialize();

        try
        {
            Debug.Log("sent");
            player.socket.Send(buffer);
        }
        catch (Exception e)
        {
            Debug.LogError("Client disconnected.");

            if (winners.Contains(player.playerdata.ID)) 
                winners.Remove(player.playerdata.ID);

            if (player.playerdata != null)
            {
                var leftPkt = new PlayerLeftPacket(player.playerdata.ID);
                foreach (ConnectionInfo ci in connectionInfo)
                {
                    if (ci == player) continue;
                    try
                    { 
                        ci.socket.Send(leftPkt.Serialize()); 
                    } 
                    catch 
                    {
                    }
                }
            }


            player.socket.Close();
            connectionInfo.Remove(player);
            yield break;
        }
        StartCoroutine(PingClient(player));
    }
}


public class ConnectionInfo
{
    public PlayerData playerdata;
    public Socket socket;
}