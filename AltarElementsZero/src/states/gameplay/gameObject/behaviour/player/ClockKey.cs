using AltarElementsZero.src.states.gameplay.vectors;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.player
{
	class ClockKey : IBehaviour
	{
		public static readonly ClockKey Instance = new();

		[Flags]
		private enum FlagTypes : UInt32
		{
			None = 0,
			Collected = 1 << 0,
		}
		public void Init(GameObject gameObject)
		{
			gameObject.Type = GameObject.Types.REGION;
			gameObject.isPersistentAcrossChunks = false;

			gameObject.drawOrder = GameObject.DrawOrderTypes.NONE;
			gameObject.atlasReference.Start = new PxPosition(512, 784);
			gameObject.atlasReference.Size = new PxSize(16, 16);
			gameObject.atlasReference.Effects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(0, 0);

			gameObject.currentBoundingBox.Size = new PxSize(0, 0).ToSubpx();
		}

		public void Update(GameObject gameObject)
		{
			if(((FlagTypes)gameObject.InteractionFlags & FlagTypes.Collected) == FlagTypes.Collected)
			{
				GameObject.signalFlags!.SetSignalFlag(gameObject.spawnValue, true);
			}

			if(GameObject.signalFlags!.GetSignalFlag(gameObject.spawnValue))
			{
				gameObject.drawOrder = GameObject.DrawOrderTypes.NONE;
				gameObject.currentBoundingBox.Size = new PxSize(16, 16).ToSubpx();
			}
			else
			{
				ref uint animationTimer = ref gameObject.Timer;
				animationTimer++;
				gameObject.drawOrder = GameObject.DrawOrderTypes.MIDDLE;
				gameObject.currentBoundingBox.Size = new PxSize(16, 16).ToSubpx();


				switch ((animationTimer >> 2) & 0x7)
				{
					case 0:
					case 1:
					case 2:
						gameObject.atlasReference.Start = new PxPosition(512, 784);
						break;
					case 3:
						gameObject.atlasReference.Start = new PxPosition(512 + 16, 784);
						break;
					case 4:
						gameObject.atlasReference.Start = new PxPosition(512 + 16*2, 784);
						break;
					case 5:
						gameObject.atlasReference.Start = new PxPosition(512 + 16*3, 784);
						break;
					case 6:
						gameObject.atlasReference.Start = new PxPosition(512 + 16*4, 784);
						break;
					case 7:
						gameObject.atlasReference.Start = new PxPosition(512 + 16*5, 784);
						break;
				}
			}

		}
		public void Interact(GameObject own, GameObject other)
		{
			if(other.behaviour == Ora.Instance)
			{
				own.InteractionFlags |= (UInt32)FlagTypes.Collected;
			}
		}
	}
}
