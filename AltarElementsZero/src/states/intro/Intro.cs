using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.editor;
using AltarElementsZero.src.states.gameplay;
using AltarElementsZero.src.states.gameplay.vectors;
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

        private int timer = 0;
        public override void Enter()
        {
            base.Enter();
            timer = 0;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_inputHandler.IsPressed(Input.Jump) || _inputHandler.IsPressed(Input.Pause))
            {
				_manager.RequestTransition(new MenuPayload());
				//_manager.RequestTransition(new GameplayPayload(GameplayPayload.GameplayConfiguration.NORMAL_GAMEPLAY));
			}else{
                timer++;
                if(timer > 600)
                {
					_manager.RequestTransition(new GameplayPayload(GameplayPayload.GameplayConfiguration.PLAY_AUTOPLAY));
				}
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

            //spriteBatch.Draw(_assets.Background, Vector2.Zero, Color.White);
            //spriteBatch.Draw(_assets.DebugText, Vector2.Zero, Color.White);


            Renderer.RenderBackground(
                spriteBatch,
                new PxPosition((uint)timer, 0),
                _assets.Atlas!,
                Background1.Instance
                );
            spriteBatch.Draw(
                _assets.Atlas!,
                new Vector2(16, 16*2),
                new Rectangle(0, 384, 160, 48),
                Color.White
                );

		}
        public override void Exit()
        {
            // if allocating on Enter, dispose here
            base.Exit();
        }

    }
}
