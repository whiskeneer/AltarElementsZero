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
        }

        public override void Exit()
        {
            // if allocating on Enter, dispose here
            base.Exit();
        }

    }
}
