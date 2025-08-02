using System;
using System.Collections;
using System.Collections.Generic;
using Networking;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoading : MonoBehaviour
{

    private void OnEnable()
    {
        NetworkEvents.LoadLevelPacketReceived += LoadLevel;
    }

    private void LoadLevel(LoadLevelPacket packet)
    {
        SceneManager.LoadScene(packet.levelId);
    }

    private void OnDisable()
    {
        NetworkEvents.LoadLevelPacketReceived -= LoadLevel;
    }
}
