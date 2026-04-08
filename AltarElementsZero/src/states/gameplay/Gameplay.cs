using AltarElementsZero.src.states.gameplay.gameObject;
using AltarElementsZero.src.states.gameplay.level;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

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

        private readonly GameObject _camera = new();

        private readonly GameObject _testObject = GameObject.GetTestObject();
        private int _remainingJumpFrames = 0;
        private int _attackCooldown = 0;

        private readonly GameObject[] _objectPool = new GameObject[64];

        uint _animationFrame = 0;

        public override void Enter()
        {
            base.Enter();

            Random rnd = new();
            _level.SetAll(new Tile(Tile.Families.Ground, 0));
            for (int j = 1; j <= 6; j++)
            {
                for(int i = 1; i <= 10; i++)
                {
                    _level.SetTile(i, j, new Tile(Tile.Families.None, 0));
				}
            }

            for(int o = 0; o < _objectPool.Length; o++)
            {
                _objectPool[o] = new();
            }

            _testObject.Position = new TilePosition(1,1).ToPx().ToSubpx();

            for(int o = 0; o < 1; o++)
            {
                _objectPool[o] = GameObject.GetToki();
                _objectPool[o].Init();
                _objectPool[o].Position = new PxPosition((uint)rnd.Next(32,100), (uint)rnd.Next(32,80)).ToSubpx();

            }

            //for(int o = 0; o < _objectPool.Length; o++)
            //{

            //    _objectPool[o].Exist = true;
            //    _objectPool[o].IsMobile = true;
            //    _objectPool[o].IsSolid = true;
            //    _objectPool[o].IsVisible = true;
            //    _objectPool[o].Position = new TilePosition((uint)(3+o), 3).ToPx().ToSubpx();
            //    _objectPool[o].Size = new PxSize(16,16).ToSubpx();

            //}

        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Checking medium velocity should be done after moving the object
            _testObject.MediumVelocity = new(0, 0);
            //




            if (_inputHandler.IsDown(Input.Left))
            {
                _testObject.ApplyWingVelocity(new SubpxVelocity(-64 * 3, 0));
                _testObject.FeetVelocity = new(-64 * 2, 0);
            }
            else if (_inputHandler.IsDown(Input.Right))
            {
                _testObject.ApplyWingVelocity(new SubpxVelocity(64 * 3, 0));
				_testObject.FeetVelocity = new(64 * 2, 0);
            }
            else {
				_testObject.ApplyWingVelocity(new SubpxVelocity(0, 0));
				_testObject.FeetVelocity = new(0, 0);
            }


			//      STEP 1: directly applied forces and fluid medium friction forces

			//      RESETTING PREVIOUSLY APPLIED FORCES

			_testObject.ResetForces();

			//      DIRECTLY APPLIED FORCES
			// e.g.: gravity, magnetism, etc.

			if (_inputHandler.IsPressed(Input.Jump) && _testObject.Grounded)
			{
				_remainingJumpFrames = 12;
                _testObject.ApplyForce(new Force(0, -150));
			}
			if (_inputHandler.IsReleased(Input.Jump))
			{
				_remainingJumpFrames = 0;
			}

            if(_remainingJumpFrames == 0)
            {
                // GRAVITY!
			    _testObject.ApplyForce(new Force(0, 12));
            }
            else
            {
                _remainingJumpFrames--;
            }

            if(_inputHandler.IsPressed(Input.Attack) && _attackCooldown == 0)
            {
				_remainingJumpFrames = 0;

				if (_inputHandler.IsDown(Input.Up))
                {
					_testObject.ApplyForce(new Force(0, -64));
				}
                else if (_inputHandler.IsDown(Input.Down))
                {
					_testObject.ApplyForce(new Force(0, 64));
				}
				else if (_inputHandler.IsDown(Input.Left))
                {
                    _testObject.ApplyForce(new Force(-64, 0));
                }
                else if (_inputHandler.IsDown(Input.Right))
                {
					_testObject.ApplyForce(new Force(64, 0));
				}
				_attackCooldown = 30;
			}
            else if (_attackCooldown > 0)
            {
                _attackCooldown--;
            }

			//      FLUID MEDIUM FRICTION FORCES
			// e.g.: air, water, etc.
			// - They depend on the relative velocity of the object to the medium!
			//   ( proportional to relative_velocity ^ 2 )

			// TODO: Get fluid velocity from medium!
			_testObject.ApplyFluidFriction(0, _testObject.Velocity - _testObject.MediumVelocity);// (16,0));

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

            // CAMERA UPDATE

   //         SubpxPosition cameraPosition = _testObject.Position;
   //         cameraPosition.X -= (uint)(Configuration.VisibleScreen.Px.Width << (Configuration.Px.SubpxPower-1));
			//cameraPosition.Y -= (uint)(Configuration.VisibleScreen.Px.Height << (Configuration.Px.SubpxPower-1));
   //         if (cameraPosition.X > Configuration.Level.Subpx.Width)
   //         {
   //             cameraPosition.X = 0;
   //         }
   //         if(cameraPosition.Y > Configuration.Level.Subpx.Height)
   //         {
   //             cameraPosition.Y = 0;
   //         }

   //         _camera.Position = cameraPosition;


            //
            for (int o = 0; o < _objectPool.Length; o++)
            {
                GameObject gameObject = _objectPool[o];
                if (gameObject.exists && gameObject.isSolid && !gameObject.isFixed)
                {
                    gameObject.ApplyWingVelocity(new SubpxVelocity());

                    gameObject.ResetForces();
                    gameObject.ApplyForce(new Force(0, 12));

                    gameObject.Update();

                    gameObject.ApplyFluidFriction(0, gameObject.Velocity - gameObject.MediumVelocity);
					SubpxVelocity _velocityBeforeFirstForces = gameObject.Velocity;
                    gameObject.UpdateVelocity();

                    Force _forcesBeforeTerrainFriction = gameObject.AppliedForces;
                    gameObject.ResetForces();
					SubpxVelocity _previousRelativeVelocity = _velocityBeforeFirstForces - gameObject.GroundVelocity - gameObject.FeetVelocity;
					SubpxVelocity _targetRelativeVelocity = gameObject.Velocity - gameObject.GroundVelocity - gameObject.FeetVelocity;
					if (gameObject.Grounded && _forcesBeforeTerrainFriction.Y > 0)
                    {
						if (_previousRelativeVelocity.X == 0) // STATIC FRICTION
						{
							int staticFriction = Math.Min(
								Math.Abs(_targetRelativeVelocity.X),
								(gameObject.GroundMuSta * _forcesBeforeTerrainFriction.Y) >> 8
								);
							gameObject.ApplyForce(new Force(
								staticFriction * -Math.Sign(_targetRelativeVelocity.X),
								0
								));

							gameObject.UpdateVelocity();
						}
						else // KINEMATIC FRICTION
						{
							int kinematicFriction = Math.Min(
								Math.Abs(_targetRelativeVelocity.X),
								(gameObject.GroundMuKin * _forcesBeforeTerrainFriction.Y) >> 8
								);
							gameObject.ApplyForce(new Force(
								kinematicFriction * -Math.Sign(_targetRelativeVelocity.X),
								0
								));

							gameObject.UpdateVelocity();
						}
					}

                    MoveAndApplyCollision(gameObject);
				}
			}
            //

		}
		public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            _animationFrame++;

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

			uint currentTop = checkingVertex.Y;
			uint currentBottom = oppositeVertex.Y;
			uint currentLeft = checkingVertex.X;
			uint currentRight = oppositeVertex.X;

			gameObject.Grounded = false;

            if (gameObject.Velocity.Y > 0) // going down
            {
                bool foundBelow = false;
                uint foundAtY = 0;
                for (uint row = top; !foundBelow && row <= bottom; row++)
                {
                    for (uint col = left; !foundBelow && col <= right; col++)
                    {
                        Tile tile = _level.GetTile((int)col, (int)row);

						Tile.Families family = tile.Family;
						if (family >= Tile.Families.Ground && tile.Family <= Tile.Families.ConveyorLeft)
						{
                            foundBelow = true;
                            foundAtY = new TilePosition(0, row).ToPx().ToSubpx().Y;
                            checkingVertex.Y = foundAtY - gameObject.Size.Y;

                            gameObject.Velocity.Y = 0;

                            gameObject.Grounded = true;

                            if (tile.Family == Tile.Families.Ground)
                            {
                                gameObject.GroundMuKin = 200;
                                gameObject.GroundMuSta =  400;
                                gameObject.GroundVelocity = new SubpxVelocity(0,0);// (-100,0); // zero, because terrain is immobile ground
                            }
                            else if(tile.Family == Tile.Families.Ice)
                            {
								gameObject.GroundMuKin = 0;
								gameObject.GroundMuSta = 0;
								gameObject.GroundVelocity = new SubpxVelocity(0, 0);// (-100,0); // zero, because terrain is immobile ground
							}
                            else if (tile.Family == Tile.Families.ConveyorRight)
                            {
								gameObject.GroundMuKin = 200;
								gameObject.GroundMuSta = 400;
								gameObject.GroundVelocity = new SubpxVelocity(64<<(tile.Member & 0x3), 0);
							}
                            else if(tile.Family == Tile.Families.ConveyorLeft)
                            {
								gameObject.GroundMuKin = 200;
								gameObject.GroundMuSta = 400;
								gameObject.GroundVelocity = new SubpxVelocity(-(64<<(tile.Member & 0x3)), 0);
							}
                        }
                    }
                }

				for (int o = 0; o < _objectPool.Length; o++)
                {
                    GameObject otherGameObject = _objectPool[o];
                    if (otherGameObject.exists && !object.ReferenceEquals(otherGameObject, gameObject))
                    {
                        SubpxPosition otherVertex = otherGameObject.Position;
                        SubpxPosition otherOppositeVertex = otherGameObject.Position + otherGameObject.Size - new SubpxSize(1,1);

                        uint otherTop = otherVertex.Y;
                        uint otherBottom = otherOppositeVertex.Y;
                        uint otherLeft = otherVertex.X;
                        uint otherRight = otherOppositeVertex.X;
                    
                        if((!foundBelow || otherTop < foundAtY) &&
                            currentTop <= otherBottom && otherTop <= currentBottom &&
                            currentLeft <= otherRight && otherLeft <= currentRight)
                        {
                            foundBelow = true;
                            foundAtY = otherTop;
                            checkingVertex.Y  = foundAtY - gameObject.Size.Y;

							gameObject.Velocity.Y = 0;
							gameObject.Grounded = true;
							// TODO: get GroundMuKin and GroundMuSta
							gameObject.GroundMuKin = 200;
							gameObject.GroundMuSta = 400;
                            gameObject.GroundVelocity = otherGameObject.Velocity;
						}
                    }
                }

            }
            else // going up
            {
				bool foundAbove = false;
				uint foundAtY = 0;
                for (uint row = bottom; !foundAbove && row >= top; row--)
                {
                    for (uint col = left; !foundAbove && col <= right; col++)
                    {
						Tile tile = _level.GetTile((int)col, (int)row);

						Tile.Families family = tile.Family;
						if (family >= Tile.Families.Ground && tile.Family <= Tile.Families.ConveyorLeft)
						{
                            foundAbove = true;
                            foundAtY = new TilePosition(0, row + 1).ToPx().ToSubpx().Y - 1;
							checkingVertex.Y = foundAtY + 1;
                            gameObject.Velocity.Y = 0;
                        }
                    }
                }

				for (int o = 0; o < _objectPool.Length; o++)
				{
					GameObject otherGameObject = _objectPool[o];
					if (otherGameObject.exists && !object.ReferenceEquals(otherGameObject, gameObject))
					{
						SubpxPosition otherVertex = otherGameObject.Position;
						SubpxPosition otherOppositeVertex = otherGameObject.Position + otherGameObject.Size - new SubpxSize(1, 1);

						uint otherTop = otherVertex.Y;
						uint otherBottom = otherOppositeVertex.Y;
						uint otherLeft = otherVertex.X;
						uint otherRight = otherOppositeVertex.X;

						if ((!foundAbove || otherBottom > foundAtY) &&
							currentTop <= otherBottom && otherTop <= currentBottom &&
							currentLeft <= otherRight && otherLeft <= currentRight)
						{
							foundAbove = true;
							foundAtY = otherBottom;
							checkingVertex.Y = otherBottom + 1;

							gameObject.Velocity.Y = 0;
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

			currentTop = checkingVertex.Y;
			currentBottom = oppositeVertex.Y;
			currentLeft = checkingVertex.X;
			currentRight = oppositeVertex.X;

            if (gameObject.Velocity.X > 0) // going right
            {
                bool foundAtRight = false;
                uint foundAtX = 0;
                for (uint col = left; !foundAtRight && col <= right; col++)
                {
                    for (uint row = top; !foundAtRight && row <= bottom; row++)
                    {
						Tile tile = _level.GetTile((int)col, (int)row);

                        Tile.Families family = tile.Family;
						if (family >= Tile.Families.Ground && tile.Family <= Tile.Families.ConveyorLeft)
						{
                            foundAtRight = true;
                            foundAtX = new TilePosition(col, 0).ToPx().ToSubpx().X;
                            checkingVertex.X = foundAtX - gameObject.Size.X;
                            gameObject.Velocity.X = 0;
                        }
                    }
                }
                for (int o = 0; o < _objectPool.Length; o++)
                {
                    GameObject otherGameObject = _objectPool[o];
                    if (otherGameObject.exists && !object.ReferenceEquals(otherGameObject, gameObject))
                    {
                        SubpxPosition otherVertex = otherGameObject.Position;
                        SubpxPosition otherOppositeVertex = otherGameObject.Position + otherGameObject.Size - new SubpxSize(1, 1);

                        uint otherTop = otherVertex.Y;
                        uint otherBottom = otherOppositeVertex.Y;
                        uint otherLeft = otherVertex.X;
                        uint otherRight = otherOppositeVertex.X;

                        if ((!foundAtRight || otherLeft < foundAtX) &&
                            currentTop <= otherBottom && otherTop <= currentBottom &&
                            currentLeft <= otherRight && otherLeft <= currentRight)
                        {
                            foundAtRight = true;
                            foundAtX = otherLeft;
                            checkingVertex.X = foundAtX - gameObject.Size.X;

                            gameObject.Velocity.X = 0;
                        }
                    }
                }
            }
            else // going left
            {
                bool foundAtLeft = false;
                uint foundAtX = 0;
                for (uint col = right; !foundAtLeft && col >= left; col--)
                {
                    for (uint row = top; !foundAtLeft && row <= bottom; row++)
                    {
                        Tile tile = _level.GetTile((int)col, (int)row);

						Tile.Families family = tile.Family;
						if (family >= Tile.Families.Ground && tile.Family <= Tile.Families.ConveyorLeft)
						{
                            foundAtLeft = true;
                            foundAtX = new TilePosition(col + 1, 0).ToPx().ToSubpx().X - 1;
                            checkingVertex.X = foundAtX + 1;
                            gameObject.Velocity.X = 0;
                        }
                    }
                }

                for (int o = 0; o < _objectPool.Length; o++)
                {
                    GameObject otherGameObject = _objectPool[o];
                    if (otherGameObject.exists && !object.ReferenceEquals(otherGameObject, gameObject))
                    {
                        SubpxPosition otherVertex = otherGameObject.Position;
                        SubpxPosition otherOppositeVertex = otherGameObject.Position + otherGameObject.Size - new SubpxSize(1, 1);

                        uint otherTop = otherVertex.Y;
                        uint otherBottom = otherOppositeVertex.Y;
                        uint otherLeft = otherVertex.X;
                        uint otherRight = otherOppositeVertex.X;

                        if ((!foundAtLeft || otherRight > foundAtX) &&
                            currentTop <= otherBottom && otherTop <= currentBottom &&
                            currentLeft <= otherRight && otherLeft <= currentRight)
                        {
                            foundAtLeft = true;
                            foundAtX = otherRight;
                            checkingVertex.X = otherRight + 1;

                            gameObject.Velocity.X = 0;
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

                    if (tile.Family >= Tile.Families.Ground && tile.Family <= Tile.Families.Spikes) {
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
                            _assets.StaticSpritesheet,
							outputVector,
							sourceRectangle,
                            Color.White);
                    }
                    else if(tile.Family >= Tile.Families.ConveyorRight && tile.Family <= Tile.Families.ConveyorLeft)
                    {
						int spritesheetCol = (tile.Member & 0xc) | (((int)_animationFrame >> (3 - (tile.Member & 0x3))) & 0x3);
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
                            _assets.AnimatedSpritesheet,
							outputVector,
							sourceRectangle,
                            Color.White);

					}

                }
            }

            PxPosition testObjectPxPosition = _testObject.Position.ToPx();
            spriteBatch.Draw(
                texture: _assets.StaticSpritesheet,
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

            for(int o = 0; o < _objectPool.Length; o++)
            {
                GameObject currentObject = _objectPool[o];
                if (currentObject.exists && currentObject.isVisible)
                {
                    PxPosition objectPosition = currentObject.Position.ToPx() - currentObject.SpriteOffset;
                    uint spritesheetIndex = currentObject.spritesheetIndex;
                    SpriteEffects spriteEffects = currentObject.spriteEffects;
                    spriteBatch.Draw(
                        texture: _assets.ObjectSpritesheet,
                        position: new Vector2(
                            (int)objectPosition.X - cameraPxPosition.X,
                            (int)objectPosition.Y - cameraPxPosition.Y
                            ),
                        sourceRectangle: new(
                            Configuration.Tile.Px.Width * 2 * (int)(spritesheetIndex & 0x7),
                            Configuration.Tile.Px.Height * 2 * (int)(spritesheetIndex >> 3),
                            Configuration.Tile.Px.Width * 2,
                            Configuration.Tile.Px.Height * 2
                            ),
                        color: Color.White, 
                        0f,Vector2.Zero,1f,spriteEffects,0f
                        );
				}
            }

		}
    }
}
