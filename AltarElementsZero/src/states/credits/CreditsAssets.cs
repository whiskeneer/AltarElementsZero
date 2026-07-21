using AltarElementsZero.src.states.intro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.credits
{
	sealed class CreditsAssets(
		GraphicsDevice graphicsDevice,
		GameServiceContainer gameServiceContainer
	) : LocalAssets(
		graphicsDevice: graphicsDevice,
		gameServiceContainer: gameServiceContainer)
	{
		public Texture2D? Atlas { get; private set; }

		public override void Load()
		{
			base.Load();

			// Loading textures
			Atlas = _contentManager!.Load<Texture2D>("img/atlas.png");

			// Creating renderTargets
			// none
		}

		public override void Prerender(
			SpriteBatch spriteBatch,
			GlobalAssets globalAssets,
			Payload payload
			)
		{
			base.Prerender(spriteBatch, globalAssets, payload);

			CreditsPayload creditsPayload = (payload as CreditsPayload)!;

			// Prerendering renderTargets
			// none
		}

		public override void Unload()
		{
			// Disposing managed (textures)
			base.Unload();

			// Unreferencing textures
			Atlas = null;

			// Disposing renterTargets
			// none
		}
	}
}
