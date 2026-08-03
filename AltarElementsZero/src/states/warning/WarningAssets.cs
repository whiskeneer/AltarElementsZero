using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.warning
{
	sealed class WarningAssets(
		GraphicsDevice graphicsDevice,
		GameServiceContainer gameServiceContainer
	) : LocalAssets(
		graphicsDevice: graphicsDevice,
		gameServiceContainer: gameServiceContainer)
	{
		public override void Load()
		{
			base.Load();

			// Loading textures
			// none

			// Creating renderTargets
			// none
		}

		public override void Prerender(SpriteBatch spriteBatch, GlobalAssets globalAssets, Payload payload)
		{
			base.Prerender(spriteBatch, globalAssets, payload);

			WarningPayload warningPayload = (payload as WarningPayload)!;

			// Prerendering renderTargets
			// none
		}

		public override void Unload()
		{
			// Disposing managed (textures)
			base.Unload();

			// Unreferencing textures
			// none

			// Disposing renderTargets
			// none
		}
	}
}
