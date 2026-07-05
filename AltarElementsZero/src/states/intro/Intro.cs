using AltarElementsZero.src.states.editor;
using AltarElementsZero.src.states.gameplay;
using AltarElementsZero.src.states.menu;
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

            if (_inputHandler.IsPressed(Input.Jump))
            {
				_manager.RequestTransition(new MenuPayload());
				//_manager.RequestTransition(new GameplayPayload(GameplayPayload.GameplayConfiguration.NORMAL_GAMEPLAY));
			}
   //         else if (_inputHandler.IsPressed(Input.Attack))
			//{
			//	_manager.RequestTransition(new GameplayPayload(GameplayPayload.GameplayConfiguration.PLAY_AUTOPLAY));
			//}
			//else if (_inputHandler.IsPressed(Input.Dash))
			//{
   //             //_manager.RequestTransition(new GameplayPayload(GameplayPayload.GameplayConfiguration.RECORD_AUTOPLAY));
   //             _manager.RequestTransition(new MenuPayload());
			//}
			//else if (_inputHandler.IsPressed(Input.Pause))
   //         {
   //             _manager.RequestTransition(new EditorPayload());
   //         }

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Draw(_assets.Background, Vector2.Zero, Color.White);
            //spriteBatch.Draw(_assets.DebugText, Vector2.Zero, Color.White);
        }
        public override void Exit()
        {
            // if allocating on Enter, dispose here
            base.Exit();
        }

    }
}
