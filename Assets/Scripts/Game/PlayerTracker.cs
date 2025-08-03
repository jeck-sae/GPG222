
using System;
using System.Collections.Generic;
using System.Linq;
using Networking;
using UnityEngine;

public class PlayerTracker : MonoBehaviour
{
    protected static PlayerTracker instance;
    Dictionary<string, PlayerData>  players = new ();
    public PlayerData MyInfo { get; protected set; }
    
    public static int Count => instance.players.Count + 1;

    public static List<PlayerData> GetAllPlayers() 
        => instance.players.Values.ToList();
    
    public static PlayerData GetPlayerInfo(string id) 
        => instance.players.GetValueOrDefault(id);

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    
    private void OnEnable()
    {
        NetworkEvents.PlayerJoinPacketReceived += PlayerJoined;
    }

    private void OnDisable()
    {
        NetworkEvents.PlayerJoinPacketReceived -= PlayerJoined;
    }
    
    private void PlayerJoined(PlayerJoinPacket data)
    {
        if (players.ContainsKey(data.playerId))
        {
            Debug.LogWarning("Player ID already exists: " +  data.playerId);
            return;
        }

        if (MyInfo == null)
        {
            MyInfo = new PlayerData(data.playerId, data.playerName);
            return;
        }
        
        players.Add(data.playerId, new PlayerData(data.playerId, data.playerName));
    }
}