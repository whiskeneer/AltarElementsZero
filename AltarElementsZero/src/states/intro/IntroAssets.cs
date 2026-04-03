using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AltarElementsZero.src;

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

        public override void Prerender(
            SpriteBatch spriteBatch, 
            GlobalAssets globalAssets,
            Payload payload
            )
        {
            base.Prerender(spriteBatch, globalAssets, payload);

            IntroPayload introPayload = (payload as IntroPayload)!;

            // Prerendering renterTargets
            PrerenderBegin(spriteBatch, DebugText!);
            TextUtilities.DrawText(
                spriteBatch,
                globalAssets.RomanFont!,
                16,16,
                introPayload.DebugText,
                0,0
                );
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
