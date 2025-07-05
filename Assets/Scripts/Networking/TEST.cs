using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerJoinPacket PPckt = new PlayerJoinPacket (Guid.NewGuid().ToString(), 123 ,"meow");

        Client.Instance.SendPacket (PPckt);
    }
}
