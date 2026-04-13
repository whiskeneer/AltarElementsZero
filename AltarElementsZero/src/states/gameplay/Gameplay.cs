using AltarElementsZero.src.states.gameplay.gameObject;
using AltarElementsZero.src.states.gameplay.level;
using AltarElementsZero.src.states.gameplay.vectors;
using AltarElementsZero.src.renderer;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//using System;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.enemies;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.gimmicks;
using AltarElementsZero.src.states.intro;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.debug;

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
        private readonly Level _level = new("assets/lvl/DEBUG_LEVEL.json");

        private readonly GameObject _camera = new();

        private bool frameByFrameMode = false;

        //private readonly GameObject _testObject = GameObject.GetTestObject();
        //private int _remainingJumpFrames = 0;
        //private int _attackCooldown = 0;

        private bool _drawIndices = false;

        private readonly GameObject[] _objectPool = new GameObject[64];

        uint _animationFrame = 0;

        public override void Enter()
        {
            GameObject.inputHandler = _inputHandler;

            base.Enter();

            //Random rnd = new();

            for(int o = 0; o < _objectPool.Length; o++)
            {
                _objectPool[o] = new();
            }

            int nextAssignableObject = 0;
            for (int j = 0; j < Configuration.Level.Tile.Height && nextAssignableObject < _objectPool.Length; j++)
            {
                for (int i = 0; i < Configuration.Level.Tile.Width && nextAssignableObject < _objectPool.Length; i++)
                {
                    Tile tile = _level.GetTile(i, j);
                    if (tile.IsObjectSpawn())
                    {
                        if(tile.Family == Tile.Families.Toki)
                        {
                            _objectPool[nextAssignableObject].behaviour = Toki.Instance;
                            _objectPool[nextAssignableObject].Init();
                            _objectPool[nextAssignableObject].currentBoundingBox.Position = new TilePosition((uint)i, (uint)j).ToPx().ToSubpx();
							nextAssignableObject++;
						}
						else if(tile.Family == Tile.Families.MovingPlatform1)
                        {
							_objectPool[nextAssignableObject].behaviour = MovingPlatform1.Instance;
							_objectPool[nextAssignableObject].Init();
							_objectPool[nextAssignableObject].currentBoundingBox.Position = new TilePosition((uint)i, (uint)j).ToPx().ToSubpx();
							nextAssignableObject++;
                        }
                        else if(tile.Family == Tile.Families.DebugBox)
                        {
							_objectPool[nextAssignableObject].behaviour = DebugBox.Instance;
							_objectPool[nextAssignableObject].Init();
							_objectPool[nextAssignableObject].currentBoundingBox.Position = new TilePosition((uint)i, (uint)j).ToPx().ToSubpx();
							nextAssignableObject++;
						}
						else if (tile.Family == Tile.Families.DebugPusher)
						{
							_objectPool[nextAssignableObject].behaviour = DebugPusher.Instance;
							_objectPool[nextAssignableObject].Init();
							_objectPool[nextAssignableObject].currentBoundingBox.Position = new TilePosition((uint)i, (uint)j).ToPx().ToSubpx();
							nextAssignableObject++;
						}
						else if(tile.Family == Tile.Families.DebugImmobile)
						{
							_objectPool[nextAssignableObject].behaviour = DebugImmobile.Instance;
							_objectPool[nextAssignableObject].Init();
							_objectPool[nextAssignableObject].currentBoundingBox.Position = new TilePosition((uint)i, (uint)j).ToPx().ToSubpx();
							nextAssignableObject++;
						}

						else if(tile.Family == Tile.Families.FanUp)
						{
							_objectPool[nextAssignableObject].behaviour = CurrentUp.Instance;
							_objectPool[nextAssignableObject].Init();
							_objectPool[nextAssignableObject].currentBoundingBox.Position = new TilePosition((uint)i, (uint)j-2).ToPx().ToSubpx();
							nextAssignableObject++;
						}
					}
                }
            }


        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);


            if (_inputHandler.IsPressed(Input.Pause))
            {
                _manager.RequestTransition(new IntroPayload("IM BACK"));
            }

            _drawIndices = _inputHandler.IsDown(Input.Dash);

            if (_inputHandler.IsPressed(Input.Attack))
            {
                frameByFrameMode = !frameByFrameMode;
            }

            if(frameByFrameMode && !_inputHandler.IsPressed(Input.Jump))
            {
                return;
            }


            CalculateDesiredOutcomes();
            ApplyHorizontalVelocities();
            CheckHorizontalCollisions();
            ApplyVerticalVelocities();
            CheckVerticalCollisions();
            SeparatePushables();
			SeparatePushablesFromImmobile();
			

			if (frameByFrameMode && _inputHandler.IsPressed(Input.Jump))
            {
                for(int o = 0; o < _objectPool.Length; o++)
                {
                    GameObject gameObject = _objectPool[o];
                    if (gameObject.Type == GameObject.Types.NONEXISTENT) continue;
                    Console.Write($"{o} : UP={gameObject.PushedUp} DN={gameObject.PushedDown} LF={gameObject.PushedLeft} RH={gameObject.PushedRight} ");
                    Console.WriteLine($"P-UP={gameObject.PushedPreviouslyUp} P-DN={gameObject.PushedPreviouslyDown} P-LF={gameObject.PushedPreviouslyLeft} P-RH={gameObject.PushedPreviouslyRight}");
                }
            }

		}

        
        private void CalculateDesiredOutcomes()
        {
			for (int o = 0; o < _objectPool.Length; o++)
			{
				GameObject gameObject = _objectPool[o];
				if (gameObject.Type != GameObject.Types.NONEXISTENT)
				{
					gameObject.SavePreviousValues();
					gameObject.CalculateDesiredOutcome();

					gameObject.VelocityBelow = 0;
					gameObject.FrictionCoefficientsBelow = new();
					gameObject.VelocityAround = new();
					gameObject.FrictionCoefficientAround = 0;

					gameObject.AppliedForces = new();

				}
			}
		}

        private void ApplyHorizontalVelocities()
        {
			for (int o = 0; o < _objectPool.Length; o++)
			{
				GameObject gameObject = _objectPool[o];
				if (gameObject.Type != GameObject.Types.NONEXISTENT)
				{
					gameObject.ApplyHorizontalDesiredVelocity();
				}
			}
		}

        private void CheckHorizontalCollisions()
        {
			for (int o = 0; o < _objectPool.Length; o++)
			{
				GameObject go1 = _objectPool[o];
				if (go1.Type == GameObject.Types.NONEXISTENT) continue;
				go1.CleanHorizontalPushFlags();
				for (int u = o + 1; u < _objectPool.Length; u++)
				{
					GameObject go2 = _objectPool[u];
					if (go2.Type == GameObject.Types.NONEXISTENT) continue;

					GameObject.CheckHorizontalCollisions(go1, go2);
				}

				if (go1.Type != GameObject.Types.PUSHABLE) continue;

				TileSpan tileSpan = go1.currentBoundingBox.GetTileSpan();
                if(go1.currentVelocity.X > 0)
                { // going right
                    bool foundCollision = false;

                    for(int col = (int)tileSpan.Left;
                        col <= (int)tileSpan.Right && !foundCollision;
                        col++)
                    {
                        for(int row  = (int)tileSpan.Top;
                            row <= (int)tileSpan.Bottom && !foundCollision;
                            row++)
                        {
                            Tile tile = _level.GetTile(col,row);
                            if (!tile.IsSolid()) continue;

                            ObjectBoundingBox tileBoundingBox = ObjectBoundingBox.FromTile((uint)col, (uint)row);
                            if (go1.currentBoundingBox & tileBoundingBox)
                            {
                                go1.currentBoundingBox.LeanAtLeft(tileBoundingBox, (uint)Configuration.Tile.Subpx.Width);
								go1.FixHorizontalVelocity();
								go1.PushedLeft = true;
								foundCollision = true;
                            }

                        }
                    }
                }
                else
                { // going left (or idle)
					bool foundCollision = false;

					for (int col = (int)tileSpan.Right;
						col >= (int)tileSpan.Left && !foundCollision;
						col--)
					{
						for (int row = (int)tileSpan.Top;
							row <= (int)tileSpan.Bottom && !foundCollision;
							row++)
						{
							Tile tile = _level.GetTile(col, row);
							if (!tile.IsSolid()) continue;

							ObjectBoundingBox tileBoundingBox = ObjectBoundingBox.FromTile((uint)col, (uint)row);
							if (go1.currentBoundingBox & tileBoundingBox)
							{
								go1.currentBoundingBox.LeanAtRight(tileBoundingBox, (uint)Configuration.Tile.Subpx.Width);
								go1.FixHorizontalVelocity();
								go1.PushedRight = true;
								foundCollision = true;
							}

						}
					}
				}


			}
		}

        private void ApplyVerticalVelocities()
        {
			for (int o = 0; o < _objectPool.Length; o++)
			{
				GameObject gameObject = _objectPool[o];
				if (gameObject.Type != GameObject.Types.NONEXISTENT)
				{
					gameObject.ApplyVerticalDesiredVelocity();
				}
			}
		}

        private void CheckVerticalCollisions()
        {
			for (int o = 0; o < _objectPool.Length; o++)
			{
				GameObject go1 = _objectPool[o];
				if (go1.Type == GameObject.Types.NONEXISTENT) continue;
				go1.CleanVerticalPushFlags();
				for (int u = o + 1; u < _objectPool.Length; u++)
				{
					GameObject go2 = _objectPool[u];
					if (go2.Type == GameObject.Types.NONEXISTENT) continue;

					GameObject.CheckVerticalCollisions(go1, go2);
				}

				if (go1.Type != GameObject.Types.PUSHABLE) continue;

				TileSpan tileSpan = go1.currentBoundingBox.GetTileSpan();
				if (go1.currentVelocity.Y > 0)
				{ // going down
					bool foundCollision = false;

					for (int row = (int)tileSpan.Top;
						row <= (int)tileSpan.Bottom && !foundCollision;
						row++)
					{
						for (int col = (int)tileSpan.Left;
							col <= (int)tileSpan.Right && !foundCollision;
							col++)
						{
							Tile tile = _level.GetTile(col, row);
							if (!tile.IsSolid()) continue;

							ObjectBoundingBox tileBoundingBox = ObjectBoundingBox.FromTile((uint)col, (uint)row);
							if (go1.currentBoundingBox & tileBoundingBox)
							{
								go1.currentBoundingBox.LeanAbove(tileBoundingBox, (uint)Configuration.Tile.Subpx.Height);
								go1.FixVerticalVelocity();
								go1.PushedUp = true;
								foundCollision = true;

								go1.VelocityBelow = tile.GetSurfaceVelocityAbove();
								go1.FrictionCoefficientsBelow = tile.GetFrictionCoefficients();

							}

						}
					}
				}
				else
				{ // going up (or idle)
					bool foundCollision = false;

					for (int row = (int)tileSpan.Bottom;
						row >= (int)tileSpan.Top && !foundCollision;
						row--)
					{
						for (int col = (int)tileSpan.Left;
							col <= (int)tileSpan.Right && !foundCollision;
							col++)
						{
							Tile tile = _level.GetTile(col, row);
							if (!tile.IsSolid()) continue;

							ObjectBoundingBox tileBoundingBox = ObjectBoundingBox.FromTile((uint)col, (uint)row);
							if (go1.currentBoundingBox & tileBoundingBox)
							{
								go1.currentBoundingBox.LeanBelow(tileBoundingBox, (uint)Configuration.Tile.Subpx.Height);
								go1.FixVerticalVelocity();
								go1.PushedDown = true;
								foundCollision = true;
							}

						}
					}
				}
			}
		}

        private void SeparatePushables()
        {
			for (int o = 0; o < _objectPool.Length; o++)
			{
				GameObject go1 = _objectPool[o];


				if (object.ReferenceEquals(go1.behaviour, DebugPusher.Instance))
				{
					_camera.currentBoundingBox.Position = go1.currentBoundingBox.Position;
					_camera.currentBoundingBox.Position.X -= (uint)Configuration.Chunk.Subpx.Width / 2 - 64 * 8;
					_camera.currentBoundingBox.Position.Y -= (uint)Configuration.Chunk.Subpx.Height / 2 - 64 * 8;
				}


				if (go1.Type != GameObject.Types.PUSHABLE) continue;

				for (int u = o + 1; u < _objectPool.Length; u++)
				{
					GameObject go2 = _objectPool[u];
					if (go2.Type != GameObject.Types.PUSHABLE) continue;

					if (go1.currentBoundingBox & go2.currentBoundingBox)
					{
						//if (

						//	!go1.PushedRight && !go1.PushedLeft &&
						//	!go2.PushedRight && !go2.PushedLeft &&
						//	!go1.PushedPreviouslyRight && !go1.PushedPreviouslyLeft &&
						//	!go2.PushedPreviouslyRight && !go2.PushedPreviouslyLeft &&
						//	!go1.PushedDown && !go1.PushedUp &&
						//	!go2.PushedDown && !go2.PushedUp &&
						//	!go1.PushedPreviouslyDown && !go1.PushedPreviouslyUp &&
						//	!go2.PushedPreviouslyDown && !go2.PushedPreviouslyUp
						//	)
						{
							//GameObject.HorizontalSeparation(go1, go2);
							//GameObject.VerticalSeparation(go1, go2);
							GameObject.Separation(go1, go2);
						}

					}
				}
			}
		}

		private void SeparatePushablesFromImmobile()
		{
			for (int o = 0; o < _objectPool.Length; o++)
			{
				GameObject go1 = _objectPool[o];

				if (go1.Type != GameObject.Types.PUSHABLE) continue;

				for (int u = 0; u < _objectPool.Length; u++)
				{
					if (o == u) continue;
					GameObject go2 = _objectPool[u];

					if(go2.Type == GameObject.Types.FLUID && go1.currentBoundingBox & go2.currentBoundingBox)
					{
						go1.VelocityAround = go2.FluidVelocity;
						go1.FrictionCoefficientAround = go2.FluidCoefficient;
					}

					if (go2.Type != GameObject.Types.IMMOBILE && go2.Type != GameObject.Types.UNSTOPPABLE) continue;

					if (go1.currentBoundingBox & go2.currentBoundingBox)
					{
						if (
							true
							//!go1.PushedRight && !go1.PushedLeft &&
							//!go1.PushedPreviouslyRight && !go1.PushedPreviouslyLeft &&
							//!go1.PushedDown && !go1.PushedUp &&
							//!go1.PushedPreviouslyDown && !go1.PushedPreviouslyUp
							)
						{
							go1.SeparationFrom(go2.currentBoundingBox);
						}

					}
				}

				TileSpan tileSpan = go1.currentBoundingBox.GetTileSpan();
				for (int row = (int)tileSpan.Top;
					row <= (int)tileSpan.Bottom;
					row++)
				{
					for (int col = (int)tileSpan.Left;
						col <= (int)tileSpan.Right;
						col++)
					{
						Tile tile = _level.GetTile(col, row);
						if (!tile.IsSolid()) continue;
						ObjectBoundingBox tileBoundingBox = ObjectBoundingBox.FromTile((uint)col, (uint)row);
						if (go1.currentBoundingBox & tileBoundingBox)
						{
							go1.SeparationFrom(tileBoundingBox);
						}
					}
				}
			}

		}

		public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            _animationFrame++;

			Render(spriteBatch);

        }
        public override void Exit()
        {
            GameObject.inputHandler = null;

            // if allocating on Enter, dispose here
            base.Exit();
        }

       
        private void Render(SpriteBatch spriteBatch)
        {

            PxPosition cameraPxPosition = _camera.currentBoundingBox.Position.ToVisualPx();

            Renderer.RenderTiles(
                spriteBatch,
                _level,
                cameraPxPosition,
                _animationFrame,
                _assets.StaticSpritesheet!,
                _assets.AnimatedSpritesheet!
                );


            for(int o = 0; o < _objectPool.Length; o++)
            {
                GameObject currentObject = _objectPool[o];
                //if (currentObject.exists && currentObject.isVisible)
                if(currentObject.Type != GameObject.Types.NONEXISTENT && currentObject.isVisible)
                {
                    PxPosition objectPosition = currentObject.currentBoundingBox.Position.ToVisualPx() - currentObject.SpriteOffset;
                    uint spritesheetIndex = currentObject.spritesheetIndex;

                    if (object.ReferenceEquals(currentObject.behaviour, DebugBox.Instance))
                    {
                        if (currentObject.PushedPreviouslyUp) spritesheetIndex |= 0x1;
                        if (currentObject.PushedPreviouslyDown) spritesheetIndex |= 0x2;
                        if (currentObject.PushedPreviouslyLeft) spritesheetIndex |= 0x4;
                        if (currentObject.PushedPreviouslyRight) spritesheetIndex |= 0x8;
                    }

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
                    if (_drawIndices)
                    {
						spriteBatch.Draw(
							texture: _assets.DebugSpritesheet,
							position: new Vector2(
								(int)objectPosition.X - cameraPxPosition.X,
								(int)objectPosition.Y - cameraPxPosition.Y
								),
							sourceRectangle: new(
                                4 * ((o>>4) & 0xf), 0, 4, 8
								),
							color: Color.White,
							0f, Vector2.Zero, 1f, spriteEffects, 0f
							);
						spriteBatch.Draw(
							texture: _assets.DebugSpritesheet,
							position: new Vector2(
								(int)objectPosition.X - cameraPxPosition.X + 4,
								(int)objectPosition.Y - cameraPxPosition.Y
								),
							sourceRectangle: new(
								4 * (o & 0xf), 0, 4, 8
								),
							color: Color.White,
							0f, Vector2.Zero, 1f, spriteEffects, 0f
							);
					}
				}
            }

		}
    }
}
