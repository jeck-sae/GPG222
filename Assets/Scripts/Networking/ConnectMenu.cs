using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField ipField;
    [SerializeField] private string loadSceneOnConnect;

    private void Start()
    {
        Client.Instance.OnConnect += OnConnect;
    }

    private void OnDestroy()
    {
        Client.Instance.OnConnect -= OnConnect;
    }

    public void Connect()
    {
        if (string.IsNullOrWhiteSpace(usernameField.text)
            || string.IsNullOrWhiteSpace(ipField.text))
        {
            Debug.LogWarning("Invalid username or IP");
            return;
        }
        
        var playerData = new PlayerData(Guid.NewGuid().ToString(),usernameField.text);
        
        Client.Instance.ConnectToServer(ipField.text, playerData);
    }

    private void OnConnect()
    {
        SceneManager.LoadScene(loadSceneOnConnect);
    }
}
