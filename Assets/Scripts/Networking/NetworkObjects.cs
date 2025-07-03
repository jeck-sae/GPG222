using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NetworkObjects : MonoBehaviour
{
    public static NetworkObjects Instance { get; private set; }
    private Dictionary<string, GameObject> objects = new();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }


    public GameObject Spawn(string prefabName, string objectId)
    {
        var go = Resources.Load<GameObject>(Path.Join("Prefabs", prefabName));
        if (!go)
        {
            Debug.LogError("Prefab not found: " + prefabName);
            return null;
        }
        
        objects.Add(objectId, go);
        return go;
    }

    public void Destroy(string objectId)
    {
        if (!objects.ContainsKey(objectId))
        {
            Debug.LogError("Object to destroy not found: " + objectId);
            return;
        }
        Destroy(objects[objectId]);
        objects.Remove(objectId);
    }
}
