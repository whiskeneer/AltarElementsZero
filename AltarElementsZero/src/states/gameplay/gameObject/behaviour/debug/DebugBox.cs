using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.debug
{
    class DebugBox : IBehaviour
    {
        public static readonly DebugBox Instance = new ();

        public void Init(GameObject gameObject)
        {
            //gameObject.exists = true;
            //gameObject.isSolid = true;
            //gameObject.hurtsPlayer = false;
            //gameObject.isFixed = false;
            //gameObject.isAffectedByGravity = false;

            gameObject.isVisible = true;
            gameObject.spritesheetIndex = 0x30;
            gameObject.spriteEffects = SpriteEffects.None;

            gameObject.Size = new PxSize(16,16).ToSubpx();
            gameObject.SpriteOffset = new PxSize(0, 0);
        }

        public void Update(GameObject gameObject)
        {
		    gameObject.spritesheetIndex = 0x30;

            if (gameObject.PushingUp) gameObject.spritesheetIndex |= 0x1;
            if (gameObject.PushingDown) gameObject.spritesheetIndex |= 0x2;
            if (gameObject.PushingLeft) gameObject.spritesheetIndex |= 0x4;
			if (gameObject.PushingRight) gameObject.spritesheetIndex |= 0x8;


		}
	}
}
