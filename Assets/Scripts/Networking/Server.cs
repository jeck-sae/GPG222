using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System.Net;
using System.Text;

public class Server : MonoBehaviour
{
    Socket serverSocket;
    int port = 6969;

    List<Socket> clientsSocket;

    void Start()
    {
        clientsSocket = new List<Socket>();

        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
        serverSocket.Listen(5);
        serverSocket.Blocking = false;
    }

    void Update()
    {
        try
        {
            clientsSocket.Add(serverSocket.Accept());
            Debug.LogError("Client connected: ");
        }
        catch
        {
        }

        for (int i = 0; i < clientsSocket.Count; i++)
        {
            if (clientsSocket[i].Poll(0, SelectMode.SelectRead) && clientsSocket[i].Available == 0)
            {
                Debug.Log("Client disconnected.");
                clientsSocket[i].Close();
                clientsSocket.RemoveAt(i);
                i--;
                continue;
            }
        }

        try
        {
            for (int i = 0; i < clientsSocket.Count; i++)
            {
                if (clientsSocket[i].Available > 0)
                {
                    byte[] buffer = new byte[clientsSocket[i].Available];
                    clientsSocket[i].Receive(buffer);

                    for (int j = 0; j < clientsSocket.Count; j++)
                    {
                        if (j != i)
                        {
                            clientsSocket[j].Send(buffer);
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