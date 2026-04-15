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

        //

        public RenderTarget2D? ChunkOutline { get; private set; }

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
			ChunkOutline = new RenderTarget2D(
				graphicsDevice: _graphicsDevice,
				width: Configuration.Chunk.Px.Width * 2,
				height: Configuration.Chunk.Px.Height * 2,
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
            Payload payload)
        {
            base.Prerender(spriteBatch, globalAssets, payload);

            EditorPayload editorPayload = (payload as EditorPayload)!;

            // Prerendering renderTargets
            PrerenderBegin(spriteBatch, ChunkOutline!);

            for (int i = 0; i < 24; i++)
            {
				spriteBatch.Draw(
	                texture: EditorSpritesheet,
	                position: new Vector2(i * 16, 16 * 7),
	                sourceRectangle: new Rectangle(0, 32, 16, 16),
	                color: Color.White
	                );
			}
			for (int j = 0; j < 16; j++)
			{
				spriteBatch.Draw(
					texture: EditorSpritesheet,
					position: new Vector2(11 * 16, 16 * j),
					sourceRectangle: new Rectangle(16, 32, 16, 16),
					color: Color.White
					);
			}

			PrerenderEnd(spriteBatch);
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
			ChunkOutline!.Dispose();
			ChunkOutline = null;
		}


	}
}
