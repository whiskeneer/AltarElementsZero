using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.gimmicks
{
    class SwitchableDoor : IBehaviour
    {
        public static readonly SwitchableDoor Instance = new();

        public void Init(GameObject gameObject)
        {
            gameObject.Type = GameObject.Types.UNSTOPPABLE;
            gameObject.isPersistentAcrossChunks = false;

            gameObject.drawOrder = GameObject.DrawOrderTypes.MIDDLE;
			//gameObject.spritesheetIndex = 11;
			//gameObject.SpriteOffset = new(0, 0);
			//gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
			gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(11);
			gameObject.atlasReference.Size = new PxSize(32, 32);
			gameObject.atlasReference.Effects = SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(0, 0);

			gameObject.currentBoundingBox.Size = new PxSize(16, 32).ToSubpx();
        }

        public void Update(GameObject gameObject)
        {
            if (GameObject.signalFlags!.GetSignalFlag(gameObject.spawnValue))
			{
                //gameObject.spritesheetIndex = 12;
				gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(12);
				gameObject.currentBoundingBox.Size = new PxSize(16, 0).ToSubpx();
			}
            else
            {
				//gameObject.spritesheetIndex = 11;
				gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(11);
				gameObject.currentBoundingBox.Size = new PxSize(16, 32).ToSubpx();
			}
		}

        public void Interact(GameObject own, GameObject other)
        {
            return;
        }
    }
}
