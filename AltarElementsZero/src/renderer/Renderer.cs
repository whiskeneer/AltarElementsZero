using AltarElementsZero.src.states.gameplay.gameObject;
using AltarElementsZero.src.states.gameplay.level;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static System.Net.Mime.MediaTypeNames;

namespace AltarElementsZero.src.renderer
{
    static class Renderer
    {

        public static void RenderBackground(
            SpriteBatch spriteBatch,
			PxPosition cameraPosition,
			Texture2D atlas,
            IBackground background
		    )
        {
            if(background.IsVertical)
            {
				RenderVerticallyScrollableBackground(
					spriteBatch,
					cameraPosition,
					atlas,
					background.AtlasPosition,
					background.Distances
				);
			}
            else
            {
                RenderHorizontallyScrollableBackground(
                    spriteBatch,
                    cameraPosition,
                    atlas,
                    background.AtlasPosition,
                    background.Distances
                );
            }
        }

		public static void RenderVerticallyScrollableBackground(
			SpriteBatch spriteBatch,
			PxPosition cameraPosition,
			Texture2D atlas,
			PxPosition atlasPosition,
			uint[] distances
			)
		{
			if (distances.Length < Configuration.Chunk.Tile.Width) return;
			for (int col = 0; col < Configuration.Chunk.Tile.Width; col++)
			{
				Rectangle sourceRectangle = new(
					(int)atlasPosition.X + col * Configuration.Tile.Px.Width,
					(int)atlasPosition.Y,
					Configuration.Tile.Px.Width,
					Configuration.Chunk.Px.Height
					);
				uint distance = distances[col];

				uint cameraOffset = 0;
				if (distance != 0)
				{
					cameraOffset = (cameraPosition.Y / distance) % (uint)Configuration.Chunk.Px.Height;
				} // otherwise, col remains immobile
				spriteBatch.Draw(
					texture: atlas,
					position: new(col * Configuration.Tile.Px.Width, -(int)cameraOffset),
					sourceRectangle: sourceRectangle,
					color: Color.White
					);
				spriteBatch.Draw(
					texture: atlas,
					position: new(col * Configuration.Tile.Px.Width, -(int)cameraOffset + Configuration.Chunk.Px.Height),
					sourceRectangle: sourceRectangle,
					color: Color.White
					);
			}
		}
		public static void RenderHorizontallyScrollableBackground(
            SpriteBatch spriteBatch,
            PxPosition cameraPosition,
            Texture2D atlas,
            PxPosition atlasPosition,
            uint[] distances
            )
        {
            if (distances.Length < Configuration.Chunk.Tile.Height) return;
            for (int row = 0; row < Configuration.Chunk.Tile.Height; row++)
            {
                Rectangle sourceRectangle = new(
                    (int)atlasPosition.X,
                    (int)atlasPosition.Y + row * Configuration.Tile.Px.Height,
                    Configuration.Chunk.Px.Width,
                    Configuration.Tile.Px.Height
                    );
                uint distance = distances[row];

                uint cameraOffset = 0;
                if(distance != 0)
                {
                    cameraOffset = (cameraPosition.X / distance) % (uint)Configuration.Chunk.Px.Width;
                } // otherwise, col remains immobile
                spriteBatch.Draw(
                    texture: atlas,
                    position: new(-(int)cameraOffset, row * Configuration.Tile.Px.Height),
                    sourceRectangle: sourceRectangle,
                    color: Color.White
                    );
				spriteBatch.Draw(
	                texture: atlas,
	                position: new(-(int)cameraOffset + Configuration.Chunk.Px.Width, row * Configuration.Tile.Px.Height),
	                sourceRectangle: sourceRectangle,
	                color: Color.White
                    );
			}
        }

		private static void RenderObject(
            SpriteBatch spriteBatch, 
            GameObject gameObject, 
            PxPosition cameraPosition, 
            Texture2D atlas
            )
        {
            if (gameObject.atlasReference.Size.X == 0 || gameObject.atlasReference.Size.Y == 0)
                return;

            PxPosition spritePosition = gameObject.currentBoundingBox.Position.ToVisualPx() - cameraPosition - gameObject.atlasReference.Offset;
            Rectangle atlasSourceRectangle = new(
                (int)gameObject.atlasReference.Start.X,
				(int)gameObject.atlasReference.Start.Y,
				(int)gameObject.atlasReference.Size.X,
				(int)gameObject.atlasReference.Size.Y
            );
				
            for (int y = 0; y < gameObject.atlasReference.RepeatY + 1; y++)
            {
                for (int x = 0; x < gameObject.atlasReference.RepeatX + 1; x++)
				{
                    spriteBatch.Draw(
                        texture: atlas,
                        position: new(
                            (int)spritePosition.X + x * gameObject.atlasReference.Size.X, 
                            (int)spritePosition.Y + y * gameObject.atlasReference.Size.Y),
                        sourceRectangle: atlasSourceRectangle,
                        color: Color.White,
                        0f, Vector2.Zero, 1f, gameObject.atlasReference.Effects, 0f 
                    );
				}
			}
        }
        public static void RenderObjects(
            SpriteBatch spriteBatch,
            GameObject[] objectPool,
            PxPosition cameraPosition,
            Texture2D atlas
            )
        {
            for (int o = 0; o < objectPool.Length; o++)
            {
                GameObject currentObject = objectPool[o];
                if(currentObject.Type != GameObject.Types.NONEXISTENT &&
                    currentObject.drawOrder == GameObject.DrawOrderTypes.BACK)
                {
                    RenderObject(spriteBatch, currentObject, cameraPosition, atlas);
                }
            }

			for (int o = 0; o < objectPool.Length; o++)
			{
				GameObject currentObject = objectPool[o];
				if (currentObject.Type != GameObject.Types.NONEXISTENT &&
					currentObject.drawOrder == GameObject.DrawOrderTypes.MIDDLE)
				{
					RenderObject(spriteBatch, currentObject, cameraPosition, atlas);
				}
			}

			for (int o = 0; o < objectPool.Length; o++)
			{
				GameObject currentObject = objectPool[o];
				if (currentObject.Type != GameObject.Types.NONEXISTENT &&
					currentObject.drawOrder == GameObject.DrawOrderTypes.FRONT)
				{
					RenderObject(spriteBatch, currentObject, cameraPosition, atlas);
				}
			}
		}

        public static void RenderSpawnPoints(
            SpriteBatch spriteBatch,
            Level level,
            PxPosition cameraPosition,
            Texture2D editorSpritesheet
            )
        {
            PxPosition cameraTileRemainder = cameraPosition.TileRemainder();
            TilePosition cameraTilePosition = cameraPosition.ToTile();
            for (int tileOffsetY = 0; tileOffsetY <= Configuration.Chunk.Tile.Height; tileOffsetY++)
            {
                for (int tileOffsetX = 0; tileOffsetX <= Configuration.Chunk.Tile.Width; tileOffsetX++)
                {
                    Tile tile = level.GetTile(
                        (int)cameraTilePosition.X + tileOffsetX,
                        (int)cameraTilePosition.Y + tileOffsetY
                        );

                    if (tile.IsObjectSpawn())
                    {
                        int spritesheetCol = (int)tile.Family & 0xf;
                        int spritesheetRow = ((int)tile.Family >> 4) & 0xf;
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
                            editorSpritesheet,
                            outputVector,
                            sourceRectangle,
                            Color.White
                            );

                    }

                }
            }
        }
        public static void RenderTilesHex(
			SpriteBatch spriteBatch,
			Level level,
			PxPosition cameraPosition,
            Texture2D editorSpritesheet
			)
        {
			PxPosition cameraTileRemainder = cameraPosition.TileRemainder();
			TilePosition cameraTilePosition = cameraPosition.ToTile();


            for (int tileOffsetY = 0; tileOffsetY <= Configuration.Chunk.Tile.Height; tileOffsetY++)
            {
                for (int tileOffsetX = 0; tileOffsetX <= Configuration.Chunk.Tile.Width; tileOffsetX++)
                {
                    Tile tile = level.GetTile(
                        (int)cameraTilePosition.X + tileOffsetX,
                        (int)cameraTilePosition.Y + tileOffsetY
                        );

                    int n1 = (int)tile.Family >> 4;
                    int n2 = (int)tile.Family & 0xf;
                    int n3 = tile.Member >> 4;
                    int n4 = tile.Member & 0xf;

                    PxPosition tilePosition = new(
                        (uint)(Configuration.Tile.Px.Width * tileOffsetX - cameraTileRemainder.X),
                        (uint)(Configuration.Tile.Px.Height * tileOffsetY - cameraTileRemainder.Y)
                        );

                    spriteBatch.Draw(
                        editorSpritesheet,
                        new Vector2((int)tilePosition.X, (int)tilePosition.Y),
                        new Rectangle(n1 * 4, 0, 4, 8),
                        Color.White);
					spriteBatch.Draw(
						editorSpritesheet,
						new Vector2((int)tilePosition.X + 4, (int)tilePosition.Y),
						new Rectangle(n2 * 4, 0, 4, 8),
						Color.White);
					spriteBatch.Draw(
						editorSpritesheet,
						new Vector2((int)tilePosition.X, (int)tilePosition.Y + 8),
						new Rectangle(n3 * 4, 8, 4, 8),
						Color.White);
					spriteBatch.Draw(
						editorSpritesheet,
						new Vector2((int)tilePosition.X + 4, (int)tilePosition.Y + 8),
						new Rectangle(n4 * 4, 8, 4, 8),
						Color.White);


				}
            }
        }

        public static void RenderDarkness(
		    SpriteBatch spriteBatch,
			SubpxPosition playerPositionCenter,
            bool illuminated,
            PxPosition cameraPosition,
            Texture2D atlas,
            uint from, uint to
            )
        {
            PxPosition cameraTileRemainder = cameraPosition.TileRemainder();
            TilePosition cameraTilePosition = cameraPosition.ToTile();

            PxPosition lightSourceCenter = playerPositionCenter.ToVisualPx();

            for (uint tileOffsetY = 0; tileOffsetY <= Configuration.Chunk.Tile.Height; tileOffsetY++)
            {
				uint tileY = cameraTilePosition.Y + tileOffsetY;
				for (uint tileOffsetX = 0; tileOffsetX <= Configuration.Chunk.Tile.Width; tileOffsetX++)
                {
                    uint tileX = cameraTilePosition.X + tileOffsetX;
                    if (tileX < from || tileX > to) continue;

					TilePosition tilePosition = new(tileX,tileY);
                    PxPosition tileCenter = tilePosition.ToPx() + new PxPosition(8, 8);

                    Rectangle atlasSourceRectangle;

					if (illuminated)
                    {
                        uint distanceX = tileCenter.X - lightSourceCenter.X;
                        uint distanceY = tileCenter.Y - lightSourceCenter.Y;
                        uint squaredDistance = distanceX * distanceX + distanceY * distanceY;

                        if (squaredDistance < (uint)((16 * 2) * (16 * 2) * 2))
                        {
                            atlasSourceRectangle = new(416, 976, 16, 16);
                        }
                        else if (squaredDistance < (uint)((16 * 2.5) * (16 * 2.5) * 2))
                        {
                            atlasSourceRectangle = new(400, 976, 16, 16);
                        }
                        else if (squaredDistance < (uint)((16 * 3) * (16 * 3) * 2))
                        {
                            atlasSourceRectangle = new(384, 976, 16, 16);
                        }
						else if (squaredDistance < (uint)((16 * 3.5) * (16 * 3.5) * 2))
						{
							atlasSourceRectangle = new(368, 976, 16, 16);
						}
						else
						{
							atlasSourceRectangle = new(352, 976, 16, 16);
						}
					}
                    else 
                    {
                        atlasSourceRectangle = new(352, 976, 16, 16);
                    }

					Vector2 outputVector = new(
	                    Configuration.Tile.Px.Width * tileOffsetX - cameraTileRemainder.X,
	                    Configuration.Tile.Px.Height * tileOffsetY - cameraTileRemainder.Y
	                    );

					spriteBatch.Draw(
						atlas,
						outputVector,
						atlasSourceRectangle,
						Color.White);
				}
            }
        }

		public static void RenderTiles(
            SpriteBatch spriteBatch,
            Level level,
            PxPosition cameraPosition,
            uint frame,
            Texture2D staticSpritesheet,
            Texture2D animatedSpritesheet,
            Texture2D blobSpritesheet
            )
        {
            PxPosition cameraTileRemainder = cameraPosition.TileRemainder();
            TilePosition cameraTilePosition = cameraPosition.ToTile();

            for (int tileOffsetY = 0; tileOffsetY <= Configuration.Chunk.Tile.Height; tileOffsetY++)
            {
                for (int tileOffsetX = 0; tileOffsetX <= Configuration.Chunk.Tile.Width; tileOffsetX++)
                {
                    Tile tile = level.GetTile(
                        (int)cameraTilePosition.X + tileOffsetX,
                        (int)cameraTilePosition.Y + tileOffsetY
                        );

                    if (tile.IsStaticTile())
                    {
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
                            staticSpritesheet,
                            outputVector,
                            sourceRectangle,
                            Color.White
                            );

                    }
                    else if (tile.IsAnimatedTile())
                    {
                        int spritesheetCol = (tile.Member & 0xc) | (((int)frame >> (3 - (tile.Member & 0x3))) & 0x3);
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
                            animatedSpritesheet,
                            outputVector,
                            sourceRectangle,
                            Color.White);
                    }
                    else if (tile.IsBlobTile())
                    {
                        int spritesheetCol = (tile.BlobFamilyIndex() & 0x7) * 8 + (tile.Member & 0x7);
                        int spritesheetRow = (tile.BlobFamilyIndex() >> 3) * 8 + (tile.Member >> 3);
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
                            blobSpritesheet,
                            outputVector,
                            sourceRectangle,
                            Color.White);
					}


                }
            }
        }

        public enum Fonts
        {
            FONT8X8,
            FONT16X16
        }

		public static void RenderText(
	        SpriteBatch spriteBatch,
	        PxPosition position,
	        Texture2D atlas,
	        string text,
            int maxLength,
            Fonts font
        )
        {
            int fontOriginX, fontOriginY;
            int fontWidth, fontHeight;
            switch(font)
            {
                case Fonts.FONT8X8:
                    fontOriginX = 896;
                    fontOriginY = 640;
                    fontWidth = 8;
                    fontHeight = 8;
                    break;
                case Fonts.FONT16X16:
                default:
                    fontOriginX = 768;
                    fontOriginY = 768;
                    fontWidth = 16;
                    fontHeight = 16;
                    break;
            }
            int row = 0;
            int col = 0;

			for (int i = 0; i < text.Length && (maxLength == -1 || i < maxLength); i++)
			{
				char c = text[i];
				byte b = (byte)c;

				Vector2 outputVector = new(
					position.X + col * fontWidth,
					position.Y + row * fontHeight
				);
				Rectangle sourceRectangle = new(
					fontOriginX + fontWidth * (b & 0xf),
					fontOriginY + fontHeight * ((b >> 4) & 0xf),
					fontWidth,
					fontHeight
				);

				spriteBatch.Draw(
					atlas,
					outputVector,
					sourceRectangle,
					Color.White
				);

                if (c == '\n')
                {
                    row++;
                    col = 0;
                }
                else 
                {
                    col++;
                }
			}
		}

		public static void RenderText(
            SpriteBatch spriteBatch,
            PxPosition position,
            Texture2D atlas,
            string text,
            Fonts font
        )
        {
            RenderText(
                spriteBatch,
                position,
                atlas,
                text,
                -1,
                font
            );
        }
		public static void RenderText(
	        SpriteBatch spriteBatch,
	        PxPosition position,
	        Texture2D atlas,
	        string text
            )
        {
			RenderText(
		        spriteBatch,
		        position,
		        atlas,
		        text,
		        -1,
		        Fonts.FONT16X16
	        );
		}
		public static void RenderText(
			SpriteBatch spriteBatch,
			PxPosition position,
			Texture2D atlas,
			string text,
			int maxLength
			)
		{
			RenderText(
				spriteBatch,
				position,
				atlas,
				text,
				maxLength,
				Fonts.FONT16X16
			);
		}

	}
}
