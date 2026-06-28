namespace OctoGames.UI.Popups
{
    /// <summary>Shows popups. Consumers depend on this, not the concrete service.</summary>
    public interface IPopupService
    {
        PopupView Show(PopupData data);
    }
}
