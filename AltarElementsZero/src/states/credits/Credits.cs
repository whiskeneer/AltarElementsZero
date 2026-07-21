using System.Timers;
using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.credits
{
	class Credits(
		GraphicsDevice graphicsDevice,
		GameServiceContainer gameServiceContainer,
		IManager manager,
		CreditsPayload payload,
		GlobalAssets globalAssets,
		InputHandler inputHandler
	) : State<CreditsAssets, CreditsPayload>(
		manager: manager,
		payload: payload,
		assets: new CreditsAssets(graphicsDevice, gameServiceContainer),
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

			Renderer.RenderBackground(
				spriteBatch,
				new PxPosition(0, 0),
				_assets.Atlas!,
				Background1.Instance
				);
			Renderer.RenderText(spriteBatch, new PxPosition(),_assets.Atlas!, "\n   MUCHAS\n   GRACIAS\n  POR JUGAR\n\nDEMO 1.0 VER\nEL DESTELLO", -1, Renderer.Fonts.FONT16X16);
		}
		public override void Exit()
		{
			// if allocating on Enter, dispose here
			base.Exit();
		}
	}
}
