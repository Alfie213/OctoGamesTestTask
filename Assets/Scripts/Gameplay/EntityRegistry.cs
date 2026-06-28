using System;
using System.Collections.Generic;

namespace OctoGames.Gameplay
{
    /// <summary>
    /// Tracks gameplay entities. They self-register on enable and leave on
    /// disable/destroy, so it never holds stale references. O(1) add/remove.
    /// </summary>
    public sealed class EntityRegistry
    {
        private readonly HashSet<IGameplayEntity> _entities = new();
        private readonly List<IGameplayEntity> _activeBuffer = new();

        public int Count => _entities.Count;

        public event Action<IGameplayEntity> EntityRegistered;
        public event Action<IGameplayEntity> EntityUnregistered;

        public void Register(IGameplayEntity entity)
        {
            if (entity != null && _entities.Add(entity)) EntityRegistered?.Invoke(entity);
        }

        public void Unregister(IGameplayEntity entity)
        {
            if (entity != null && _entities.Remove(entity)) EntityUnregistered?.Invoke(entity);
        }

        /// <summary>Active entities. Returns a shared buffer — read now, don't cache.</summary>
        public IReadOnlyList<IGameplayEntity> GetActive()
        {
            _activeBuffer.Clear();
            foreach (IGameplayEntity entity in _entities)
            {
                if (entity != null && entity.IsActive) _activeBuffer.Add(entity);
            }

            return _activeBuffer;
        }
    }
}
