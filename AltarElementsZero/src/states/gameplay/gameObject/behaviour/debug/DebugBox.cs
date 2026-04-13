using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.debug
{
    class DebugBox : IBehaviour
    {
        public static readonly DebugBox Instance = new ();

        public void Init(GameObject gameObject)
        {
			gameObject.Type = GameObject.Types.PUSHABLE;

			gameObject.isVisible = true;
            gameObject.spritesheetIndex = 0x30;
            gameObject.spriteEffects = SpriteEffects.None;

            gameObject.currentBoundingBox.Size = new PxSize(16,16).ToSubpx();
            gameObject.SpriteOffset = new PxSize(0, 0);
        }

        public void Update(GameObject gameObject)
        {
		    gameObject.spritesheetIndex = 0x30;

            gameObject.SimulateRegularObjectPhysics();

		}
	}
}
