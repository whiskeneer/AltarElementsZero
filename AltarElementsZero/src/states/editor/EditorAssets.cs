using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.editor
{
    sealed class EditorAssets(
        GraphicsDevice graphicsDevice,
        GameServiceContainer gameServiceContainer
        ) : LocalAssets(
            graphicsDevice: graphicsDevice,
            gameServiceContainer: gameServiceContainer
            )
    {
        public Texture2D? DebugSpritesheet { get; private set; }
        public Texture2D? EditorSpritesheet { get; private set; }

        // LEVEL 1 SPRITESHEETS
        public Texture2D? StaticSpritesheet1 { get; private set; }
        public Texture2D? AnimatedSpritesheet1 { get; private set; }
        public Texture2D? ObjectSpritesheet1 { get; private set; }

        public override void Load()
        {
            base.Load();

			// Loading textures
			DebugSpritesheet = _contentManager!.Load<Texture2D>("img/spritesheet_placeholder.png");
            EditorSpritesheet = _contentManager!.Load<Texture2D>("img/editor_spritesheet.png");

            StaticSpritesheet1 = _contentManager!.Load<Texture2D>("img/static_spritesheet_level1.png");
			AnimatedSpritesheet1 = _contentManager!.Load<Texture2D>("img/animated_spritesheet_level1.png");
			ObjectSpritesheet1 = _contentManager!.Load<Texture2D>("img/object_spritesheet_level1.png");

			// Creating renderTargets
			// none
		}

        public override void Prerender(
            SpriteBatch spriteBatch, 
            GlobalAssets globalAssets, 
            Payload payload)
        {
            base.Prerender(spriteBatch, globalAssets, payload);

            EditorPayload editorPayload = (payload as EditorPayload)!;

            // Prerendering renderTargets
            // none
        }

        public override void Unload()
        {
            // Disposing managed (textures)
            base.Unload();

			// Unreferencing textures
			DebugSpritesheet = null;
            EditorSpritesheet = null;

			StaticSpritesheet1 = null;
			AnimatedSpritesheet1 = null;
			ObjectSpritesheet1 = null;

			// Disposing renderTargets
			// none
		}


	}
}
