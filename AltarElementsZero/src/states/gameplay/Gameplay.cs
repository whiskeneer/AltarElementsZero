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

        private readonly TestObject _testObject = new();

        public override void Enter()
        {
            base.Enter();

            _level.SetAll(new Tile(Tile.Families.Terrain, 2));
            for (int j = 1; j <= 6; j++)
            {
                for(int i = 1; i <= 10; i++)
                {
					_level.SetTile(i, j, new Tile(Tile.Families.None, 0));
				}
            }

            _testObject.Position = new TilePosition(2,2).ToPx().ToSubpx();
            

        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_inputHandler.IsDown(Input.Left))
            {
                ApplyForce(_testObject, -1, 0);
            }
			if (_inputHandler.IsDown(Input.Right))
			{
				ApplyForce(_testObject, 1, 0);
			}
			if (_inputHandler.IsDown(Input.Up))
			{
				ApplyForce(_testObject, 0, -1);
			}
			if (_inputHandler.IsDown(Input.Down))
			{
				ApplyForce(_testObject, 0, 1);
			}
            ApplyVelocity(_testObject);

		}
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Render(spriteBatch);

        }
        public override void Exit()
        {
            // if allocating on Enter, dispose here
            base.Exit();
        }

        //

        private static void ApplyForce(GameObject gameObject, int forceX, int forceY)
        {
            gameObject.Velocity.X += forceX;
            gameObject.Velocity.Y += forceY;

			// cap velocity!
		}
        private void ApplyVelocity(GameObject gameObject)
        {
            // can collision calculations be optimized?

            // Vertical collisions
            SubpxPosition destinyPosition = new(
                gameObject.Position.X, // + (uint)gameObject.Velocity.X,
                gameObject.Position.Y + (uint)gameObject.Velocity.Y
                );

            SubpxPosition oppositeVertex = new(
                destinyPosition.X + gameObject.Size.X - 1,
                destinyPosition.Y + gameObject.Size.Y - 1
                );
            TilePosition vertexTile = destinyPosition.ToPx().ToTile();
            TilePosition oppositeTile = oppositeVertex.ToPx().ToTile();

            uint top = vertexTile.Y;
            uint left = vertexTile.X;
            uint bottom = oppositeTile.Y;
            uint right = oppositeTile.X;


            if (gameObject.Velocity.Y > 0) // going down
            {
                bool foundBelow = false;
                for (uint row = top; !foundBelow && row <= bottom; row++)
                {
                    for (uint col = left; !foundBelow && col <= right; col++)
                    {
                        if (_level.GetTile((int)col, (int)row).Family == Tile.Families.Terrain)
                        {
                            destinyPosition.Y = new TilePosition(0, row).ToPx().ToSubpx().Y - gameObject.Size.Y;
                            gameObject.Velocity.Y = 0;
                            foundBelow = true;
                        }
                    }
                }
            }
            else // going up
            {
                bool foundAbove = false;
                for (uint row = bottom; !foundAbove && row >= top; row--)
                {
                    for (uint col = left; !foundAbove && col <= right; col++)
                    {
                        if (_level.GetTile((int)col, (int)row).Family == Tile.Families.Terrain)
                        {
                            destinyPosition.Y = new TilePosition(0, row + 1).ToPx().ToSubpx().Y;
                            gameObject.Velocity.Y = 0;
                            foundAbove = true;
                        }
                    }
                }
            }

            // Horizontal collisions
            destinyPosition.X += (uint)gameObject.Velocity.X;
			oppositeVertex = new(
                destinyPosition.X + gameObject.Size.X - 1,
                destinyPosition.Y + gameObject.Size.Y - 1
                );
            vertexTile = destinyPosition.ToPx().ToTile();
            oppositeTile = oppositeVertex.ToPx().ToTile();
            top = vertexTile.Y;
			left = vertexTile.X;
			bottom = oppositeTile.Y;
			right = oppositeTile.X;

			if (gameObject.Velocity.X > 0) // going right
			{
				bool foundAtRight = false;
				for (uint col = left; !foundAtRight && col <= right; col++)
				{
				    for (uint row = top; !foundAtRight && row <= bottom; row++)
					{
						if (_level.GetTile((int)col, (int)row).Family == Tile.Families.Terrain)
						{
							destinyPosition.X = new TilePosition(col, 0).ToPx().ToSubpx().X - gameObject.Size.X;
							gameObject.Velocity.X = 0;
							foundAtRight = true;
						}
					}
				}
			}
			else // going left
			{
				bool foundAtLeft = false;
				for (uint col = right; !foundAtLeft && col >= left; col--)
				{
				    for (uint row = top; !foundAtLeft && row <= bottom; row++)
					{
						if (_level.GetTile((int)col, (int)row).Family == Tile.Families.Terrain)
						{
							destinyPosition.X = new TilePosition(col+1, 0).ToPx().ToSubpx().X;
							gameObject.Velocity.X = 0;
							foundAtLeft = true;
						}
					}
				}
			}




			gameObject.Position = destinyPosition;
        }
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

            PxPosition testObjectPxPosition = _testObject.Position.ToPx();
            spriteBatch.Draw(
                texture: _assets.DebugSpritesheet,
                position: new Vector2(testObjectPxPosition.X, testObjectPxPosition.Y),
                sourceRectangle: new(
                    Configuration.Tile.Px.Width * 4,
                    Configuration.Tile.Px.Height * 0,
                    Configuration.Tile.Px.Width,
                    Configuration.Tile.Px.Height
                    ),
                color: Color.White
                );

		}
    }
}
