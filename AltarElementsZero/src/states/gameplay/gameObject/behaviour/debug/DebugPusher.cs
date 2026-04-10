using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.debug
{
    class DebugPusher : IBehaviour
    {
        public static readonly DebugPusher Instance = new ();

        public void Init(GameObject gameObject)
        {
			gameObject.Type = GameObject.Types.UNSTOPPABLE;

			//gameObject.exists = true;
			//gameObject.isSolid = true;
			//gameObject.hurtsPlayer = false;
			//gameObject.isFixed = false;
			//gameObject.isAffectedByGravity = false;

			gameObject.isVisible = true;
            gameObject.spritesheetIndex = 0x2f;
            gameObject.SpriteOffset = new(0, 0);
            gameObject.spriteEffects = SpriteEffects.None;

            gameObject.boundingBox.Size = new PxSize(16,16).ToSubpx();
            //gameObject.isSelfMoving = true;
        }

        public void Update(GameObject gameObject)
        {
            InputHandler? inputHandler = GameObject.inputHandler;
            if (inputHandler != null)
            {
                if (inputHandler.IsDown(Input.Up))
                {
                    gameObject.Velocity.Y = -64;
                }
                else if (inputHandler.IsDown(Input.Down))
                {
                    gameObject.Velocity.Y = 64;
                }
                else
                {
                    gameObject.Velocity.Y = 0;
                }

				if (inputHandler.IsDown(Input.Left))
				{
					gameObject.Velocity.X = -64;
				}
				else if (inputHandler.IsDown(Input.Right))
				{
					gameObject.Velocity.X = 64;
				}
				else
				{
					gameObject.Velocity.X = 0;
				}
			}
        }
    }
}
