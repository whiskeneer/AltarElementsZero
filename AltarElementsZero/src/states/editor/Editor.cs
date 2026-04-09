using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.level;
using AltarElementsZero.src.states.gameplay.vectors;
using AltarElementsZero.src.states.intro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.editor
{
    internal class Editor(
        GraphicsDevice graphicsDevice,
        GameServiceContainer gameServiceContainer,
        IManager manager,
        EditorPayload payload,
        GlobalAssets globalAssets,
        InputHandler inputHandler
        ) : State<EditorAssets, EditorPayload>(
            manager: manager,
            payload: payload,
            assets: new EditorAssets(graphicsDevice, gameServiceContainer),
            inputHandler: inputHandler,
            globalAssets: globalAssets
            )
    {

        private bool _showHex = false;
        private readonly Level _level = new("assets/lvl/DEBUG_LEVEL.json");
        private uint _frame = 0;


        private PxPosition _cursorPosition = new();
        private TilePosition _cursorTilePosition = new();
        private PxPosition _cameraPosition = new();

        private byte paintingByteHigh = 0;
        private byte paintingByteLow = 0;
        private int selectedNibble = 0;


        public override void Enter()
        {
            base.Enter();

            //_level.SetAll(new Tile(Tile.Families.ConveyorRight,0));
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _frame++;
            _showHex = _inputHandler.IsDown(Input.Dash);

            if (!_inputHandler.IsDown(Input.Dash))
            {
                if (_inputHandler.IsDown(Input.Left))
                {
                    _cursorPosition.X--;
                }
                if (_inputHandler.IsDown(Input.Right))
                {
                    _cursorPosition.X++;
                }
                if (_inputHandler.IsDown(Input.Up))
                {
                    _cursorPosition.Y--;
                }
                if (_inputHandler.IsDown(Input.Down))
                {
                    _cursorPosition.Y++;
                }

                if (_inputHandler.IsDown(Input.Jump))
                {
                    _level.SetTile(
                        (int)_cursorTilePosition.X,
						(int)_cursorTilePosition.Y,
                        new Tile((Tile.Families)paintingByteHigh, paintingByteLow)
                        );
                }
                if (_inputHandler.IsDown(Input.Attack))
                {
					_level.SetTile(
						(int)_cursorTilePosition.X,
						(int)_cursorTilePosition.Y,
						new Tile((Tile.Families)0, 0)
						);
				}

            }
            else
            {
                if (_inputHandler.IsPressed(Input.Left))
                {
                    selectedNibble = (selectedNibble - 1) & 3;
                }
                if (_inputHandler.IsPressed(Input.Right))
                {
                    selectedNibble = (selectedNibble + 1) & 3;
                }
                if (_inputHandler.IsPressed(Input.Up))
                {
                    switch (selectedNibble)
                    {
                        case 0: paintingByteHigh += 0x10; break;
                        case 1: paintingByteHigh += 0x01; break;
                        case 2: paintingByteLow += 0x10; break;
                        case 3: paintingByteLow += 0x01; break;
                    }
                }
				if (_inputHandler.IsPressed(Input.Down))
				{
					switch (selectedNibble)
					{
						case 0: paintingByteHigh -= 0x10; break;
						case 1: paintingByteHigh -= 0x01; break;
						case 2: paintingByteLow -= 0x10; break;
						case 3: paintingByteLow -= 0x01; break;
					}
				}
			}

            _cameraPosition = _cursorPosition - new PxSize(
                (uint)Configuration.VisibleScreen.Px.Width >> 1,
				(uint)Configuration.VisibleScreen.Px.Height >> 1
                );
            if (_cameraPosition.X > Configuration.Level.Px.Width)
            {
                _cameraPosition.X = 0;
            }
            if (_cameraPosition.Y > Configuration.Level.Px.Height)
            {
                _cameraPosition.Y = 0;
            }

            _cursorTilePosition = _cursorPosition.ToTile();

            if (_inputHandler.IsPressed(Input.Pause))
            {
                _manager.RequestTransition(new IntroPayload("LEVEL\nMODIFIED"));
            }


		}

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            Renderer.RenderTiles(
                spriteBatch,
                _level,
                _cameraPosition,
                _frame,
                _assets.StaticSpritesheet1!,
                _assets.AnimatedSpritesheet1!
                );
            if (_showHex)
            {
                Renderer.RenderTilesHex(
                    spriteBatch,
                    _level,
                    _cameraPosition,
                    _assets.EditorSpritesheet!
                    );

				spriteBatch.Draw(
		            _assets.EditorSpritesheet,
		            (_cursorPosition - _cameraPosition).ToVector2(),
		            new Rectangle(64+16, 16, 16, 16),
		            Color.White
	            );

				int n1 = paintingByteHigh >> 4;
				int n2 = paintingByteHigh & 0xf;
				int n3 = paintingByteLow >> 4;
				int n4 = paintingByteLow & 0xf;

                if(((_frame & 0x8) == 0x8) || selectedNibble != 0)
                {
                    spriteBatch.Draw(
                        _assets.EditorSpritesheet,
                        (_cursorPosition - _cameraPosition + new PxPosition(2, 1)).ToVector2(),
                        new Rectangle(n1 * 4, 0, 4, 8),
                        Color.White);
                }
                if (((_frame & 0x8) == 0x8) || selectedNibble != 1)
                {
                    spriteBatch.Draw(
                        _assets.EditorSpritesheet,
                        (_cursorPosition - _cameraPosition + new PxPosition(6, 1)).ToVector2(),
                        new Rectangle(n2 * 4, 0, 4, 8),
                        Color.White);
                }
                if (((_frame & 0x8) == 0x8) || selectedNibble != 2)
                {
                    spriteBatch.Draw(
                        _assets.EditorSpritesheet,
                        (_cursorPosition - _cameraPosition + new PxPosition(2, 8)).ToVector2(),
                        new Rectangle(n3 * 4, 8, 4, 8),
                        Color.White);
                }
                if (((_frame & 0x8) == 0x8) || selectedNibble != 3)
                {
                    spriteBatch.Draw(
                        _assets.EditorSpritesheet,
                        (_cursorPosition - _cameraPosition + new PxPosition(6, 8)).ToVector2(),
                        new Rectangle(n4 * 4, 8, 4, 8),
                        Color.White);
                }


                TextUtilities.DrawText(
                    spriteBatch,
                    _globalAssets.RomanFont!,
                    16, 16,
                    Tile.FamilyDescriptors[paintingByteHigh],
                    0, 128 - 16);

			}
            else
            {
                if ((_frame & 0x20) == 0x20)
                {
                    spriteBatch.Draw(
                        _assets.EditorSpritesheet,
                        (_cursorTilePosition.ToPx() - _cameraPosition).ToVector2(),
                        new Rectangle(64, 0, 16, 16),
                        Color.White
                        );
                }
                spriteBatch.Draw(
                        _assets.EditorSpritesheet,
					    (_cursorPosition - _cameraPosition).ToVector2(),
					    new Rectangle(64, 16, 16, 16),
					    Color.White
				    );
            }


        }

        public override void Exit()
        {
            _level.SaveToFile("assets/lvl/DEBUG_LEVEL.json");
            // if allocating on Enter, dispose here
            base.Exit();
        }

    }
}
