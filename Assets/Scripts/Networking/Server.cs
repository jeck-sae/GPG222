using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server : MonoBehaviour
{
    Socket serverSocket;
    int port = 6969;

    List<ConectionInfo> ConectionInfo;
    List<ConectionInfo> Responded;
    void Start()
    {
        Responded = new List<ConectionInfo>();
        ConectionInfo = new List<ConectionInfo>();

        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
        serverSocket.Listen(5);
        serverSocket.Blocking = false;

        StartCoroutine(Pingclient());
    }

    void Update()
    {
        try
        {
            ConectionInfo CI = new ConectionInfo() { socket = serverSocket.Accept() };
            ConectionInfo.Add(CI);
            Debug.LogError("Client connected: ");
        }
        catch
        {
        }

        //for (int i = 0; i < ConectionInfo.Count; i++)
        //{
        //    if (!ConectionInfo[i].socket.Poll(0, SelectMode.SelectRead) && ConectionInfo[i].socket.Available == 0)
        //    {
        //        
        //    }
        //}

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
                    if (type == PacketType.ping)
                    {

                        Responded.Add(ConectionInfo[i]);

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
    IEnumerator Pingclient()
    {
        PlayerReachedGoalPacket packet = new PlayerReachedGoalPacket();
        yield return new WaitForSeconds(1);
        var buffer = packet.Serialize();

        try
        {
            for (int i = 0; i < ConectionInfo.Count; i++)
            {
                ConectionInfo[i].socket.Send(buffer);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to send packet: " + e.Message);
        }
        yield return new WaitForSeconds(2);
        for (int i = 0; i < ConectionInfo.Count; i++)
        {
            if (!Responded.Contains(ConectionInfo[i]))
            {
                {
                    Debug.LogError("Client disconnected.");
                    //NetworkObjects.Instance.Destroy(playerobject);
                    ConectionInfo[i].socket.Close();
                    ConectionInfo.RemoveAt(i);
                    i--;
                    continue;
                }
            }
        }
        StartCoroutine(Pingclient());
    }
}
public class ConectionInfo
{
    public PlayerData playerdata;
    public Socket socket;
}