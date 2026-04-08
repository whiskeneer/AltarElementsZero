using AltarElementsZero.src.states.gameplay.level;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.renderer
{
    static class Renderer
    {
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
