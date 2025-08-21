using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(SendPosition());
    }

    private IEnumerator SendPosition()
    {
        yield return new WaitForSeconds(0.1f);
        try
        {
            if (Client.Instance.connected && Client.Instance.playerData != null)
            {
                try
                {
                    Vector3 pos = transform.position;

                    var MovePacket = new MovePacket(Client.Instance.playerData.ID, pos.x, pos.y);
                    Client.Instance.SendPacket(MovePacket);
                }
                catch
                {}
            }
        }
        catch
        {}
        
        StartCoroutine(SendPosition());
    }
}
