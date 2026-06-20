using AltarElementsZero.src.renderer;
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
            gameObject.isPersistentAcrossChunks = false;

			gameObject.drawOrder = GameObject.DrawOrderTypes.MIDDLE;

            gameObject.currentBoundingBox.Size = new PxSize(16,16).ToSubpx();

            //gameObject.spritesheetIndex = 0x0f;
            //gameObject.spriteEffects = SpriteEffects.None;
            //gameObject.SpriteOffset = new PxSize(0, 0);

            gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x0f);
            gameObject.atlasReference.Size = new PxSize(32, 32);
            gameObject.atlasReference.Effects = SpriteEffects.None;
            gameObject.atlasReference.Offset = new PxPosition(0, 0);


		}

        public void Update(GameObject gameObject)
        {
			//gameObject.spritesheetIndex = 0x0f;
			gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x0f);

			gameObject.SimulateRegularObjectPhysics();

		}
		public void Interact(GameObject own, GameObject other)
		{
			return;
		}
	}
}
