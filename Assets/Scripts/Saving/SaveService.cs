using System;
using System.IO;
using UnityEngine;

namespace OctoGames.Saving
{
    /// <summary>JSON save/load with atomic writes and backup recovery. Never throws on load.</summary>
    public sealed class SaveService : ISaveService
    {
        private const string FileExtension = ".json";
        private const string TempExtension = ".tmp";
        private const string BackupExtension = ".bak";

        private readonly ISaveSerializer _serializer;
        private readonly string _rootDirectory;

        public SaveService(ISaveSerializer serializer = null, string rootDirectory = null)
        {
            _serializer = serializer ?? new JsonUtilitySerializer();
            _rootDirectory = rootDirectory ?? Path.Combine(Application.persistentDataPath, "Saves");
        }

        // TODO: add a schema version to saved data and migrate on load when the format changes.
        public bool Save<T>(string key, T data)
        {
            if (!IsKeyValid(key)) return false;
            if (data == null)
            {
                Debug.LogWarning($"[SaveService] Won't save null for '{key}'.");
                return false;
            }

            try
            {
                Directory.CreateDirectory(_rootDirectory);
                string target = PathFor(key);
                string temp = target + TempExtension;

                File.WriteAllText(temp, _serializer.Serialize(data));
                AtomicReplace(temp, target);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveService] Save '{key}' failed: {e}");
                return false;
            }
        }

        public bool TryLoad<T>(string key, out T data)
        {
            data = default;
            if (!IsKeyValid(key)) return false;

            string path = PathFor(key);
            if (TryLoadFile(path, out data)) return true;

            string backup = path + BackupExtension;
            if (File.Exists(backup) && TryLoadFile(backup, out data))
            {
                Debug.LogWarning($"[SaveService] '{key}' recovered from backup.");
                return true;
            }

            return false;
        }

        public T LoadOrDefault<T>(string key, T fallback = default) =>
            TryLoad(key, out T data) ? data : fallback;

        public bool Exists(string key) => IsKeyValid(key) && File.Exists(PathFor(key));

        public bool Delete(string key)
        {
            if (!IsKeyValid(key)) return false;

            try
            {
                string path = PathFor(key);
                DeleteIfExists(path);
                DeleteIfExists(path + BackupExtension);
                DeleteIfExists(path + TempExtension);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveService] Delete '{key}' failed: {e}");
                return false;
            }
        }

        public string GetSavePath(string key) => PathFor(key);

        private bool TryLoadFile<T>(string path, out T data)
        {
            data = default;
            if (!File.Exists(path)) return false;

            try
            {
                return _serializer.TryDeserialize(File.ReadAllText(path), out data);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SaveService] Read '{path}' failed: {e.Message}");
                return false;
            }
        }

        // Write to temp then swap, keeping a .bak — a crash mid-write can't corrupt the save.
        private void AtomicReplace(string temp, string target)
        {
            if (!File.Exists(target))
            {
                File.Move(temp, target);
                return;
            }

            try
            {
                File.Replace(temp, target, target + BackupExtension);
            }
            catch (PlatformNotSupportedException)
            {
                File.Delete(target);
                File.Move(temp, target);
            }
        }

        private static void DeleteIfExists(string path)
        {
            if (File.Exists(path)) File.Delete(path);
        }

        private string PathFor(string key) => Path.Combine(_rootDirectory, Sanitize(key) + FileExtension);

        private static bool IsKeyValid(string key)
        {
            if (!string.IsNullOrWhiteSpace(key)) return true;
            Debug.LogError("[SaveService] Key must not be empty.");
            return false;
        }

        private static string Sanitize(string key)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                key = key.Replace(c, '_');
            }

            return key;
        }
    }
}
