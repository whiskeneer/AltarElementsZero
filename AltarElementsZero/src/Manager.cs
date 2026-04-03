using AltarElementsZero.src.states.intro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src
{
	interface IManager
	{
		public void RequestTransition(Payload payload);
	}

	class Manager(
		GraphicsDevice graphicsDevice,
		GameServiceContainer gameServiceContainer,
		GlobalAssets globalAssets,
		InputHandler inputHandler
		) : IManager
	{
		private readonly GraphicsDevice _graphicsDevice = graphicsDevice;
		private readonly GameServiceContainer _gameServiceContainer = gameServiceContainer;
		private readonly GlobalAssets _globalAssets = globalAssets;
		private readonly InputHandler _inputHandler = inputHandler;

		private IState? _state = null;
		private Payload? _payload = null;

		public void RequestTransition( Payload payload)
		{
			_payload = payload;
		}

		private IState? Factory(Payload payload)
		{
			if (payload is null) return null;
			if (payload is IntroPayload introPayload) return new Intro(
				_graphicsDevice,
				_gameServiceContainer,
				this,
				introPayload,
				_globalAssets,
				_inputHandler
				);
			else return null;
		}

		public void Update(GameTime gameTime)
		{
			if(_payload != null)
			{
				_state?.Exit();
				_state = null;

				// call GC here, if necessary

				_state = Factory(_payload);
				_payload = null;

				_state?.Enter();
			}
			else
			{
				_state?.Update(gameTime);
			}
		}

		public void Prerender(SpriteBatch spriteBatch)
		{
			_state?.Prerender(spriteBatch);
		}
		public void Draw(SpriteBatch spriteBatch)
		{
			_state?.Draw(spriteBatch);
		}
		public void End()
		{
			_state?.Exit();
			_state = null;
			_payload = null;
		}

	}
}
