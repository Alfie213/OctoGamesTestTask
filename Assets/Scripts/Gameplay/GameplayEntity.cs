using UnityEngine;
using VContainer;

namespace OctoGames.Gameplay
{
    /// <summary>
    /// Base for tracked entities. OnDisable runs on both disable and destroy
    /// (Unity calls it before OnDestroy), so both leave the registry the same way.
    /// </summary>
    public class GameplayEntity : MonoBehaviour, IGameplayEntity
    {
        private EntityRegistry _registry;

        public int EntityId => gameObject.GetInstanceID();
        public bool IsActive => isActiveAndEnabled;
        public Transform Transform => transform;

        // Resolver-spawned entities are injected before OnEnable; the null-check below
        // plus idempotent Register makes the order irrelevant for scene-placed ones too.
        [Inject]
        public void Construct(EntityRegistry registry)
        {
            _registry = registry;
            if (isActiveAndEnabled) _registry.Register(this);
        }

        protected virtual void OnEnable() => _registry?.Register(this);
        protected virtual void OnDisable() => _registry?.Unregister(this);
    }
}
