using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Networking;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    Dictionary<string, PlayerData>  players = new ();
    public PlayerData MyInfo { get; protected set; }

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
    
    
    public List<PlayerData> GetAllPlayers() => players.Values.ToList();
    public PlayerData GetPlayerInfo(string id)
    {
        return players.GetValueOrDefault(id);
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