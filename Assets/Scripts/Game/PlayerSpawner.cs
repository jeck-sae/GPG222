using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    
    private void Start()
    {
        foreach (var e in GameManager.instance.GetAllPlayers())
        {
            var go = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            go.GetOrAddComponent<EnemyPlayer>().info = e;
            e.Instance = go;
            go.name = e.Name;
            //setup enemy sprite and stuff
        }
    }
}