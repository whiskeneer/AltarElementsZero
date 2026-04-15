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

        private bool editingChunks = false;

        private ChunkPosition _currentChunk = new();
        private int selectedChunkNibble = 0;


        private bool _showHex = false;
        private readonly Level _level = new("assets/lvl/DEBUG_LEVEL.json", "assets/lvl/DEBUG_LEVEL_CHUNKS.json");
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

            for (int j = 0; j < Configuration.Level.Tile.Height; j++)
            {
                for(int i = 0; i < Configuration.Level.Tile.Width; i++)
                {
                    if(_level.GetTile(i,j).Family == Tile.Families.Ora)
                    {
                        _cursorPosition = new TilePosition((uint)i, (uint)j).ToPx();
                    }
                }
            }

            //_level.SetAll(new Tile(Tile.Families.ConveyorRight,0));
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _frame++;
            _showHex = _inputHandler.IsDown(Input.Dash);

            if (!editingChunks)
            {
                if (!_showHex)
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
                else // _showHex == true
                {
                    if (_inputHandler.IsPressed(Input.Jump))
                    {
                        editingChunks = true;
                        _currentChunk = new(
                            (uint)(_cursorTilePosition.X / Configuration.Chunk.Tile.Width),
							(uint)(_cursorTilePosition.Y / Configuration.Chunk.Tile.Height)
							);
                        //_cameraPosition = _currentChunk.ToPx();
                    }

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

            }
            else // editingChunks == true
            {
                if (!_showHex)
                {
                    if (_inputHandler.IsPressed(Input.Up))
                    {
                        _currentChunk.Y = (uint)((_currentChunk.Y - 1) & (Configuration.Level.Chunk.Height - 1));
                    }
                    if (_inputHandler.IsPressed(Input.Down))
                    {
						_currentChunk.Y = (uint)((_currentChunk.Y + 1) & (Configuration.Level.Chunk.Height - 1));
					}
					if (_inputHandler.IsPressed(Input.Left))
					{
						_currentChunk.X = (uint)((_currentChunk.X - 1) & (Configuration.Level.Chunk.Width - 1));
					}
					if (_inputHandler.IsPressed(Input.Right))
					{
						_currentChunk.X = (uint)((_currentChunk.X + 1) & (Configuration.Level.Chunk.Width - 1));
					}

				}
                else // _showHex == true
                {
                    

                    if (_inputHandler.IsPressed(Input.Jump))
                    {
                        editingChunks = false;
                        _cursorPosition = _cameraPosition + new PxSize(
					        (uint)Configuration.VisibleScreen.Px.Width >> 1,
					        (uint)Configuration.VisibleScreen.Px.Height >> 1
					        );
					}

                    if (_inputHandler.IsPressed(Input.Left))
                    {
                        selectedChunkNibble = (selectedChunkNibble - 1) & 0xf;
                    }
                    if (_inputHandler.IsPressed(Input.Right))
                    {
						selectedChunkNibble = (selectedChunkNibble + 1) & 0xf;
					}

                    Chunk chunkData = _level.GetChunk((int)_currentChunk.X, (int)_currentChunk.Y);

                    if (_inputHandler.IsPressed(Input.Up))
                    {
                        switch (selectedChunkNibble)
                        {
                            case 0: chunkData.Top += 0x10; break;
                            case 1: chunkData.Top += 0x01; break;
							case 2: chunkData.Bottom += 0x10; break;
							case 3: chunkData.Bottom += 0x01; break;
							case 4: chunkData.Left += 0x10; break;
							case 5: chunkData.Left += 0x01; break;
							case 6: chunkData.Right += 0x10; break;
							case 7: chunkData.Right += 0x01; break;


							case 8: chunkData.BackgroundIndex += 0x10; break;
							case 9: chunkData.BackgroundIndex += 0x01; break;
							case 10: chunkData.Reserved1 += 0x10; break;
							case 11: chunkData.Reserved1 += 0x01; break;
							case 12: chunkData.Reserved2 += 0x10; break;
							case 13: chunkData.Reserved2 += 0x01; break;
							case 14: chunkData.Reserved3 += 0x10; break;
							case 15: chunkData.Reserved3 += 0x01; break;
						}

                        _level.SetChunk((int)_currentChunk.X, (int)_currentChunk.Y, chunkData);
                    }

					if (_inputHandler.IsPressed(Input.Down))
					{
						switch (selectedChunkNibble)
						{
							case 0: chunkData.Top -= 0x10; break;
							case 1: chunkData.Top -= 0x01; break;
							case 2: chunkData.Bottom -= 0x10; break;
							case 3: chunkData.Bottom -= 0x01; break;
							case 4: chunkData.Left -= 0x10; break;
							case 5: chunkData.Left -= 0x01; break;
							case 6: chunkData.Right -= 0x10; break;
							case 7: chunkData.Right -= 0x01; break;


							case 8: chunkData.BackgroundIndex -= 0x10; break;
							case 9: chunkData.BackgroundIndex -= 0x01; break;
							case 10: chunkData.Reserved1 -= 0x10; break;
							case 11: chunkData.Reserved1 -= 0x01; break;
							case 12: chunkData.Reserved2 -= 0x10; break;
							case 13: chunkData.Reserved2 -= 0x01; break;
							case 14: chunkData.Reserved3 -= 0x10; break;
							case 15: chunkData.Reserved3 -= 0x01; break;
						}

						_level.SetChunk((int)_currentChunk.X, (int)_currentChunk.Y, chunkData);
					}



				}

				_cameraPosition = _currentChunk.ToPx();
			}




            if (_inputHandler.IsPressed(Input.Pause))
            {
                _manager.RequestTransition(new IntroPayload("ALTAR\nELEMENTS\nZERO\n(ALPHA)"));
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
            Renderer.RenderSpawnPoints(
                spriteBatch,
                _level,
                _cameraPosition,
                _assets.EditorSpritesheet!
                );

            if (editingChunks)
            {
                int x1 = (int)_currentChunk.X >> 4;
                int x2 = (int)_currentChunk.X & 0xf;
				int y1 = (int)_currentChunk.Y >> 4;
				int y2 = (int)_currentChunk.Y & 0xf;

				spriteBatch.Draw(
					_assets.EditorSpritesheet,
					new Vector2(0,16),
					new Rectangle(x1 * 4, 0, 4, 8),
					Color.White);
				spriteBatch.Draw(
					_assets.EditorSpritesheet,
					new Vector2(4, 16),
					new Rectangle(x2 * 4, 0, 4, 8),
					Color.White);
				spriteBatch.Draw(
					_assets.EditorSpritesheet,
					new Vector2(0, 0),
					new Rectangle(y1 * 4, 0, 4, 8),
					Color.White);
				spriteBatch.Draw(
					_assets.EditorSpritesheet,
					new Vector2(4, 0),
					new Rectangle(y2 * 4, 0, 4, 8),
					Color.White);

                if (_showHex)
                {
                    Chunk chunkData = _level.GetChunk((int)_currentChunk.X, (int)_currentChunk.Y);
					if (((_frame & 0x8) == 0x8) || selectedChunkNibble != 0)
					{
						spriteBatch.Draw(
							_assets.EditorSpritesheet,
							new Vector2(32,0),
							new Rectangle((chunkData.Top >> 4) * 4, 0, 4, 8),
							Color.White);
					}
					if (((_frame & 0x8) == 0x8) || selectedChunkNibble != 1)
					{
						spriteBatch.Draw(
							_assets.EditorSpritesheet,
							new Vector2(36, 0),
							new Rectangle((chunkData.Top & 0xf) * 4, 0, 4, 8),
							Color.White);
					}
					if (((_frame & 0x8) == 0x8) || selectedChunkNibble != 2)
					{
						spriteBatch.Draw(
							_assets.EditorSpritesheet,
							new Vector2(32, 8),
							new Rectangle((chunkData.Bottom >> 4) * 4, 0, 4, 8),
							Color.White);
					}
					if (((_frame & 0x8) == 0x8) || selectedChunkNibble != 3)
					{
						spriteBatch.Draw(
							_assets.EditorSpritesheet,
							new Vector2(36, 8),
							new Rectangle((chunkData.Bottom & 0xf) * 4, 0, 4, 8),
							Color.White);
					}
					if (((_frame & 0x8) == 0x8) || selectedChunkNibble != 4)
					{
						spriteBatch.Draw(
							_assets.EditorSpritesheet,
							new Vector2(32, 16),
							new Rectangle((chunkData.Left >> 4) * 4, 0, 4, 8),
							Color.White);
					}
					if (((_frame & 0x8) == 0x8) || selectedChunkNibble != 5)
					{
						spriteBatch.Draw(
							_assets.EditorSpritesheet,
							new Vector2(36, 16),
							new Rectangle((chunkData.Left & 0xf) * 4, 0, 4, 8),
							Color.White);
					}
					if (((_frame & 0x8) == 0x8) || selectedChunkNibble != 6)
					{
						spriteBatch.Draw(
							_assets.EditorSpritesheet,
							new Vector2(32, 24),
							new Rectangle((chunkData.Right >> 4) * 4, 0, 4, 8),
							Color.White);
					}
					if (((_frame & 0x8) == 0x8) || selectedChunkNibble != 7)
					{
						spriteBatch.Draw(
							_assets.EditorSpritesheet,
							new Vector2(36, 24),
							new Rectangle((chunkData.Right & 0xf) * 4, 0, 4, 8),
							Color.White);
					}
					if (((_frame & 0x8) == 0x8) || selectedChunkNibble != 8)
					{
						spriteBatch.Draw(
							_assets.EditorSpritesheet,
							new Vector2(48, 0),
							new Rectangle((chunkData.BackgroundIndex >> 4) * 4, 0, 4, 8),
							Color.White);
					}
					if (((_frame & 0x8) == 0x8) || selectedChunkNibble != 9)
					{
						spriteBatch.Draw(
							_assets.EditorSpritesheet,
							new Vector2(52, 0),
							new Rectangle((chunkData.BackgroundIndex & 0xf) * 4, 0, 4, 8),
							Color.White);
					}
					if (((_frame & 0x8) == 0x8) || selectedChunkNibble != 10)
					{
						spriteBatch.Draw(
							_assets.EditorSpritesheet,
							new Vector2(48, 8),
							new Rectangle((chunkData.Reserved1 >> 4) * 4, 0, 4, 8),
							Color.White);
					}
					if (((_frame & 0x8) == 0x8) || selectedChunkNibble != 11)
					{
						spriteBatch.Draw(
							_assets.EditorSpritesheet,
							new Vector2(52, 8),
							new Rectangle((chunkData.Reserved1 & 0xf) * 4, 0, 4, 8),
							Color.White);
					}
					if (((_frame & 0x8) == 0x8) || selectedChunkNibble != 12)
					{
						spriteBatch.Draw(
							_assets.EditorSpritesheet,
							new Vector2(48, 16),
							new Rectangle((chunkData.Reserved2 >> 4) * 4, 0, 4, 8),
							Color.White);
					}
					if (((_frame & 0x8) == 0x8) || selectedChunkNibble != 13)
					{
						spriteBatch.Draw(
							_assets.EditorSpritesheet,
							new Vector2(52, 16),
							new Rectangle((chunkData.Reserved2 & 0xf) * 4, 0, 4, 8),
							Color.White);
					}
					if (((_frame & 0x8) == 0x8) || selectedChunkNibble != 14)
					{
						spriteBatch.Draw(
							_assets.EditorSpritesheet,
							new Vector2(48, 24),
							new Rectangle((chunkData.Reserved3 >> 4) * 4, 0, 4, 8),
							Color.White);
					}
					if (((_frame & 0x8) == 0x8) || selectedChunkNibble != 15)
					{
						spriteBatch.Draw(
							_assets.EditorSpritesheet,
							new Vector2(52, 24),
							new Rectangle((chunkData.Reserved3 & 0xf) * 4, 0, 4, 8),
							Color.White);
					}
				}
			}
            else // editingChunks == false
            {
                // NOTE: avoid using division and modulo on gameplay
                spriteBatch.Draw(
                    _assets.ChunkOutline,
                    new Vector2(
                        -(_cameraPosition.X % Configuration.Chunk.Px.Width),
                        -(_cameraPosition.Y % Configuration.Chunk.Px.Height)),
                    Color.White
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
                else // _showHex == false
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



        }

        public override void Exit()
        {
            _level.SaveToFile("assets/lvl/DEBUG_LEVEL.json", "assets/lvl/DEBUG_LEVEL_CHUNKS.json");
            // if allocating on Enter, dispose here
            base.Exit();
        }

    }
}
