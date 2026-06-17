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
		public Texture2D? Atlas {  get; private set; }
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

		// FOR BACKGROUND 3
		public RenderTarget2D? UnderwaterSky { get; private set; }
		public RenderTarget2D? UnderwaterFarRocks {  get; private set; }
		public RenderTarget2D? UnderwaterCloseRocks { get; private set; }
		public RenderTarget2D? UnderwaterFarCorals { get; private set; }
		public RenderTarget2D? UnderwaterCloseCorals { get; private set; }
		public RenderTarget2D? UnderwaterTemple1 { get; private set; }
		public RenderTarget2D? UnderwaterTemple2 { get; private set; }


        public override void Load()
        {
            base.Load();

            // Loading textures
            DebugSpritesheet = _contentManager!.Load<Texture2D>("img/editor_spritesheet.png");
            StaticSpritesheet = _contentManager!.Load<Texture2D>("img/static_spritesheet_level1.png");
            AnimatedSpritesheet = _contentManager!.Load<Texture2D>("img/animated_spritesheet_level1.png");
			Atlas = _contentManager!.Load<Texture2D>("img/atlas.png");
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

			UnderwaterSky = new RenderTarget2D(
				graphicsDevice: _graphicsDevice,
				width: Configuration.VisibleScreen.Px.Width * 2,
				height: Configuration.VisibleScreen.Px.Height,
				mipMap: false,
				preferredFormat: SurfaceFormat.Color,
				preferredDepthFormat: DepthFormat.None,
				preferredMultiSampleCount: 0,
				usage: RenderTargetUsage.DiscardContents
				);
			UnderwaterFarRocks = new RenderTarget2D(
				graphicsDevice: _graphicsDevice,
				width: Configuration.VisibleScreen.Px.Width * 2,
				height: Configuration.VisibleScreen.Px.Height,
				mipMap: false,
				preferredFormat: SurfaceFormat.Color,
				preferredDepthFormat: DepthFormat.None,
				preferredMultiSampleCount: 0,
				usage: RenderTargetUsage.DiscardContents
				);
			UnderwaterCloseRocks = new RenderTarget2D(
				graphicsDevice: _graphicsDevice,
				width: Configuration.VisibleScreen.Px.Width * 2,
				height: Configuration.VisibleScreen.Px.Height,
				mipMap: false,
				preferredFormat: SurfaceFormat.Color,
				preferredDepthFormat: DepthFormat.None,
				preferredMultiSampleCount: 0,
				usage: RenderTargetUsage.DiscardContents
				);
			UnderwaterFarCorals = new RenderTarget2D(
				graphicsDevice: _graphicsDevice,
				width: Configuration.VisibleScreen.Px.Width * 2,
				height: Configuration.VisibleScreen.Px.Height,
				mipMap: false,
				preferredFormat: SurfaceFormat.Color,
				preferredDepthFormat: DepthFormat.None,
				preferredMultiSampleCount: 0,
				usage: RenderTargetUsage.DiscardContents
				);
			UnderwaterCloseCorals = new RenderTarget2D(
				graphicsDevice: _graphicsDevice,
				width: Configuration.VisibleScreen.Px.Width * 2,
				height: Configuration.VisibleScreen.Px.Height,
				mipMap: false,
				preferredFormat: SurfaceFormat.Color,
				preferredDepthFormat: DepthFormat.None,
				preferredMultiSampleCount: 0,
				usage: RenderTargetUsage.DiscardContents
				);

			UnderwaterTemple1 = new RenderTarget2D(
				graphicsDevice: _graphicsDevice,
				width: 32 + 16,
				height: 32 * 8,
				mipMap: false,
				preferredFormat: SurfaceFormat.Color,
				preferredDepthFormat: DepthFormat.None,
				preferredMultiSampleCount: 0,
				usage: RenderTargetUsage.DiscardContents
				);
			UnderwaterTemple2 = new RenderTarget2D(
				graphicsDevice: _graphicsDevice,
				width: 64 + 16,
				height: 32 * 8,
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
			for (int i = 0; i < 12; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					spriteBatch.Draw(
						texture: BackgroundSpritesheet,
						position: new Vector2(i * 16, j * 16),
						sourceRectangle: new Rectangle(0, 0, 16, 16),
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
			for (int i = 0; i < 24; i++)
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
				position: new Vector2(Configuration.VisibleScreen.Px.Width + 16 * 5, 0),
				sourceRectangle: new Rectangle(32, 16, 64, 32),
				color: Color.White
				);

			PrerenderSwitch(spriteBatch, SkyCloudsSmall!);
			for (int i = 0; i < 12; i++)
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
			for (int pilar = 0; pilar < 6; pilar++)
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
						position: new Vector2(64 * pilar + 16, (row + 3) * 16),
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

			// BACKGROUND 3

			PrerenderSwitch(spriteBatch, UnderwaterSky!);
			for (int i = 0; i < 24; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					spriteBatch.Draw(
						texture: BackgroundSpritesheet,
						position: new Vector2(i * 16, j * 16),
						sourceRectangle: new Rectangle(0, 16 * 4, 16, 16),
						color: Color.White
						);
				}
			}

			PrerenderSwitch(spriteBatch, UnderwaterFarRocks!);
			for (int i = 0; i < 24; i++)
			{
				for (int j = 1; j < 8; j++)
				{
					spriteBatch.Draw(
						texture: BackgroundSpritesheet,
						position: new Vector2(i * 16, j * 16),
						sourceRectangle: new Rectangle(128, 96, 16, 16),
						color: Color.White
						);
				}
			}
			for (int i = 0; i < 12; i++) {
				spriteBatch.Draw(
					texture: BackgroundSpritesheet,
					position: new Vector2(i * 32, 0),
					sourceRectangle: new Rectangle(16, 64, 32, 16),
					color: Color.White
					);
			}

			PrerenderSwitch(spriteBatch, UnderwaterCloseRocks!);
			for (int i = 0; i < 24; i++)
			{
				for (int j = 1; j < 8; j++)
				{
					spriteBatch.Draw(
						texture: BackgroundSpritesheet,
						position: new Vector2(i * 16, j * 16),
						sourceRectangle: new Rectangle(112, 96, 16, 16),
						color: Color.White
						);
				}
			}
			for (int i = 0; i < 8; i++)
			{
				spriteBatch.Draw(
					texture: BackgroundSpritesheet,
					position: new Vector2(i * 32, 0),
					sourceRectangle: new Rectangle(48, 64, 16 * 3, 16),
					color: Color.White
					);
			}

			PrerenderSwitch(spriteBatch, UnderwaterFarCorals!);
			for (int i = 0; i < 24; i++)
			{
				for (int j = 2; j < 8; j++)
				{
					spriteBatch.Draw(
						texture: BackgroundSpritesheet,
						position: new Vector2(i * 16, j * 16),
						sourceRectangle: new Rectangle(96, 96, 16, 16),
						color: Color.White
						);
				}
			}
			for (int i = 0; i < 6; i++)
			{
				spriteBatch.Draw(
					texture: BackgroundSpritesheet,
					position: new Vector2(i * 64, 0),
					sourceRectangle: new Rectangle(96, 64, 16, 16 * 2),
					color: Color.White
					);
				spriteBatch.Draw(
					texture: BackgroundSpritesheet,
					position: new Vector2(i * 64 + 32, 0),
					sourceRectangle: new Rectangle(112, 64, 16, 16 * 2),
					color: Color.White
					);
			}
			PrerenderSwitch(spriteBatch, UnderwaterCloseCorals!);
			for (int i = 0; i < 24; i++)
			{
				for (int j = 2; j < 8; j++)
				{
					spriteBatch.Draw(
						texture: BackgroundSpritesheet,
						position: new Vector2(i * 16, j * 16),
						sourceRectangle: new Rectangle(96, 96, 16, 16),
						color: Color.White
						);
				}
			}
			for (int i = 0; i < 4; i++)
			{
				spriteBatch.Draw(
					texture: BackgroundSpritesheet,
					position: new Vector2(i * 96, 0),
					sourceRectangle: new Rectangle(160, 64, 16 * 2, 16 * 2),
					color: Color.White
					);
				spriteBatch.Draw(
					texture: BackgroundSpritesheet,
					position: new Vector2(i * 96 + 48, 0),
					sourceRectangle: new Rectangle(128, 64, 16 * 2, 16 * 2),
					color: Color.White
					);
			}

			PrerenderSwitch(spriteBatch, UnderwaterTemple1!);
			for(int frame = 0; frame < 8;  frame++){
				for (int row = 0; row < 16; row++)
				{
					int index = (frame + row) & 7;
					int offset = 0;
					switch(index){
						case 0: offset = 1; break;
						case 1: offset = 2; break;
						case 2: offset = 3; break;
						case 3: offset = 3; break;
						case 4: offset = 2; break;
						case 5: offset = 1; break;
						case 6: offset = 0; break;
						case 7: offset = 0; break;
					}
					spriteBatch.Draw(
						texture: BackgroundSpritesheet,
						position: new Vector2(offset, frame * 32 + row * 2),
						sourceRectangle: new Rectangle(0, 80 + row * 2, 32, 2),
						color: Color.White
						);
				}
			}
			PrerenderSwitch(spriteBatch, UnderwaterTemple2!);
			for (int frame = 0; frame < 8; frame++)
			{
				for (int row = 0; row < 16; row++)
				{
					int index = (frame + row) & 7;
					int offset = 0;
					switch (index)
					{
						case 0: offset = 1; break;
						case 1: offset = 2; break;
						case 2: offset = 3; break;
						case 3: offset = 3; break;
						case 4: offset = 2; break;
						case 5: offset = 1; break;
						case 6: offset = 0; break;
						case 7: offset = 0; break;
					}
					spriteBatch.Draw(
						texture: BackgroundSpritesheet,
						position: new Vector2(offset, frame * 32 + row * 2),
						sourceRectangle: new Rectangle(32, 80 + row * 2, 64, 2),
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
			Atlas = null;
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

			UnderwaterSky!.Dispose();
			UnderwaterSky = null;
			UnderwaterFarRocks!.Dispose();
			UnderwaterFarRocks = null;
			UnderwaterCloseRocks!.Dispose();
			UnderwaterCloseRocks = null;
			UnderwaterFarCorals!.Dispose();
			UnderwaterFarCorals = null;
			UnderwaterCloseCorals!.Dispose();
			UnderwaterCloseCorals = null;
			UnderwaterTemple1!.Dispose();
			UnderwaterTemple1 = null;
			UnderwaterTemple2!.Dispose();
			UnderwaterTemple2 = null;
        }
    }
}
