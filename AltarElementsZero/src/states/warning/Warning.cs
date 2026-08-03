using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.vectors;
using AltarElementsZero.src.states.intro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.warning
{
	class Warning(
		GraphicsDevice graphicsDevice,
		GameServiceContainer gameServiceContainer,
		IManager manager,
		WarningPayload payload,
		GlobalAssets globalAssets,
		InputHandler inputHandler
		) : State<WarningAssets, WarningPayload>(
			manager: manager,
			payload: payload,
			assets: new WarningAssets(graphicsDevice, gameServiceContainer),
			inputHandler: inputHandler,
			globalAssets: globalAssets
			)
	{

		private enum State{
			PAGE_1,
			PAGE_2
		}
		private State state = State.PAGE_1;

		const string warningText1 =
		//	 123456789012345678901234
			" A small percentage of\n" +
			" people may experience\n" +
			"seizures when exposed to\n" +
			"   flashing lights or\n" +
			" patterns. Symptoms can\n" +
			"   include dizziness,\n" +

		//	 123456789012345678901234
			"    altered vision,\n" +
			"     twitching, or\n" +
			"    disorientation.";

		const string warningText2 =
		//	 123456789012345678901234
			"   If you or a family\n"+
			"member has a history of\n" +
			" epilepsy or seizures,\n" +
			"consult a doctor before\n" +
			"        playing\n\n" +

		//	 123456789012345678901234
			" Stop playing and seek\n" +
			"   medical attention\n" +
			"   immediately if you\n"+
			"experience any of these\n"+
			"       symptoms."
			;




		public override void Enter()
		{
			base.Enter();
			state = State.PAGE_1;
		}
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if(state == State.PAGE_1)
			{
				if(_inputHandler.IsPressed(Input.Right))
				{
					state = State.PAGE_2;
				}
			}
			else
			{
				if (_inputHandler.IsPressed(Input.Left))
				{
					state = State.PAGE_1;
				}

				else if(_inputHandler.IsPressed(Input.Pause))
				{
					_manager.RequestTransition(new IntroPayload(debugText: ""));
				}
			}
		}
		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			spriteBatch.Draw(
				_globalAssets.Atlas!,
				new Vector2(),
				new Rectangle(576, 512, 192, 128),
				Color.White
			);
			if(state == State.PAGE_1)
			{
				Renderer.RenderText(
					spriteBatch,
					new PxPosition(0, 8),
					_globalAssets.Atlas!,
					//123456789012345678901234
					 "    PHOTOSENSITIVE\n    SEIZURE WARNING",
					-1,
					Renderer.Fonts.FONT8X8);

				Renderer.RenderText(
					spriteBatch,
					new PxPosition(0, 32),
					_globalAssets.Atlas!,
					warningText1,
					-1,
					Renderer.Fonts.FONT8X8);

				Renderer.RenderText(
					spriteBatch,
					new PxPosition(192-16, 128-16),
					_globalAssets.Atlas!,
					">",
					-1,
					Renderer.Fonts.FONT8X8);
			}
			else
			{
				Renderer.RenderText(
					spriteBatch,
					new PxPosition(0, 8),
					_globalAssets.Atlas!,
					warningText2,
					-1,
					Renderer.Fonts.FONT8X8);

				Renderer.RenderText(
					spriteBatch,
					new PxPosition(0, 128 - 16),
					_globalAssets.Atlas!,
					//123456789012345678901234
					 " <         I UNDERSTAND",
					-1,
					Renderer.Fonts.FONT8X8);

			}

		}
		public override void Exit()
		{
			base.Exit();
		}
	}
}
