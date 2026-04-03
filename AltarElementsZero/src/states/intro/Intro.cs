using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.intro
{
    class Intro(
        GraphicsDevice graphicsDevice,
        GameServiceContainer gameServiceContainer,
        IManager manager,
        IntroPayload payload,
        GlobalAssets globalAssets,
        InputHandler inputHandler
        ) : State<IntroAssets, IntroPayload>(
            manager: manager,
            payload: payload,
            assets: new IntroAssets(graphicsDevice, gameServiceContainer),
            inputHandler: inputHandler,
            globalAssets: globalAssets
            )
    {
        public override void Enter()
        {
            base.Enter();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Draw(_assets.Background, Vector2.Zero, Color.White);
        }
        public override void Exit()
        {
            // if allocating on Enter, dispose here
            base.Exit();
        }

    }
}
