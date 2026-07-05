using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.vectors;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.player
{
	class Portal : IBehaviour
	{
		public static readonly Portal Instance = new();

		public void Init(GameObject gameObject)
		{
			gameObject.Type = GameObject.Types.REGION;
			gameObject.isPersistentAcrossChunks = false;
			
			gameObject.drawOrder = GameObject.DrawOrderTypes.BACK;
			gameObject.atlasReference.Start = new PxPosition(512, 800);
			gameObject.atlasReference.Size = new PxSize(32, 32);
			gameObject.atlasReference.Effects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(0, 0);

			gameObject.currentBoundingBox.Size = new PxSize(32, 32).ToSubpx();
		}

		public void Update(GameObject gameObject)
		{
			ref uint animationTimer = ref gameObject.Timer;

			animationTimer++;
			switch ((animationTimer >> 2) & 0x3)
			{
				case 0:
					gameObject.atlasReference.Start = new PxPosition(512, 800);
					break;
				case 1:
					gameObject.atlasReference.Start = new PxPosition(512 + 32, 800);
					break;
				case 2:
					gameObject.atlasReference.Start = new PxPosition(512 + 64, 800);
					break;
				case 3:
					gameObject.atlasReference.Start = new PxPosition(512 + 96, 800);
					break;
			}
		}
		public void Interact(GameObject own, GameObject other)
		{
		}
	}
}
