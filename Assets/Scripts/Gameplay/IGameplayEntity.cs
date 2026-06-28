using UnityEngine;

namespace OctoGames.Gameplay
{
    /// <summary>A trackable gameplay entity: enemy, interactable, story actor, etc.</summary>
    public interface IGameplayEntity
    {
        int EntityId { get; }
        bool IsActive { get; }
        Transform Transform { get; }
    }
}
