using System.Data;
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
        public Texture2D? BackgroundSpritesheet { get; private set; }
        public Texture2D? ObjectSpritesheet { get; private set; }
        public Texture2D? OraSpritesheet { get; private set; }


		// FOR BACKGROUND 1
        public RenderTarget2D? SkyBackground { get; private set; }
        public RenderTarget2D? SkyCloudsBig { get; private set; }
        public RenderTarget2D? SkyCloudsSmall { get; private set; }

        public RenderTarget2D? WaterBackground { get; private set; }
        public RenderTarget2D? WaterHorizon { get; private set; }


		// FOR BACKGROUND 2
		public RenderTarget2D? TempleBackground { get; private set; }
		public RenderTarget2D? TemplePilarsBig { get; private set; }
		public RenderTarget2D? TemplePilarsSmall { get; private set; }


        public override void Load()
        {
            base.Load();

            // Loading textures
            DebugSpritesheet = _contentManager!.Load<Texture2D>("img/editor_spritesheet.png");
            StaticSpritesheet = _contentManager!.Load<Texture2D>("img/static_spritesheet_level1.png");
            AnimatedSpritesheet = _contentManager!.Load<Texture2D>("img/animated_spritesheet_level1.png");
            BackgroundSpritesheet = _contentManager!.Load<Texture2D>("img/background_spritesheet_level1.png");
            ObjectSpritesheet = _contentManager!.Load<Texture2D>("img/object_spritesheet_level1.png");
            OraSpritesheet = _contentManager!.Load<Texture2D>("img/ora_spritesheet.png");

            // Creating renderTargets
            SkyBackground = new RenderTarget2D(
                graphicsDevice: _graphicsDevice,
				width: Configuration.VisibleScreen.Px.Width,
				height: Configuration.VisibleScreen.Px.Height,
				mipMap: false,
				preferredFormat: SurfaceFormat.Color,
				preferredDepthFormat: DepthFormat.None,
				preferredMultiSampleCount: 0,
				usage: RenderTargetUsage.DiscardContents
				);

			WaterBackground = new RenderTarget2D(
	            graphicsDevice: _graphicsDevice,
	            width: Configuration.VisibleScreen.Px.Width,
	            height: Configuration.VisibleScreen.Px.Height,
	            mipMap: false,
	            preferredFormat: SurfaceFormat.Color,
	            preferredDepthFormat: DepthFormat.None,
	            preferredMultiSampleCount: 0,
	            usage: RenderTargetUsage.DiscardContents
	            );

            SkyCloudsBig = new RenderTarget2D(
				graphicsDevice: _graphicsDevice,
				width: Configuration.VisibleScreen.Px.Width * 2,
				height: Configuration.Tile.Px.Height,
				mipMap: false,
				preferredFormat: SurfaceFormat.Color,
				preferredDepthFormat: DepthFormat.None,
				preferredMultiSampleCount: 0,
				usage: RenderTargetUsage.DiscardContents
				);

			SkyCloudsSmall = new RenderTarget2D(
	            graphicsDevice: _graphicsDevice,
	            width: Configuration.VisibleScreen.Px.Width * 2,
	            height: Configuration.Tile.Px.Height,
	            mipMap: false,
	            preferredFormat: SurfaceFormat.Color,
	            preferredDepthFormat: DepthFormat.None,
	            preferredMultiSampleCount: 0,
	            usage: RenderTargetUsage.DiscardContents
	            );

			WaterHorizon = new RenderTarget2D(
				graphicsDevice: _graphicsDevice,
				width: Configuration.VisibleScreen.Px.Width * 2,
				height: Configuration.Tile.Px.Height * 3,
				mipMap: false,
				preferredFormat: SurfaceFormat.Color,
				preferredDepthFormat: DepthFormat.None,
				preferredMultiSampleCount: 0,
				usage: RenderTargetUsage.DiscardContents
				);


			TempleBackground = new RenderTarget2D(
				graphicsDevice: _graphicsDevice,
				width: Configuration.VisibleScreen.Px.Width,
				height: Configuration.VisibleScreen.Px.Height,
				mipMap: false,
				preferredFormat: SurfaceFormat.Color,
				preferredDepthFormat: DepthFormat.None,
				preferredMultiSampleCount: 0,
				usage: RenderTargetUsage.DiscardContents
				);
			TemplePilarsBig = new RenderTarget2D(
				graphicsDevice: _graphicsDevice,
				width: Configuration.VisibleScreen.Px.Width * 2,
				height: Configuration.VisibleScreen.Px.Height,
				mipMap: false,
				preferredFormat: SurfaceFormat.Color,
				preferredDepthFormat: DepthFormat.None,
				preferredMultiSampleCount: 0,
				usage: RenderTargetUsage.DiscardContents
				);
			TemplePilarsSmall = new RenderTarget2D(
				graphicsDevice: _graphicsDevice,
				width: Configuration.VisibleScreen.Px.Width * 2,
				height: Configuration.VisibleScreen.Px.Height,
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

            GameplayPayload gameplayPayload = (payload as GameplayPayload)!;

            // Prerendering renderTargets

			// BACKGROUND 1
            PrerenderBegin(spriteBatch, SkyBackground!);
            for(int i = 0; i < 12; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    spriteBatch.Draw(
                        texture: BackgroundSpritesheet,
                        position: new Vector2(i*16, j*16),
                        sourceRectangle: new Rectangle(0,0,16,16),
                        color: Color.White
                        );
                }
            }
            PrerenderSwitch(spriteBatch, WaterBackground!);
			for (int i = 0; i < 12; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					spriteBatch.Draw(
						texture: BackgroundSpritesheet,
						position: new Vector2(i * 16, j * 16),
						sourceRectangle: new Rectangle(0, 48, 16, 16),
						color: Color.White
						);
				}
			}

            PrerenderSwitch(spriteBatch, WaterHorizon!);
            for(int i = 0; i < 24; i++)
            {
				spriteBatch.Draw(
	                texture: BackgroundSpritesheet,
	                position: new Vector2(i * 16, 32),
	                sourceRectangle: new Rectangle(16, 48, 16, 16),
	                color: Color.White
	                );
			}
			spriteBatch.Draw(
				texture: BackgroundSpritesheet,
				position: new Vector2(0, 0),
				sourceRectangle: new Rectangle(0, 16, 32, 32),
				color: Color.White
				);
			spriteBatch.Draw(
				texture: BackgroundSpritesheet,
				position: new Vector2(Configuration.VisibleScreen.Px.Width, 0),
				sourceRectangle: new Rectangle(0, 16, 32, 32),
				color: Color.White
				);

			spriteBatch.Draw(
				texture: BackgroundSpritesheet,
				position: new Vector2(16 * 5, 0),
				sourceRectangle: new Rectangle(32, 16, 64, 32),
				color: Color.White
				);
			spriteBatch.Draw(
				texture: BackgroundSpritesheet,
				position: new Vector2(Configuration.VisibleScreen.Px.Width + 16*5, 0),
				sourceRectangle: new Rectangle(32, 16, 64, 32),
				color: Color.White
				);

			PrerenderSwitch(spriteBatch, SkyCloudsSmall!);
            for(int i = 0; i < 12; i++)
            {
				spriteBatch.Draw(
	                texture: BackgroundSpritesheet,
	                position: new Vector2(i * 32 + 16, 0),
	                sourceRectangle: new Rectangle(48, 0, 16, 16),
	                color: Color.White
	                );
			}

			PrerenderSwitch(spriteBatch, SkyCloudsBig!);
			for (int i = 0; i < 6; i++)
			{
				spriteBatch.Draw(
					texture: BackgroundSpritesheet,
					position: new Vector2(i * 64, 0),
					sourceRectangle: new Rectangle(16, 0, 32, 16),
					color: Color.White
					);
			}

			// BACKGROUND 2

			PrerenderSwitch(spriteBatch, TempleBackground!);
			for (int i = 0; i < 12; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					spriteBatch.Draw(
						texture: BackgroundSpritesheet,
						position: new Vector2(i * 16, j * 16),
						sourceRectangle: new Rectangle(128, 0, 16, 16),
						color: Color.White
						);
				}
			}

			PrerenderSwitch(spriteBatch, TemplePilarsBig!);
			for(int pilar = 0; pilar < 6; pilar++)
			{
				spriteBatch.Draw(
					texture: BackgroundSpritesheet,
					position: new Vector2(64 * pilar, 0),
					sourceRectangle: new Rectangle(128, 16, 64, 48),
					color: Color.White
					);
				for (int row = 0; row < 5; row++)
				{
					spriteBatch.Draw(
						texture: BackgroundSpritesheet,
						position: new Vector2(64 * pilar + 16, (row+3) * 16),
						sourceRectangle: new Rectangle(128 + 16, 0, 16, 16),
						color: Color.White
						);
				}
			}

			PrerenderSwitch(spriteBatch, TemplePilarsSmall!);
			for (int pilar = 0; pilar < 12; pilar++)
			{
				spriteBatch.Draw(
					texture: BackgroundSpritesheet,
					position: new Vector2(32 * pilar, 48),
					sourceRectangle: new Rectangle(192, 16, 32, 32),
					color: Color.White
					);
				for (int row = 0; row < 3; row++)
				{
					spriteBatch.Draw(
						texture: BackgroundSpritesheet,
						position: new Vector2(32 * pilar, (row + 5) * 16),
						sourceRectangle: new Rectangle(128 + 32, 0, 16, 16),
						color: Color.White
						);
				}
			}

			PrerenderEnd(spriteBatch);

		}

        public override void Unload()
        {
            // Disposing managed (textures)
            base.Unload();

            // Unreferencing textures
            DebugSpritesheet = null;
            StaticSpritesheet = null;
            AnimatedSpritesheet = null;
            BackgroundSpritesheet = null;
            ObjectSpritesheet = null;
            OraSpritesheet = null;

            // Disposing renderTargets
            SkyBackground!.Dispose();
            SkyBackground = null;
            SkyCloudsBig!.Dispose();
            SkyCloudsBig = null;
            SkyCloudsSmall!.Dispose();
            SkyCloudsSmall = null;
            WaterBackground!.Dispose();
            WaterBackground = null;
            WaterHorizon!.Dispose();
            WaterHorizon = null;

			TempleBackground!.Dispose(); 
			TempleBackground = null;
			TemplePilarsBig!.Dispose(); 
			TemplePilarsBig = null;
			TemplePilarsSmall!.Dispose();
			TemplePilarsSmall = null;
        }
    }
}
