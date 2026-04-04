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

        public override void Enter()
        {
            base.Enter();

            _level.SetAll(new Tile(Tile.Families.Terrain, 2));

        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Render(spriteBatch);

            TextUtilities.DrawText(
                spriteBatch,
                _globalAssets.RomanFont!,
                16,16,
                "GAMEPLAY",
                0,0
                );


        }
        public override void Exit()
        {
            // if allocating on Enter, dispose here
            base.Exit();
        }

        //

        private void Render(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < Configuration.Chunk.Tile.Height; y++)
            {
                for (int x = 0; x < Configuration.Chunk.Tile.Width; x++)
                {
                    Tile tile = _level.GetTile(x, y);
                    if (tile.Family == Tile.Families.Terrain) {
                        int spritesheetCol = tile.Member & 0xf;
                        int spritesheetRow = (tile.Member >> 4) & 0xf;

                        Vector2 outputVector = new(
                            Configuration.Tile.Px.Width * x,
                            Configuration.Tile.Px.Height * y
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
        }
    }
}
