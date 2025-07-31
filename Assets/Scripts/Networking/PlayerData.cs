using UnityEngine;

public class PlayerData
{
    public string ID { get; private set; }
    public string Name { get; private set; }
    public Color Color { get; private set; }
    public GameObject Instance;

    public PlayerData(string id, string name)
    {
        ID = id;
        Name = name;
    }


}