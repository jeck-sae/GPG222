using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server : MonoBehaviour
{
    Socket serverSocket;
    int port = 6969;

    List<ConectionInfo> ConnectionInfo;

    void Start()
    {
        ConnectionInfo = new List<ConectionInfo>();
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
        serverSocket.Listen(5);
        serverSocket.Blocking = false;
    }

    void Update()
    {
        try
        {
            ConectionInfo CI = new ConectionInfo() { socket = serverSocket.Accept() };
            ConnectionInfo.Add(CI);
            Debug.LogError("Client connected: ");
            StartCoroutine(Pingclient(CI));
        }
        catch
        {
        }

        try
        {

            for (int i = 0; i < ConnectionInfo.Count; i++)
            {
                if (ConnectionInfo[i].socket.Available > 0)
                {
                    byte[] buffer = new byte[ConnectionInfo[i].socket.Available];
                    ConnectionInfo[i].socket.Receive(buffer);

                    var rms = new MemoryStream(buffer);
                    var br = new BinaryReader(rms);

                    var packet = BasePacket.DeserializePacket(br);
                    
                    if (packet.type == PacketType.PlayerJoin)
                    {
                        PlayerJoinPacket joinPKT = new PlayerJoinPacket();
                        joinPKT.Deserialize(br);

                        PlayerData pd = new PlayerData(joinPKT.playerId, joinPKT.playerName);
                        ConnectionInfo[i].playerdata = pd;
                        Debug.LogError(ConnectionInfo[i].playerdata.Name + " joined");

                        // Send info of already connected players to the new one
                        foreach (var alreadyConnected in ConnectionInfo)
                        {
                            if(alreadyConnected.playerdata.ID == pd.ID)
                                continue;
                            PlayerJoinPacket joinPacket = new PlayerJoinPacket(
                                alreadyConnected.playerdata.ID, 0, alreadyConnected.playerdata.Name);
                            ConnectionInfo[i].socket.Send(joinPacket.Serialize());
                        }
                    }

                    for (int j = 0; j < ConnectionInfo.Count; j++)
                    {
                        if (j != i)
                        {
                            ConnectionInfo[j].socket.Send(packet.Serialize());
                        }
                    }
                }
            }
        }
        catch
        {
        }
    }
    IEnumerator Pingclient(ConectionInfo player)
    {
        pingPacket packet = new pingPacket();
        yield return new WaitForSeconds(1f);
        var buffer = packet.Serialize();

        try
        {
            Debug.Log("sent");
            player.socket.Send(buffer);
        }
        catch (Exception e)
        {
            //Debug.Log("nothing recived");
            Debug.LogError("Failed to send packet: " + e.Message);
            Debug.LogError("Client disconnected.");
            //NetworkObjects.Instance.Destroy(playerobject);
            player.socket.Close();
            ConnectionInfo.Remove(player);
            yield break;
        }
        StartCoroutine(Pingclient(player));
    }
}
public class ConectionInfo
{
    public PlayerData playerdata;
    public Socket socket;
}