using OctoGames.Demo;
using OctoGames.Gameplay;
using OctoGames.UI;
using VContainer;
using VContainer.Unity;

namespace OctoGames.DI
{
    /// <summary>Registers the entity registry and injects it into the view and spawner.</summary>
    public sealed class GameplayLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<EntityRegistry>(Lifetime.Singleton);
            builder.RegisterComponentInHierarchy<CharactersView>();
            builder.RegisterComponentInHierarchy<EntitySpawner>();
        }
    }
}
