using System;
using Networking;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    
    private void Start()
    {
        if(!transform) Debug.LogWarning("Player spawn point not set", this);
        if(!enemyPrefab) Debug.LogWarning("Enemy prefab not set", this);
        
        foreach (var playerData in PlayerTracker.GetAllPlayers())
        {
            SpawnPlayer(playerData);
        }
    }
    
    private void OnEnable()
    {
        PlayerTracker.OnPlayerJoined += PlayerJoined;
    }

    private void OnDisable()
    {
        PlayerTracker.OnPlayerJoined -= PlayerJoined;
    }
    
    private void PlayerJoined(PlayerData data)
    {
        SpawnPlayer(data);
    }

    void SpawnPlayer(PlayerData data)
    {
        if (data.ID == PlayerTracker.myself.ID)
            return;
        
        var go = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        go.name = data.Name;
        data.Instance = go;
        go.GetOrAddComponent<EnemyPlayer>().Initialize(data);
        //setup enemy sprite and stuff
    }
}