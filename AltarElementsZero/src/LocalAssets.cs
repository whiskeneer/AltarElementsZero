using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src
{
    abstract class LocalAssets(
        GraphicsDevice graphicsDevice,
        GameServiceContainer gameServiceContainer
        ) : Assets(
            graphicsDevice: graphicsDevice,
            gameServiceContainer: gameServiceContainer)
    {
        protected void PrerenderBegin(
            SpriteBatch spriteBatch,
            RenderTarget2D renderTarget2D
            )
        {
            _graphicsDevice.SetRenderTarget( renderTarget2D );
            _graphicsDevice.Clear( Color.Transparent );
            spriteBatch.Begin(
				SpriteSortMode.Deferred,
				BlendState.AlphaBlend,
				SamplerState.PointClamp,
				DepthStencilState.None,
				RasterizerState.CullNone
				);
        }
        protected void PrerenderEnd(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            _graphicsDevice.SetRenderTarget(null);
        }

        public virtual void Prerender(
            SpriteBatch spriteBatch,
            GlobalAssets globalAssets
            )
        {
            // use globalAssets to prerender 
        }
    }
}
