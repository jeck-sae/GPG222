using System.Collections;
using System.Collections.Generic;
using Networking;
using UnityEngine;

public class LevelSelection : MonoBehaviour
{
    public static LevelSelection Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadLevel(int levelId)
    {
        LoadLevelPacket packet = new LoadLevelPacket { levelId = levelId };
        NetworkEvents.OnLoadLevelPacketReceived(packet);
    }
}