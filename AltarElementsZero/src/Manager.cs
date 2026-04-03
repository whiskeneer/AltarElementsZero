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

		private Payload? _payload = null;

		public void RequestTransition( Payload payload)
		{
			_payload = payload;
		}
	}
}
