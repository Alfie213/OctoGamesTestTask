using System;
using UnityEngine;

namespace OctoGames.Saving
{
    /// <summary>
    /// JsonUtility serializer. No Dictionary / polymorphism / top-level collections —
    /// wrap those in a [Serializable] class or inject a different serializer.
    /// </summary>
    public sealed class JsonUtilitySerializer : ISaveSerializer
    {
        private readonly bool _prettyPrint;

        public JsonUtilitySerializer(bool prettyPrint = true) => _prettyPrint = prettyPrint;

        public string Serialize<T>(T data) => JsonUtility.ToJson(data, _prettyPrint);

        public bool TryDeserialize<T>(string raw, out T data)
        {
            data = default;
            if (string.IsNullOrWhiteSpace(raw)) return false;

            try
            {
                data = JsonUtility.FromJson<T>(raw);
                return data != null; // null on a type mismatch
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SaveService] Bad JSON for {typeof(T).Name}: {e.Message}");
                return false;
            }
        }
    }
}
