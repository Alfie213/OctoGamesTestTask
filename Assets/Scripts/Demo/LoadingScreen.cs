using System.Collections;
using TMPro;
using UnityEngine;

namespace OctoGames.Demo
{
    /// <summary>Full-screen overlay with a fade-out, driven by a CanvasGroup.</summary>
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private float _fadeDuration = 0.3f;

        private void Reset()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _label = GetComponentInChildren<TMP_Text>();
        }

        private void Awake()
        {
            if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
            SetVisible(true); // cover the first frame of startup
        }

        public void Show(string message = null)
        {
            if (message != null && _label != null) _label.text = message;
            gameObject.SetActive(true);
            SetVisible(true);
        }

        public Coroutine Hide() => StartCoroutine(FadeOut());

        private IEnumerator FadeOut()
        {
            float t = 0f;
            float start = _canvasGroup.alpha;
            while (t < _fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                _canvasGroup.alpha = Mathf.Lerp(start, 0f, t / _fadeDuration);
                yield return null;
            }

            SetVisible(false);
            gameObject.SetActive(false);
        }

        private void SetVisible(bool visible)
        {
            _canvasGroup.alpha = visible ? 1f : 0f;
            _canvasGroup.blocksRaycasts = visible;
            _canvasGroup.interactable = visible;
        }
    }
}
