using System.Collections.Generic;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    [System.Serializable]
    public class ScoreEntry
    {
        public string playerId;
        public float timeTaken;
    }

    private List<ScoreEntry> leaderboard = new List<ScoreEntry>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // optional but keeps it alive between scenes

        // ✅ Subscribe to the EVENT (not the method)
        Networking.NetworkEvents.PlayerReachedGoalPacketReceived += OnPlayerReachedGoalReceived;
    }

    private void OnDestroy()
    {
        Networking.NetworkEvents.PlayerReachedGoalPacketReceived -= OnPlayerReachedGoalReceived;
    }

    // Called when packet is received from other players
    private void OnPlayerReachedGoalReceived(PlayerReachedGoalPacket packet)
    {
        Debug.Log($"[Leaderboard] Player {packet.playerId} reached goal in {packet.timeTaken:F2}s");

        AddEntry(packet.playerId, packet.timeTaken);
    }

    // Public method to also add the local player's own time
    public void AddEntry(string playerId, float timeTaken)
    {
        leaderboard.Add(new ScoreEntry { playerId = playerId, timeTaken = timeTaken });
        leaderboard.Sort((a, b) => a.timeTaken.CompareTo(b.timeTaken));
        PrintLeaderboard();
    }

    public void PrintLeaderboard()
    {
        Debug.Log("=== Leaderboard ===");
        foreach (var entry in leaderboard)
        {
            Debug.Log($"{entry.playerId}: {entry.timeTaken:F2}s");
        }
    }

    // Optional: get current leaderboard list for UI
    public List<ScoreEntry> GetLeaderboard() => leaderboard;
}
