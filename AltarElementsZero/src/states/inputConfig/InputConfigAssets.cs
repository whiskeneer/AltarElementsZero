using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.inputConfig
{
	sealed class InputConfigAssets(
		GraphicsDevice graphicsDevice,
		GameServiceContainer gameServiceContainer
		) : LocalAssets (
			graphicsDevice: graphicsDevice,
			gameServiceContainer: gameServiceContainer)
	{
		public Texture2D? Atlas {  get; private set; }

		public override void Load()
		{
			base.Load();

			// Loading textures
			Atlas = _contentManager!.Load<Texture2D>("img/atlas.png");

			// Creating renderTargets
			// (none)
		}
		public override void Prerender(SpriteBatch spriteBatch, GlobalAssets globalAssets, Payload payload)
		{
			base.Prerender(spriteBatch, globalAssets, payload);

			InputConfigPayload inputConfigPayload = (payload as InputConfigPayload)!;

			// Prerendering renterTargets
			// (none)
		}
		public override void Unload()
		{
			// Disposing managed (textures)
			base.Unload();

			// Unreferencing textures
			Atlas = null;

			// Disposing renterTargets
			// (none)
		}
	}
}
