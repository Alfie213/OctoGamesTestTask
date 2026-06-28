using OctoGames.UI.Popups;
using UnityEngine;
using VContainer;

namespace OctoGames.Demo
{
    /// <summary>Demo: opens a popup for each use case. IPopupService is injected.</summary>
    public sealed class PopupDemoPresenter : MonoBehaviour
    {
        private IPopupService _popups;

        [Inject]
        public void Construct(IPopupService popups) => _popups = popups;

        public void ShowConfirmation()
        {
            PopupBuilder.Create()
                .Title("Delete save?")
                .Body("This permanently removes your progress.")
                .Button("Cancel", null)
                .Button("Delete", () => Debug.Log("[Demo] Save deleted."))
                .Show(_popups);
        }

        public void ShowStoryChoice()
        {
            PopupBuilder.Create()
                .Title("A fork in the road")
                .Body("Which path do you take?")
                .Button("Forest", () => Debug.Log("[Demo] forest"))
                .Button("River", () => Debug.Log("[Demo] river"))
                .Button("Turn back", () => Debug.Log("[Demo] back"))
                .Show(_popups);
        }

        public void ShowWarning()
        {
            PopupBuilder.Create()
                .Title("Connection lost")
                .Body("You were disconnected from the server.")
                .Button("OK", () => Debug.Log("[Demo] warning ok"))
                .Show(_popups);
        }

        public void ShowTutorial()
        {
            PopupBuilder.Create()
                .Title("Tip: saving")
                .Body("Your progress saves at each checkpoint.")
                .Button("Got it", () => Debug.Log("[Demo] tutorial done"))
                .Show(_popups);
        }
    }
}
