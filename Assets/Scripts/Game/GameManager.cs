using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Networking;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    protected int winners;
    
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnChangedScene;
    }

    private void OnChangedScene(Scene arg0, LoadSceneMode arg1)
    {
        winners = 0;
    }
    
    private void OnEnable()
    {
        NetworkEvents.PlayerReachedGoalPacketReceived += EnemyPlayerReachedGoal;
    }

    private void OnDisable()
    {
        NetworkEvents.PlayerReachedGoalPacketReceived -= EnemyPlayerReachedGoal;
    }
    

    private void EnemyPlayerReachedGoal(PlayerReachedGoalPacket obj)
    {
        AddWinner();
    }

    protected void AddWinner()
    {
        winners++;
        if (winners >= PlayerTracker.Count)
            Invoke(nameof(OpenLevelSelect), 5);
    }
    
    public void ReachedGoal()
    {
        var player = FindAnyObjectByType<playerMovement>();
        if (player != null) player.enabled = false;
        
        string playerId = Client.Instance.playerData.ID;
        float timeTaken = Time.timeSinceLevelLoad;
        
        var packet = new PlayerReachedGoalPacket(playerId, timeTaken);
        Client.Instance.SendPacket(packet);
        
        LeaderboardManager.Instance.AddEntry(playerId, timeTaken);
        AddWinner();
    }
    
    public void OpenLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }
    
    
}