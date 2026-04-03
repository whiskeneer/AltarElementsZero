using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src
{
    sealed class GlobalAssets(
        GraphicsDevice graphicsDevice,
        GameServiceContainer gameServiceContainer) 
        : Assets(
            graphicsDevice: graphicsDevice,
            gameServiceContainer: gameServiceContainer)
    {
        public Texture2D? Placeholder { get; private set; }
        public Texture2D? RomanFont { get; private set; }

        public override void Load()
        {
            base.Load();

            Placeholder = _contentManager!.Load<Texture2D>("img/default_placeholder.png");
            RomanFont = _contentManager!.Load<Texture2D>("img/font_placeholder.png");
        }

        public override void Unload()
        {
            base.Unload();

            Placeholder = null;
            RomanFont = null;
        }
    }
}
