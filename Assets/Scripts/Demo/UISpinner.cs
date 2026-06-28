using UnityEngine;

namespace OctoGames.Demo
{
    /// <summary>Spins a UI element; unscaled time so it keeps turning while paused.</summary>
    public sealed class UISpinner : MonoBehaviour
    {
        [SerializeField] private float _degreesPerSecond = 180f;

        private RectTransform _rect;

        private void Awake() => _rect = (RectTransform)transform;

        private void Update() => _rect.Rotate(0f, 0f, -_degreesPerSecond * Time.unscaledDeltaTime);
    }
}
