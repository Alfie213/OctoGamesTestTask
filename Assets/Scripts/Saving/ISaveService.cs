namespace OctoGames.Saving
{
    /// <summary>Persists serializable data by key. Injected so callers stay testable.</summary>
    public interface ISaveService
    {
        bool Save<T>(string key, T data);

        /// <summary>False (and default) for missing or invalid data — never throws.</summary>
        bool TryLoad<T>(string key, out T data);

        T LoadOrDefault<T>(string key, T fallback = default);
        bool Exists(string key);
        bool Delete(string key);

        /// <summary>Absolute file path for a key (used by the corrupt-file test).</summary>
        string GetSavePath(string key);
    }
}
