using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src
{
    interface IState
    {
        public void Enter();
        public void Update(GameTime gameTime);
        public void Prerender(SpriteBatch spriteBatch);
        public void Draw(SpriteBatch spriteBatch);
        public void Exit();
    }

    abstract class State<TAssets, TPayload>(
            IManager manager,
            TPayload payload,
            TAssets assets,
            InputHandler inputHandler,
            GlobalAssets globalAssets
        ) : IState
		where TAssets : LocalAssets
        where TPayload : Payload
    {
        protected readonly IManager _manager = manager;
        protected readonly TPayload _payload = payload;
        protected readonly TAssets _assets = assets;
        protected readonly InputHandler _inputHandler = inputHandler;
        protected readonly GlobalAssets _globalAssets = globalAssets;

        protected bool IsPrerendered = false;

        public virtual void Enter()
        {
            _assets.Load();
        }
        public virtual void Update(GameTime gameTime)
        {

        }
        public virtual void Prerender(SpriteBatch spriteBatch)
        {
            if (!IsPrerendered)
            {
                _assets.Prerender(spriteBatch, _globalAssets);
                IsPrerendered = true;
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
        public virtual void Exit()
        {
            _assets.Unload();
        }
    }
}
