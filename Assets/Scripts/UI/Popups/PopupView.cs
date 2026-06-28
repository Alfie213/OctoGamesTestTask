using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace OctoGames.UI.Popups
{
    /// <summary>Renders a PopupData: title, body and 1–5 buttons. Modal via CanvasGroup.</summary>
    public sealed class PopupView : MonoBehaviour
    {
        [Header("Content")]
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _body;

        [Header("Buttons")]
        [SerializeField] private RectTransform _buttonContainer;
        [SerializeField] private PopupButton _buttonPrefab;

        [Header("Visuals")]
        [SerializeField] private CanvasGroup _canvasGroup;

        private readonly List<PopupButton> _spawnedButtons = new();

        public event Action<PopupView> Closed;

        public void Populate(PopupData data)
        {
            if (_title != null) _title.text = data.Title;
            if (_body != null) _body.text = data.Body;

            BuildButtons(data);
            SetVisible(true);
        }

        public void Close()
        {
            SetVisible(false);
            ClearButtons();
            Closed?.Invoke(this);
        }

        private void BuildButtons(PopupData data)
        {
            ClearButtons();
            foreach (PopupButtonData buttonData in data.Buttons)
            {
                PopupButton button = Instantiate(_buttonPrefab, _buttonContainer);
                button.Bind(buttonData);
                if (data.CloseOnButtonClick) button.Clicked += Close;
                _spawnedButtons.Add(button);
            }
        }

        private void ClearButtons()
        {
            foreach (PopupButton button in _spawnedButtons)
            {
                if (button != null) Destroy(button.gameObject);
            }

            _spawnedButtons.Clear();
        }

        private void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
            if (_canvasGroup == null) return;

            _canvasGroup.alpha = visible ? 1f : 0f;
            _canvasGroup.interactable = visible;
            _canvasGroup.blocksRaycasts = visible;
        }
    }
}
