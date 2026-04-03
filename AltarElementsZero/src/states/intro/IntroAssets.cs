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
        public RenderTarget2D? DebugText { get; private set; }

        public override void Load()
        {
            base.Load();

            // Loading textures
            Background = _contentManager!.Load<Texture2D>("img/intro_placeholder.png");

            // Creating renderTargets
            DebugText = new RenderTarget2D(
                graphicsDevice: _graphicsDevice,
                width: 192,
                height: 128,
                mipMap: false,
                preferredFormat: SurfaceFormat.Color,
                preferredDepthFormat: DepthFormat.None,
                preferredMultiSampleCount: 0,
                usage: RenderTargetUsage.DiscardContents
                );
        }

        public override void Prerender(SpriteBatch spriteBatch, GlobalAssets globalAssets)
        {
            base.Prerender(spriteBatch, globalAssets);

            // Prerendering renterTargets
            PrerenderBegin(spriteBatch, DebugText!);
            spriteBatch.Draw(globalAssets.RomanFont, Vector2.Zero, new Rectangle(16, 64, 16, 16), Color.White);
            PrerenderEnd(spriteBatch);
        }

        public override void Unload()
        {
            // Disposing managed (textures)
            base.Unload();

            // Unreferencing textures
            Background = null;

            // Disposing renterTargets
            DebugText!.Dispose();
            DebugText = null;
        }
    }
}
