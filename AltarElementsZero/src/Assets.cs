using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src
{
    abstract class Assets(
        GraphicsDevice graphicsDevice, 
        GameServiceContainer gameServiceContainer)
    {
        protected readonly GraphicsDevice _graphicsDevice = graphicsDevice;
        protected readonly GameServiceContainer _gameServiceContainer = gameServiceContainer;

        protected ContentManager? _contentManager;

        public virtual void Load()
        {
            _contentManager = new ContentManager(_gameServiceContainer, "assets");
        }
        public virtual void Unload()
        {
            _contentManager!.Unload();
            _contentManager!.Dispose();
            _contentManager = null;
        }

    }
}
