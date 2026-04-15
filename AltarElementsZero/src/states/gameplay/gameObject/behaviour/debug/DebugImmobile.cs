using AltarElementsZero.src.states.gameplay.vectors;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.debug
{
    class DebugImmobile : IBehaviour
    {
        public static readonly DebugImmobile Instance = new ();

        public void Init(GameObject gameObject)
        {
            gameObject.Type = GameObject.Types.IMMOBILE;
			gameObject.isPersistentAcrossChunks = false;

			gameObject.isVisible = true;
            gameObject.spritesheetIndex = 0x2e;
            gameObject.SpriteOffset = new(0, 0);
            gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;

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
