using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            string playerId = "Player123"; // replace with actual player ID from PlayerData
            float timeTaken = Time.timeSinceLevelLoad;

            var packet = new PlayerReachedGoalPacket(playerId, timeTaken);
            Client.Instance.SendPacket(packet);

            // Locally add your own time immediately
            LeaderboardManager.Instance.AddEntry(playerId, timeTaken);
        }
    }
}
