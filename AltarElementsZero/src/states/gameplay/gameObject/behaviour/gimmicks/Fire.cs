using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.gimmicks
{
	class Fire : IBehaviour
	{
		public static readonly Fire Instance = new();

		public void Init(GameObject gameObject)
		{
			gameObject.Type = GameObject.Types.REGION;
			gameObject.isPersistentAcrossChunks = false;

			gameObject.drawOrder = GameObject.DrawOrderTypes.BACK;
			gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x32);
			gameObject.atlasReference.Size = new PxSize(16, 16);
			gameObject.atlasReference.Effects = SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(0, 0);

			gameObject.currentBoundingBox.Size = new PxSize(16, 16).ToSubpx();

		}

		public void Update(GameObject gameObject)
		{
			ref uint animationTimer = ref gameObject.Timer;

			animationTimer++;
			switch ((animationTimer >> 2) & 0x3)
			{
				case 0:
					gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x32);
					break;
				case 1:
					gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x32) + new PxPosition(16, 0);
					break;
				case 2:
					gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x32) + new PxPosition(0, 16);
					break;
				case 3:
					gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x32) + new PxPosition(16, 16);
					break;
			}
		}
		public void Interact(GameObject own, GameObject other)
		{
		}
	}
}
