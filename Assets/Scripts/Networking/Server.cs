using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System.Net;
using System.Text;
using System;

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
            ConectionInfo CI = new ConectionInfo() { socket = serverSocket.Accept ()};
            ConectionInfo.Add(CI);
            Debug.LogError("Client connected: ");
        }
        catch
        {
        }

        for (int i = 0; i < ConectionInfo.Count; i++)
        {
            if (ConectionInfo[i].socket.Poll(0, SelectMode.SelectRead) && ConectionInfo[i].socket.Available == 0)
            {
                Debug.Log("Client disconnected.");
                //NetworkObjects.Instance.Destroy(PlayerData[i]);
                ConectionInfo[i].socket.Close();
                ConectionInfo.RemoveAt(i);
                i--;
                continue;
            }
        }

        try
        {
            for (int i = 0; i < ConectionInfo.Count; i++)
            {
                if (ConectionInfo[i].socket.Available > 0)
                {
                    byte[] buffer = new byte[ConectionInfo[i].socket.Available];
                    ConectionInfo[i].socket.Receive(buffer);

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
}

public class ConectionInfo
{
    public PlayerData playerdata;
    public Socket socket;
}