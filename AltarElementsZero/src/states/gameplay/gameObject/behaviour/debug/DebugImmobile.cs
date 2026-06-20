using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.debug
{
    class DebugImmobile : IBehaviour
    {
        public static readonly DebugImmobile Instance = new ();

        public void Init(GameObject gameObject)
        {
            gameObject.Type = GameObject.Types.IMMOBILE;
			gameObject.isPersistentAcrossChunks = false;

			gameObject.drawOrder = GameObject.DrawOrderTypes.MIDDLE;
			//gameObject.spritesheetIndex = 0x2e;
			//gameObject.SpriteOffset = new(0, 0);
			//gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
			gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x2e);
			gameObject.atlasReference.Size = new PxSize(32, 32);
			gameObject.atlasReference.Effects = SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(0, 0);


			gameObject.currentBoundingBox.Size = new PxSize(16, 16).ToSubpx();

		}
        public void Update(GameObject gameObject)
        {
            // Does nothing
        }
		public void Interact(GameObject own, GameObject other)
		{
			return;
		}
	}
}
