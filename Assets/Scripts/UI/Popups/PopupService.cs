using System.Collections.Generic;
using UnityEngine;

namespace OctoGames.UI.Popups
{
    /// <summary>Shows popups, pooling instances to avoid Instantiate churn.</summary>
    public sealed class PopupService : MonoBehaviour, IPopupService
    {
        [SerializeField] private PopupView _popupPrefab;

        private readonly Stack<PopupView> _pool = new();

        // TODO: queue requests so two popups can't open on top of each other.
        public PopupView Show(PopupData data)
        {
            if (_popupPrefab == null)
            {
                Debug.LogError("[PopupService] No popup prefab assigned.");
                return null;
            }

            PopupView popup = Rent();
            popup.Populate(data);
            popup.transform.SetAsLastSibling();
            return popup;
        }

        private PopupView Rent()
        {
            PopupView popup = _pool.Count > 0 ? _pool.Pop() : Instantiate(_popupPrefab, transform);
            popup.Closed += Recycle;
            return popup;
        }

        private void Recycle(PopupView popup)
        {
            popup.Closed -= Recycle;
            _pool.Push(popup);
        }
    }
}
