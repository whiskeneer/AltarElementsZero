using AltarElementsZero.src.renderer;
using AltarElementsZero.src.screenEffects;
using AltarElementsZero.src.states.editor;
using AltarElementsZero.src.states.gameplay;
using AltarElementsZero.src.states.gameplay.vectors;
using AltarElementsZero.src.states.menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

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
        private enum State{
            ENTERING_INTRO,
            IDLE,
            ENTERING_MENU,
        }
        private State state = State.IDLE;

        private int timer = 0;
        public override void Enter()
        {
            base.Enter();
            timer = 0;
            state = State.ENTERING_INTRO;
            LoadingEffectEnd.Instance.Start();
            MediaPlayer.Stop();
            MediaPlayer.IsRepeating = false;
            MediaPlayer.Play(_globalAssets.IntroOST);
            
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
			timer++;

			if (state == State.IDLE){
                if (_inputHandler.IsPressed(Input.Jump) || _inputHandler.IsPressed(Input.Pause))
                {
                    LoadingEffectStart.Instance.Start();

					_globalAssets!.LoadingScreenSFXInstance!.Stop();
					_globalAssets!.LoadingScreenSFXInstance!.Play();
					state = State.ENTERING_MENU;
			    }else{
                    if(timer > 600)
                    {
					    _manager.RequestTransition(new GameplayPayload(GameplayPayload.GameplayConfiguration.PLAY_AUTOPLAY));
				    }
                }
            }
            else if(state == State.ENTERING_MENU)
            {
                LoadingEffectStart.Instance.Update();
                if(LoadingEffectStart.Instance.IsFinished())
                {
					_manager.RequestTransition(new MenuPayload());
				}
            }
            else if(state == State.ENTERING_INTRO)
            {
                LoadingEffectEnd.Instance.Update();
                if(LoadingEffectEnd.Instance.IsFinished())
                {
                    state = State.IDLE;
                }
            }



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

            if(state == State.ENTERING_MENU)
            {
                LoadingEffectStart.Instance.Draw(spriteBatch, _assets.Atlas!);
            }
            else if(state == State.ENTERING_INTRO)
			{
				LoadingEffectEnd.Instance.Draw(spriteBatch, _assets.Atlas!);
			}



		}
        public override void Exit()
        {
            // if allocating on Enter, dispose here
            base.Exit();
			MediaPlayer.Stop();

		}

	}
}
