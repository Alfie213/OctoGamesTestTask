using System.Collections;
using System.IO;
using OctoGames.Saving;
using TMPro;
using UnityEngine;
using VContainer;

namespace OctoGames.Demo
{
    /// <summary>Buttons-driven demo for the save system. Public methods bind to Button.onClick.</summary>
    public sealed class SaveLoadUI : MonoBehaviour
    {
        private const string SaveKey = "player_profile";

        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private TMP_Text _statusText;
        [SerializeField] private LoadingScreen _loadingScreen;
        [SerializeField] private float _minLoadingTime = 1f;

        private ISaveService _saveService;
        private PlayerProfile _profile = PlayerProfile.CreateDefault();

        [Inject]
        public void Construct(ISaveService saveService) => _saveService = saveService;

        private void Start() => StartCoroutine(StartupRoutine());

        // Stand-in for real async loading; the minimum time stops the overlay flashing.
        private IEnumerator StartupRoutine()
        {
            _loadingScreen?.Show("Loading...");
            yield return null; // let the overlay render before we work

            float start = Time.unscaledTime;
            _profile = _saveService.LoadOrDefault(SaveKey, PlayerProfile.CreateDefault());
            RefreshNameInput();
            Report($"Loaded: {Describe()}");

            float elapsed = Time.unscaledTime - start;
            if (elapsed < _minLoadingTime) yield return new WaitForSecondsRealtime(_minLoadingTime - elapsed);

            _loadingScreen?.Hide();
        }

        public void OnSaveClicked()
        {
            ReadNameFromInput();
            bool ok = _saveService.Save(SaveKey, _profile);
            Report(ok ? $"Saved: {Describe()}" : "Save failed (see Console).");
        }

        public void OnLoadClicked()
        {
            _profile = _saveService.LoadOrDefault(SaveKey, PlayerProfile.CreateDefault());
            RefreshNameInput();
            Report($"Loaded: {Describe()}");
        }

        public void OnDeleteClicked()
        {
            bool ok = _saveService.Delete(SaveKey);
            _profile = PlayerProfile.CreateDefault();
            RefreshNameInput();
            Report(ok ? "Deleted. Reset to default." : "Delete failed (see Console).");
        }

        public void OnLevelUpClicked()
        {
            _profile.level++;
            _profile.unlockedFlags.Add($"flag_{_profile.level}");
            Report($"In memory (not saved): {Describe()}");
        }

        public void OnCheckExistsClicked() => Report($"Save exists: {_saveService.Exists(SaveKey)}");

        // Robustness check: corrupt the file then load — no crash, recovers or defaults.
        public void OnCorruptTestClicked()
        {
            _saveService.Save(SaveKey, _profile);
            File.WriteAllText(_saveService.GetSavePath(SaveKey), "{ not valid json ]]]");

            _profile = _saveService.LoadOrDefault(SaveKey, PlayerProfile.CreateDefault());
            RefreshNameInput();
            Report($"Corrupt file handled: {Describe()}");
        }

        private void ReadNameFromInput()
        {
            if (_nameInput != null && !string.IsNullOrWhiteSpace(_nameInput.text))
            {
                _profile.playerName = _nameInput.text;
            }
        }

        private void RefreshNameInput()
        {
            if (_nameInput != null) _nameInput.text = _profile.playerName;
        }

        private string Describe() =>
            $"name='{_profile.playerName}', level={_profile.level}, flags={_profile.unlockedFlags.Count}";

        private void Report(string message)
        {
            if (_statusText != null) _statusText.text = message;
            Debug.Log($"[SaveLoadUI] {message}");
        }
    }
}
