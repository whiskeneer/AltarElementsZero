using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay
{
    sealed class GameplayAssets(
        GraphicsDevice graphicsDevice,
        GameServiceContainer gameServiceContainer
        ) : LocalAssets(
            graphicsDevice: graphicsDevice,
            gameServiceContainer: gameServiceContainer
            )
    {
        public Texture2D? DebugSpritesheet {  get; private set; }

        public override void Load()
        {
            base.Load();

            // Loading textures
            DebugSpritesheet = _contentManager!.Load<Texture2D>("img/spritesheet_placeholder.png");

            // Creating renderTargets
            // none
        }

        public override void Prerender(
            SpriteBatch spriteBatch, 
            GlobalAssets globalAssets, 
            Payload payload
            )
        {
            base.Prerender(spriteBatch, globalAssets, payload);

            GameplayPayload gameplayPayload = (payload as GameplayPayload)!;

            // Prerendering renderTargets
            // none
        }

        public override void Unload()
        {
            // Disposing managed (textures)
            base.Unload();

            // Unreferencing textures
            DebugSpritesheet = null;

            // Disposing renderTargets
            // none
        }
    }
}
