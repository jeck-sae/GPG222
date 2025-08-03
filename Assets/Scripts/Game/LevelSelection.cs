using System.Collections;
using System.Collections.Generic;
using Networking;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void LoadLevel(string levelId)
    {
        LoadLevelPacket packet = new LoadLevelPacket { levelId = levelId };
        Client.Instance.SendPacket(packet);
        SceneManager.LoadScene(levelId);
    }
}