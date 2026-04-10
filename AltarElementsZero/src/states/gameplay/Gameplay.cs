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
                            _objectPool[nextAssignableObject].boundingBox.Position = new TilePosition((uint)i, (uint)j).ToPx().ToSubpx();
							nextAssignableObject++;
						}
						else if(tile.Family == Tile.Families.MovingPlatform1)
                        {
							_objectPool[nextAssignableObject].behaviour = MovingPlatform1.Instance;
							_objectPool[nextAssignableObject].Init();
							_objectPool[nextAssignableObject].boundingBox.Position = new TilePosition((uint)i, (uint)j).ToPx().ToSubpx();
							nextAssignableObject++;
                        }
                        else if(tile.Family == Tile.Families.DebugBox)
                        {
							_objectPool[nextAssignableObject].behaviour = DebugBox.Instance;
							_objectPool[nextAssignableObject].Init();
							_objectPool[nextAssignableObject].boundingBox.Position = new TilePosition((uint)i, (uint)j).ToPx().ToSubpx();
							nextAssignableObject++;
						}
						else if (tile.Family == Tile.Families.DebugPusher)
						{
							_objectPool[nextAssignableObject].behaviour = DebugPusher.Instance;
							_objectPool[nextAssignableObject].Init();
							_objectPool[nextAssignableObject].boundingBox.Position = new TilePosition((uint)i, (uint)j).ToPx().ToSubpx();
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

            //Console.WriteLine("CLEANED PUSHING");
			for (int o = 0; o < _objectPool.Length; o++)
            {
				GameObject gameObject = _objectPool[o];
                if(gameObject.Type == GameObject.Types.PUSHABLE)
                {
                    gameObject.PushingUp = false;
                    gameObject.PushingDown = false;
                    gameObject.PushingLeft = false;
                    gameObject.PushingRight = false;
                }
			}

			for (int o = 0; o < _objectPool.Length; o++)
            {
                GameObject gameObject = _objectPool[o];
                switch (gameObject.Type)
                {
                    case GameObject.Types.IMMOBILE:
                        break;
                    case GameObject.Types.PUSHABLE:
                        gameObject.Update(); // behaviour modifies gameObject.Velocity

                        gameObject.boundingBox.Position.X += (uint)gameObject.Velocity.X;
                        for(int u = 0; u < _objectPool.Length; u++)
                        {
                            if (u == o) continue;

                            GameObject otherGameObject = _objectPool[u];

                            if (otherGameObject.Type == GameObject.Types.NONEXISTENT) continue;

                            if(gameObject.boundingBox & otherGameObject.boundingBox)
                            {
                                HorizontalLean(gameObject, otherGameObject);
                                //if(otherGameObject.Type == GameObject.Types.UNSTOPPABLE)
                                //{
                                //}
                                //else
                                //{
                                //    HorizontalPush(gameObject, otherGameObject);
                                //}
							}
                        }

						gameObject.boundingBox.Position.Y += (uint)gameObject.Velocity.Y;
						for (int u = 0; u < _objectPool.Length; u++)
						{
							if (u == o) continue;

							GameObject otherGameObject = _objectPool[u];

							if (otherGameObject.Type == GameObject.Types.NONEXISTENT) continue;

							if (gameObject.boundingBox & otherGameObject.boundingBox)
							{
                                VerticalLean(gameObject, otherGameObject);
                                //if(otherGameObject.Type == GameObject.Types.UNSTOPPABLE)
                                //{
                                //}
                                //else
                                //{
                                //    VerticalPush(gameObject, otherGameObject);
                                //}
							}
						}

						break;

                    case GameObject.Types.UNSTOPPABLE:
                        gameObject.Update(); // behaviour modifies gameObject.Velocity

						gameObject.boundingBox.Position.X += (uint)gameObject.Velocity.X;
						for (int u = 0; u < _objectPool.Length; u++)
						{
							if (u == o) continue;

							GameObject otherGameObject = _objectPool[u];

							if (otherGameObject.Type != GameObject.Types.PUSHABLE) continue;

							if (gameObject.boundingBox & otherGameObject.boundingBox)
							{
								HorizontalPush(gameObject, otherGameObject);
							}
						}

						gameObject.boundingBox.Position.Y += (uint)gameObject.Velocity.Y;
						for (int u = 0; u < _objectPool.Length; u++)
						{
							if (u == o) continue;

							GameObject otherGameObject = _objectPool[u];

							if (otherGameObject.Type != GameObject.Types.PUSHABLE) continue;

							if (gameObject.boundingBox & otherGameObject.boundingBox)
							{
								VerticalPush(gameObject, otherGameObject);
							}
						}
						break;

                    case GameObject.Types.NONEXISTENT:
                        break;
                    default:
                        break;
                }
            }
		}

        private static void HorizontalPush(GameObject gameObject, GameObject otherGameObject)
        {
			if (otherGameObject.Velocity.X > gameObject.Velocity.X)
			{
				otherGameObject.boundingBox.LeanAtLeft(gameObject.boundingBox);
                otherGameObject.PushingLeft = true;
			}
			else
			{
				otherGameObject.boundingBox.LeanAtRight(gameObject.boundingBox);
                otherGameObject.PushingRight = true;
			}
            otherGameObject.Velocity.X = gameObject.Velocity.X;

		}
        private static void VerticalPush(GameObject gameObject, GameObject otherGameObject)
        {
			if (otherGameObject.Velocity.Y > gameObject.Velocity.Y)
			{
				otherGameObject.boundingBox.LeanAbove(gameObject.boundingBox);
                otherGameObject.PushingUp = true;
			}
			else
			{
				otherGameObject.boundingBox.LeanBelow(gameObject.boundingBox);
                otherGameObject.PushingDown = true;
			}
			otherGameObject.Velocity.Y = gameObject.Velocity.Y;
		}

        private static void HorizontalLean(GameObject gameObject, GameObject otherGameObject)
        {
			if (gameObject.Velocity.X > otherGameObject.Velocity.X)
			{
				gameObject.boundingBox.LeanAtLeft(otherGameObject.boundingBox);
				gameObject.PushingLeft = true;
			}
			else
			{
				gameObject.boundingBox.LeanAtRight(otherGameObject.boundingBox);
                gameObject.PushingRight= true;
			}
            gameObject.Velocity.X = otherGameObject.Velocity.X;
		}
        private static void VerticalLean(GameObject gameObject, GameObject otherGameObject)
        {
			if (gameObject.Velocity.Y > otherGameObject.Velocity.Y)
			{
				gameObject.boundingBox.LeanAbove(otherGameObject.boundingBox);
				gameObject.PushingUp = true;
			}
			else
			{
				gameObject.boundingBox.LeanBelow(otherGameObject.boundingBox);
                gameObject.PushingDown = true;
			}
            gameObject.Velocity.Y = otherGameObject.Velocity.Y;
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
            //Console.WriteLine("RENDER");
            // TODO: add SubpxPosition.ToVisualPx()
            PxPosition cameraPxPosition = _camera.boundingBox.Position.ToPx();
            //PxPosition cameraTileRemainder = cameraPxPosition.TileRemainder();
            //TilePosition cameraTilePosition = cameraPxPosition.ToTile();

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
                    PxPosition objectPosition = currentObject.boundingBox.Position.ToPx() - currentObject.SpriteOffset;
                    uint spritesheetIndex = currentObject.spritesheetIndex;

                    if(object.ReferenceEquals(currentObject.behaviour, DebugBox.Instance))
                    {
						if (currentObject.PushingUp) spritesheetIndex |= 0x1;
						if (currentObject.PushingDown) spritesheetIndex |= 0x2;
						if (currentObject.PushingLeft) spritesheetIndex |= 0x4;
						if (currentObject.PushingRight) spritesheetIndex |= 0x8;
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
