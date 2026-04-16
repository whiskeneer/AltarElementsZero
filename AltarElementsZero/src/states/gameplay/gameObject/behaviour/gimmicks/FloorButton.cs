using AltarElementsZero.src.states.gameplay.vectors;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.gimmicks
{
    class FloorButton : IBehaviour
    {
        public static readonly FloorButton Instance = new();

        [Flags]
        private enum FlagTypes : UInt32
        {
            None = 0,
            Pushed = 1 << 0,
        }

        public void Init(GameObject gameObject)
        {
            gameObject.Type = GameObject.Types.REGION;
            gameObject.isPersistentAcrossChunks = false;

            gameObject.isVisible = true;
            gameObject.spritesheetIndex = 9;
            gameObject.SpriteOffset = new(0, 0);
            gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;

            gameObject.currentBoundingBox.Size = new PxSize(16, 16).ToSubpx();
        }

        public void Update(GameObject gameObject)
        {
            if(((FlagTypes)gameObject.InteractionFlags & FlagTypes.Pushed) == FlagTypes.Pushed)
            {
				gameObject.spritesheetIndex = 10;
                GameObject.signalFlags!.SetSignalFlag(gameObject.spawnValue, true);
            }
            else
            {
                gameObject.spritesheetIndex = 9;
                GameObject.signalFlags!.SetSignalFlag(gameObject.spawnValue, false);
			}
		}
        public void Interact(GameObject own, GameObject other)
        {
            if(other.Type == GameObject.Types.PUSHABLE)
            {
                own.InteractionFlags |= (UInt32) FlagTypes.Pushed; 
            }
        }
    }
}
