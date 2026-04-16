using AltarElementsZero.src.states.gameplay.vectors;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.gimmicks
{
    class SwitchableDoor : IBehaviour
    {
        public static readonly SwitchableDoor Instance = new();

        public void Init(GameObject gameObject)
        {
            gameObject.Type = GameObject.Types.UNSTOPPABLE;
            gameObject.isPersistentAcrossChunks = false;

            gameObject.isVisible = true;
            gameObject.spritesheetIndex = 11;
            gameObject.SpriteOffset = new(0, 0);
            gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;

            gameObject.currentBoundingBox.Size = new PxSize(16, 32).ToSubpx();
        }

        public void Update(GameObject gameObject)
        {
            if (GameObject.signalFlags!.GetSignalFlag(gameObject.spawnValue))
			{
                gameObject.spritesheetIndex = 12;
				gameObject.currentBoundingBox.Size = new PxSize(16, 0).ToSubpx();
			}
            else
            {
				gameObject.spritesheetIndex = 11;
				gameObject.currentBoundingBox.Size = new PxSize(16, 32).ToSubpx();
			}
		}

        public void Interact(GameObject own, GameObject other)
        {
            return;
        }
    }
}
