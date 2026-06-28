using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OctoGames.UI.Popups
{
    /// <summary>One popup button: binds a label + callback onto a uGUI Button.</summary>
    [RequireComponent(typeof(Button))]
    public sealed class PopupButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _label;

        private Action _onClick;

        public event Action Clicked;

        private void Reset()
        {
            _button = GetComponent<Button>();
            _label = GetComponentInChildren<TMP_Text>();
        }

        private void Awake() => _button.onClick.AddListener(HandleClick);
        private void OnDestroy() => _button.onClick.RemoveListener(HandleClick);

        public void Bind(in PopupButtonData data)
        {
            _label.text = data.Label;
            _onClick = data.OnClick;
        }

        private void HandleClick()
        {
            _onClick?.Invoke();
            Clicked?.Invoke();
        }
    }
}
