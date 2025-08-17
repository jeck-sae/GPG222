
using System;
using System.Collections.Generic;
using System.Linq;
using Networking;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerTracker : MonoBehaviour
{
    protected static PlayerTracker instance;
    [ShowInInspector] Dictionary<string, PlayerData>  players = new ();
    [ShowInInspector] public static PlayerData myself { get; private set; }

    public static event Action<PlayerData> OnPlayerJoined;
    public static event Action<PlayerData> OnPlayerLeft; // NOT IMPLEMENTED
    
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

    private void Start()
    {
        Client.Instance.OnConnect += AddMyself;
    }

    void AddMyself()
    {
        myself = Client.Instance.playerData;
        players.Add(myself.ID, myself);
    }

    private void OnEnable()
    {
        NetworkEvents.PlayerJoinPacketReceived += PlayerJoined;
    }

    private void OnDisable()
    {
        NetworkEvents.PlayerJoinPacketReceived -= PlayerJoined;
        if(Client.Instance) Client.Instance.OnConnect -= AddMyself;
    }
    
    private void PlayerJoined(PlayerJoinPacket data)
    {
        if (players.ContainsKey(data.playerId))
        {
            Debug.LogWarning("Player ID already exists: " +  data.playerId);
            return;
        }

        var playerData = new PlayerData(data.playerId, data.playerName);
        players.Add(data.playerId, playerData);
        OnPlayerJoined?.Invoke(playerData);
    }
}