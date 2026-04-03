using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.intro
{
    sealed class IntroAssets(
        GraphicsDevice graphicsDevice,
        GameServiceContainer gameServiceContainer
        ) : LocalAssets (
            graphicsDevice:  graphicsDevice,
            gameServiceContainer: gameServiceContainer)
    {
        public Texture2D? Background { get; private set; }

        public override void Load()
        {
            base.Load();

            // Loading textures
            Background = _contentManager!.Load<Texture2D>("img/intro_placeholder.png");

            // Creating renderTargets
            // ... none yet
        }

        public override void Prerender(SpriteBatch spriteBatch, GlobalAssets globalAssets)
        {
            base.Prerender(spriteBatch, globalAssets);

            // Prerendering renterTargets
            // ... nothing yet
        }

        public override void Unload()
        {
            // Disposing managed (textures)
            base.Unload();

            // Unreferencing textures
            Background = null;

            // Disposing renterTargets
            // ... none yet
        }
    }
}
