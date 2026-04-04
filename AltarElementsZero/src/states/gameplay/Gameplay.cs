using AltarElementsZero.src.states.gameplay.gameObject;
using AltarElementsZero.src.states.gameplay.level;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay
{
    class Gameplay(
        GraphicsDevice graphicsDevice,
        GameServiceContainer gameServiceContainer,
        IManager manager,
        GameplayPayload payload,
        GlobalAssets globalAssets,
        InputHandler inputHandler
        ) : State<GameplayAssets, GameplayPayload>(
            manager: manager,
            payload: payload,
            assets: new GameplayAssets(graphicsDevice, gameServiceContainer),
            inputHandler: inputHandler,
            globalAssets: globalAssets
            )
    {
        private readonly Level _level = new();

        private readonly Camera _camera = new();

        public override void Enter()
        {
            base.Enter();

            _level.SetAll(new Tile(Tile.Families.Terrain, 2));
            _level.SetTile(5,5,new Tile(Tile.Families.Terrain, 3));

        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_inputHandler.IsDown(Input.Up))
            {
                _camera.Position.Y -= 10;
            }
            if (_inputHandler.IsDown(Input.Down))
            {
                _camera.Position.Y += 10;
            }
			if (_inputHandler.IsDown(Input.Left))
			{
				_camera.Position.X -= 10;
			}
			if (_inputHandler.IsDown(Input.Right))
			{
				_camera.Position.X += 10;
			}
		}
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Render(spriteBatch);

            TextUtilities.DrawText(
                spriteBatch,
                _globalAssets.RomanFont!,
                16,16,
                "GAMEPLAY",
                0,0
                );


        }
        public override void Exit()
        {
            // if allocating on Enter, dispose here
            base.Exit();
        }

        //

        private void Render(SpriteBatch spriteBatch)
        {
            PxPosition cameraPxPosition = _camera.Position.ToPx();
            PxPosition cameraTileRemainder = cameraPxPosition.TileRemainder();
            TilePosition cameraTilePosition = cameraPxPosition.ToTile();

            for (int tileOffsetY = 0; tileOffsetY <= Configuration.Chunk.Tile.Height; tileOffsetY++)
            {
                for (int tileOffsetX = 0; tileOffsetX <= Configuration.Chunk.Tile.Width; tileOffsetX++)
                {
                    Tile tile = _level.GetTile(
                        (int)cameraTilePosition.X + tileOffsetX,
                        (int)cameraTilePosition.Y + tileOffsetY
                        );

                    if (tile.Family == Tile.Families.Terrain) {
                        int spritesheetCol = tile.Member & 0xf;
                        int spritesheetRow = (tile.Member >> 4) & 0xf;

                        Vector2 outputVector = new(
                            Configuration.Tile.Px.Width * tileOffsetX - cameraTileRemainder.X,
                            Configuration.Tile.Px.Height * tileOffsetY - cameraTileRemainder.Y
                            );
                        Rectangle sourceRectangle = new(
                            Configuration.Tile.Px.Width * spritesheetCol,
							Configuration.Tile.Px.Height * spritesheetRow,
							Configuration.Tile.Px.Width,
							Configuration.Tile.Px.Height
							);

						spriteBatch.Draw(
                            _assets.DebugSpritesheet,
							outputVector,
							sourceRectangle,
                            Color.White);
                    }
                }
            }
        }
    }
}
