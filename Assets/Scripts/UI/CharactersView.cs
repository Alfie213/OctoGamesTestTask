using System.Collections.Generic;
using System.Text;
using OctoGames.Gameplay;
using TMPro;
using UnityEngine;
using VContainer;

namespace OctoGames.UI
{
    /// <summary>
    /// Shows the count and average value of active entities. Refreshes on an interval
    /// (not every frame) and allocates nothing per refresh. See ANSWERS.md §4.
    /// </summary>
    public sealed class CharactersView : MonoBehaviour
    {
        [SerializeField] private float _refreshInterval = 0.25f;

        private readonly StringBuilder _builder = new(64);
        private TMP_Text _label;
        private EntityRegistry _registry;
        private float _timer;
        private string _lastText;

        [Inject]
        public void Construct(EntityRegistry registry) => _registry = registry;

        private void Awake()
        {
            _label = GetComponent<TMP_Text>();
            if (_label == null)
            {
                Debug.LogError($"[CharactersView] No TMP_Text on '{name}'.", this);
                enabled = false;
            }
        }

        private void OnEnable() => _timer = _refreshInterval; // refresh on first frame

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer < _refreshInterval) return;

            _timer = 0f;
            Refresh();
        }

        private void Refresh()
        {
            if (_registry == null) return;

            IReadOnlyList<IGameplayEntity> active = _registry.GetActive();
            int count = 0;
            float total = 0f;
            for (int i = 0; i < active.Count; i++)
            {
                if (active[i] is Character character)
                {
                    count++;
                    total += character.Value;
                }
            }

            float average = count > 0 ? total / count : 0f;

            _builder.Clear();
            _builder.Append("Characters: ").Append(count)
                    .Append("  Avg value: ").Append(average.ToString("0.##"));
            string text = _builder.ToString();

            // Assigning text rebuilds the mesh, so only do it when it actually changed.
            if (text != _lastText)
            {
                _lastText = text;
                _label.text = text;
            }
        }
    }
}
