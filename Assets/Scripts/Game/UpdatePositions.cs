using Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePositions : MonoBehaviour
{
    
    void Start()
    {
        NetworkEvents.MovePacketReceived += updatePosition;
    }

    private void updatePosition(MovePacket packet)
    {
        Transform Player = PlayerTracker.GetPlayerInfo(packet.playerId).Instance.transform;
        Player.position = new Vector2 (packet.x,packet.y);
    }

    private void OnDestroy()
    {
        NetworkEvents.MovePacketReceived -= updatePosition;
    }
}
