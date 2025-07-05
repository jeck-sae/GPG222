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

    List<ConectionInfo> ConectionInfo;

    void Start()
    {
        ConectionInfo = new List<ConectionInfo>();
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
            ConectionInfo.Add(CI);
            Debug.LogError("Client connected: ");
            StartCoroutine(Pingclient(CI));
        }
        catch
        {
        }

        try
        {

            for (int i = 0; i < ConectionInfo.Count; i++)
            {
                if (ConectionInfo[i].socket.Available > 0)
                {
                    byte[] buffer = new byte[ConectionInfo[i].socket.Available];
                    ConectionInfo[i].socket.Receive(buffer);

                    var rms = new MemoryStream(buffer);
                    var br = new BinaryReader(rms);

                    var type = (PacketType)br.ReadInt32();

                    if (type == PacketType.PlayerJoin)
                    {
                        PlayerJoinPacket joinPKT = new PlayerJoinPacket();
                        joinPKT.Deserialize(br);

                        PlayerData PD = new PlayerData(joinPKT.playerId, joinPKT.playerName);
                        ConectionInfo[i].playerdata = PD;
                        Debug.LogError(ConectionInfo[i].playerdata.Name + " joined");
                    }
                    for (int j = 0; j < ConectionInfo.Count; j++)
                    {
                        if (j != i)
                        {
                            ConectionInfo[j].socket.Send(buffer);
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
            ConectionInfo.Remove(player);
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