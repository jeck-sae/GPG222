using System;
using UnityEngine;

namespace Networking
{
    public static class NetworkEvents
    {
        public static event Action<MovePacket> MovePacketReceived;
        public static event Action<PlayerReachedGoalPacket>  PlayerReachedGoalPacketReceived;
        public static event Action<LoadLevelPacket>  LoadLevelPacketReceived;
        public static event Action<PlayerJoinPacket>  PlayerJoinPacketReceived;

        public static void OnMovePacketReceived(MovePacket packet)
        {
            MovePacketReceived?.Invoke(packet);
        }

        public static void OnPlayerReachedGoalPacketReceived(PlayerReachedGoalPacket packet)
        {
            PlayerReachedGoalPacketReceived?.Invoke(packet);
        }

        public static void OnLoadLevelPacketReceived(LoadLevelPacket packet)
        {
            LoadLevelPacketReceived?.Invoke(packet);
        }

        public static void OnPlayerJoinPacketReceived(PlayerJoinPacket packet)
        {
            PlayerJoinPacketReceived?.Invoke(packet);
        }
    }
}