using UnityEngine;

namespace OctoGames.Gameplay
{
    /// <summary>Sample entity with a numeric value, used by the demo and CharactersView.</summary>
    public sealed class Character : GameplayEntity
    {
        [SerializeField] private float _value;

        public float Value => _value;

        public void SetValue(float value) => _value = value;
    }
}
