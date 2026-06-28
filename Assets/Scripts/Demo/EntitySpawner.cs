using System.Collections.Generic;
using OctoGames.Gameplay;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace OctoGames.Demo
{
    /// <summary>Demo driver: spawns / disables / destroys entities to move the active count.</summary>
    public sealed class EntitySpawner : MonoBehaviour
    {
        [SerializeField] private Character _prefab;
        [SerializeField] private Transform _spawnParent;
        [SerializeField] private Vector2 _areaHalfExtents = new(7f, 7f);

        private IObjectResolver _resolver;
        private readonly List<Character> _spawned = new();

        [Inject]
        public void Construct(IObjectResolver resolver) => _resolver = resolver;

        public void SpawnOne()
        {
            // Instantiate via the resolver so the registry is injected before OnEnable.
            Character character = _resolver.Instantiate(_prefab, _spawnParent);
            character.transform.localPosition = new Vector3(
                Random.Range(-_areaHalfExtents.x, _areaHalfExtents.x), 0.5f,
                Random.Range(-_areaHalfExtents.y, _areaHalfExtents.y));
            character.SetValue(Random.Range(1, 100));
            _spawned.Add(character);
        }

        public void DisableLast()
        {
            for (int i = _spawned.Count - 1; i >= 0; i--)
            {
                if (_spawned[i] != null && _spawned[i].gameObject.activeSelf)
                {
                    _spawned[i].gameObject.SetActive(false);
                    return;
                }
            }
        }

        public void DestroyLast()
        {
            for (int i = _spawned.Count - 1; i >= 0; i--)
            {
                if (_spawned[i] != null)
                {
                    Destroy(_spawned[i].gameObject);
                    _spawned.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
