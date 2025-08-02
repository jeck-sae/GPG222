using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            string playerId = Client.Instance.playerData.ID;
            float timeTaken = Time.timeSinceLevelLoad;

            var packet = new PlayerReachedGoalPacket(playerId, timeTaken);
            Client.Instance.SendPacket(packet);

            LeaderboardManager.Instance.AddEntry(playerId, timeTaken);
        }
    }
}
