namespace OctoGames.Saving
{
    /// <summary>Pluggable (de)serialization for <see cref="SaveService"/>.</summary>
    public interface ISaveSerializer
    {
        string Serialize<T>(T data);

        /// <summary>Must not throw on bad input — return false so the caller can recover.</summary>
        bool TryDeserialize<T>(string raw, out T data);
    }
}
