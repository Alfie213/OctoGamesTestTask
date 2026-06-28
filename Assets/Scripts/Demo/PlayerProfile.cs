using System;
using System.Collections.Generic;

namespace OctoGames.Demo
{
    /// <summary>Example save payload (JsonUtility-friendly: fields, wrapped list).</summary>
    [Serializable]
    public sealed class PlayerProfile
    {
        public string playerName = "Player";
        public int level = 1;
        public float masterVolume = 0.8f;
        public List<string> unlockedFlags = new();

        public static PlayerProfile CreateDefault() => new PlayerProfile();
    }
}
