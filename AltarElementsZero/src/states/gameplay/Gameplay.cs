using AltarElementsZero.src.states.gameplay.gameObject;
using AltarElementsZero.src.states.gameplay.level;
using AltarElementsZero.src.states.gameplay.vectors;
using AltarElementsZero.src.renderer;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
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
                            _objectPool[nextAssignableObject].Position = new TilePosition((uint)i, (uint)j).ToPx().ToSubpx();
							nextAssignableObject++;
						}
						else if(tile.Family == Tile.Families.MovingPlatform1)
                        {
							_objectPool[nextAssignableObject].behaviour = MovingPlatform1.Instance;
							_objectPool[nextAssignableObject].Init();
							_objectPool[nextAssignableObject].Position = new TilePosition((uint)i, (uint)j).ToPx().ToSubpx();
							nextAssignableObject++;
                        }
                        else if(tile.Family == Tile.Families.DebugBox)
                        {
							_objectPool[nextAssignableObject].behaviour = DebugBox.Instance;
							_objectPool[nextAssignableObject].Init();
							_objectPool[nextAssignableObject].Position = new TilePosition((uint)i, (uint)j).ToPx().ToSubpx();
							nextAssignableObject++;
						}
						else if (tile.Family == Tile.Families.DebugPusher)
						{
							_objectPool[nextAssignableObject].behaviour = DebugPusher.Instance;
							_objectPool[nextAssignableObject].Init();
							_objectPool[nextAssignableObject].Position = new TilePosition((uint)i, (uint)j).ToPx().ToSubpx();
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
            // TODO: add SubpxPosition.ToVisualPx()
            PxPosition cameraPxPosition = _camera.Position.ToPx();
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
                if(currentObject.isVisible)
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
