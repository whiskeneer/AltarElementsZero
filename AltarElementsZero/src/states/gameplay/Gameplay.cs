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
                for(int i = 1; i < Configuration.Level.Tile.Width - 1; i++)
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
                _testObject.FeetVelocity = new(-64, 0);
            }
            else if (_inputHandler.IsDown(Input.Right)) 
            {
                _testObject.FeetVelocity = new(64, 0);
            }
            else{
				_testObject.FeetVelocity = new(0, 0);
			}


			//      STEP 1: directly applied forces and fluid medium friction forces

			//      RESETTING PREVIOUSLY APPLIED FORCES

			_testObject.ResetForces();

			//      DIRECTLY APPLIED FORCES
			// e.g.: gravity, magnetism, etc.

			_testObject.ApplyForce(new Force(0, 10));

            //      FLUID MEDIUM FRICTION FORCES
            // e.g.: air, water, etc.
            // - They depend on the relative velocity of the object to the medium!
            //   ( proportional to relative_velocity ^ 2 )

            // TODO: Get fluid velocity from medium!
            _testObject.ApplyFluidFriction(20, _testObject.Velocity - new SubpxVelocity(16,0));

            //      UPDATING VELOCITY
            SubpxVelocity velocityBeforeFirstForces = _testObject.Velocity;
			_testObject.UpdateVelocity();

            //      Step 2: terrain friction forces

            //      RESETTING PREVIOUSLY APPLIED FORCES
            Force forcesBeforeTerrainFriction = _testObject.AppliedForces;
            _testObject.ResetForces();

			//      TERRAIN FRICTION FORCES
			// e.g.: ground, ice, moving platforms, etc.
			// - They depend on the relative velocity of the object to the terrain!
			//   ( if prev_relative_velocity = 0 => capped at max static friction force and (target)relative_velocity )
			//   ( if prev_relative_velocity!= 0 => capped at (target)relative_velocity )
			//   ( in both cases, max is proportional to normal force )

			// previousRelativeVelocity: used for determine whether the friction is Kinematic or Static
			SubpxVelocity previousRelativeVelocity = velocityBeforeFirstForces - _testObject.GroundVelocity - _testObject.FeetVelocity;
			// targetRelativeVelocity: used for capping friction force (so that it doesn't start going "backwards" just by friction
			SubpxVelocity targetRelativeVelocity = _testObject.Velocity - _testObject.GroundVelocity - _testObject.FeetVelocity;
            if (_testObject.Grounded && forcesBeforeTerrainFriction.Y > 0)
            {
                if(previousRelativeVelocity.X == 0) // STATIC FRICTION
                {
                    int staticFriction = Math.Min(
                        Math.Abs(targetRelativeVelocity.X),
                        (_testObject.GroundMuSta * forcesBeforeTerrainFriction.Y) >> 8
                        );
                    _testObject.ApplyForce(new Force(
                        staticFriction * -Math.Sign(targetRelativeVelocity.X),
                        0
                        ));

					_testObject.UpdateVelocity();
				}
				else // KINEMATIC FRICTION
                {
					int kinematicFriction = Math.Min(
						Math.Abs(targetRelativeVelocity.X),
						(_testObject.GroundMuKin * forcesBeforeTerrainFriction.Y) >> 8
						);
					_testObject.ApplyForce(new Force(
						kinematicFriction * -Math.Sign(targetRelativeVelocity.X),
						0
						));

					_testObject.UpdateVelocity();
				}
			}


			MoveAndApplyCollision(_testObject);

            SubpxPosition cameraPosition = _testObject.Position;
            cameraPosition.X -= (uint)(Configuration.VisibleScreen.Px.Width << (Configuration.Px.SubpxPower-1));
			cameraPosition.Y -= (uint)(Configuration.VisibleScreen.Px.Height << (Configuration.Px.SubpxPower-1));
            if (cameraPosition.X > Configuration.Level.Subpx.Width)
            {
                cameraPosition.X = 0;
            }
            if(cameraPosition.Y > Configuration.Level.Subpx.Height)
            {
                cameraPosition.Y = 0;
            }

            _camera.Position = cameraPosition;

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

        private void MoveAndApplyCollision(GameObject gameObject)
        {
            // can collision calculations be optimized?
            SubpxPosition targetPosition = gameObject.TargetPosition();

            // Vertical collisions
            SubpxPosition checkingVertex = new(
                gameObject.Position.X,
                targetPosition.Y
                );

            SubpxPosition oppositeVertex = new(
                checkingVertex.X + gameObject.Size.X - 1,
                checkingVertex.Y + gameObject.Size.Y - 1
                );
            TilePosition checkingTile = checkingVertex.ToPx().ToTile();
            TilePosition oppositeTile = oppositeVertex.ToPx().ToTile();

            uint top = checkingTile.Y;
            uint left = checkingTile.X;
            uint bottom = oppositeTile.Y;
            uint right = oppositeTile.X;

            gameObject.Grounded = false;

            if (gameObject.Velocity.Y > 0) // going down
            {
                bool foundBelow = false;
                for (uint row = top; !foundBelow && row <= bottom; row++)
                {
                    for (uint col = left; !foundBelow && col <= right; col++)
                    {
                        if (_level.GetTile((int)col, (int)row).Family == Tile.Families.Terrain)
                        {
                            checkingVertex.Y = new TilePosition(0, row).ToPx().ToSubpx().Y - gameObject.Size.Y;
                            gameObject.Velocity.Y = 0;
                            foundBelow = true;
                            gameObject.Grounded = true;
							// TODO: get GroundMuKin and GroundMuSta
							gameObject.GroundMuKin = 100;
                            gameObject.GroundMuSta = 200;
                            gameObject.GroundVelocity = new SubpxVelocity(-10,0); // zero, because terrain is immobile ground
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
                            checkingVertex.Y = new TilePosition(0, row + 1).ToPx().ToSubpx().Y;
                            gameObject.Velocity.Y = 0;
                            foundAbove = true;
                        }
                    }
                }
            }

            // Horizontal collisions
            checkingVertex.X = targetPosition.X;
			oppositeVertex = new(
                checkingVertex.X + gameObject.Size.X - 1,
                checkingVertex.Y + gameObject.Size.Y - 1
                );
            checkingTile = checkingVertex.ToPx().ToTile();
            oppositeTile = oppositeVertex.ToPx().ToTile();
            top = checkingTile.Y;
			left = checkingTile.X;
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
							checkingVertex.X = new TilePosition(col, 0).ToPx().ToSubpx().X - gameObject.Size.X;
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
							checkingVertex.X = new TilePosition(col+1, 0).ToPx().ToSubpx().X;
							gameObject.Velocity.X = 0;
							foundAtLeft = true;
						}
					}
				}
			}




			gameObject.Position = checkingVertex;
        }
        private void Render(SpriteBatch spriteBatch)
        {
            // TODO: add SubpxPosition.ToVisualPx()
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
                position: new Vector2(
                    testObjectPxPosition.X - cameraPxPosition.X, 
                    testObjectPxPosition.Y - cameraPxPosition.Y
					),
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
