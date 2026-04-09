using AltarElementsZero.src.states.gameplay.level;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.renderer
{
    static class Renderer
    {
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
		public static void RenderTiles(
            SpriteBatch spriteBatch,
            Level level,
            PxPosition cameraPosition,
            uint frame,
            Texture2D staticSpritesheet,
            Texture2D animatedSpritesheet
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


                }
            }
        }
    }
}
