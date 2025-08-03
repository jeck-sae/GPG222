using System.Collections.Generic;
using TMPro;
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

    [Header("UI")]
    public TextMeshProUGUI leaderboardText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Networking.NetworkEvents.PlayerReachedGoalPacketReceived += OnPlayerReachedGoalReceived;
    }

    private void OnDestroy()
    {
        Networking.NetworkEvents.PlayerReachedGoalPacketReceived -= OnPlayerReachedGoalReceived;
    }

    private void OnPlayerReachedGoalReceived(PlayerReachedGoalPacket packet)
    {
        AddEntry(packet.playerId, packet.timeTaken);
    }

    public void AddEntry(string playerId, float timeTaken)
    {
        leaderboard.Add(new ScoreEntry { playerId = playerId, timeTaken = timeTaken });
        leaderboard.Sort((a, b) => a.timeTaken.CompareTo(b.timeTaken));
        UpdateLeaderboardText();
    }

    private void UpdateLeaderboardText()
    {
        if (leaderboardText == null)
            return;

        leaderboardText.text = "=== Leaderboard ===\n";
        int rank = 1;
        foreach (var entry in leaderboard)
        {
            leaderboardText.text += $"{rank}. {PlayerTracker.GetPlayerInfo(entry.playerId).Name}: {entry.timeTaken:F2}s\n";
            rank++;
        }
    }

    public List<ScoreEntry> GetLeaderboard() => leaderboard;
}
