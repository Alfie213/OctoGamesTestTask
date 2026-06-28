using OctoGames.Demo;
using OctoGames.Saving;
using VContainer;
using VContainer.Unity;

namespace OctoGames.DI
{
    /// <summary>Registers the save service and injects it into the SaveLoad scene.</summary>
    public sealed class SaveLoadLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance<ISaveService>(new SaveService());
            builder.RegisterComponentInHierarchy<SaveLoadUI>();
        }
    }
}
