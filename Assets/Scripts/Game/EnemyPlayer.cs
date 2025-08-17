using UnityEngine;

public class EnemyPlayer : MonoBehaviour
{
    protected PlayerData info;

    public virtual void Initialize(PlayerData data)
    {
        this.info = data;
        
        // setup player sprite and stuff
    }
    
}
