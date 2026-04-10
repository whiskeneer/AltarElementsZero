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
        public Texture2D? StaticSpritesheet { get; private set; }
        public Texture2D? AnimatedSpritesheet { get; private set; }
        public Texture2D? ObjectSpritesheet { get; private set; }

        public override void Load()
        {
            base.Load();

            // Loading textures
            DebugSpritesheet = _contentManager!.Load<Texture2D>("img/editor_spritesheet.png");
            StaticSpritesheet = _contentManager!.Load<Texture2D>("img/static_spritesheet_level1.png");
            AnimatedSpritesheet = _contentManager!.Load<Texture2D>("img/animated_spritesheet_level1.png");
            ObjectSpritesheet = _contentManager!.Load<Texture2D>("img/object_spritesheet_level1.png");

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
            StaticSpritesheet = null;
            AnimatedSpritesheet = null;
            ObjectSpritesheet = null;

            // Disposing renderTargets
            // none
        }
    }
}
