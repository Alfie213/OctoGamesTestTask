using OctoGames.Demo;
using OctoGames.UI.Popups;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace OctoGames.DI
{
    /// <summary>Wires the popup service (as IPopupService) into the demo presenter.</summary>
    public sealed class PopupLifetimeScope : LifetimeScope
    {
        [SerializeField] private PopupService _popupService;
        [SerializeField] private PopupDemoPresenter _presenter;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_popupService).As<IPopupService>();
            builder.RegisterComponent(_presenter);
        }
    }
}
