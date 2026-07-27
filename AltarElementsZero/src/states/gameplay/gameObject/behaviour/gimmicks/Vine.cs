using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;
using static AltarElementsZero.src.states.gameplay.gameObject.behaviour.player.Scythe;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.gimmicks
{
	class Vine : IBehaviour
	{
		public static readonly Vine Instance = new();

		public enum State : uint
		{
			IDLE,
			BURNING,
			EXHAUSTED
		}
		public enum FlagTypes : UInt32
		{
			None = 0,
			Burn = 1 << 0,
		}


		public void Init(GameObject gameObject)
		{
			gameObject.Type = GameObject.Types.IMMOBILE;
			gameObject.isPersistentAcrossChunks = false;

			gameObject.drawOrder = GameObject.DrawOrderTypes.BACK;
			gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x35);
			gameObject.atlasReference.Size = new PxSize(16, 16);
			gameObject.atlasReference.Effects = SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(0, 0);

			gameObject.currentBoundingBox.Size = new PxSize(16, 16).ToSubpx();

			if(gameObject.spawnValue == 1)
			{
				gameObject.State = (uint)State.BURNING;
				gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x36);
				gameObject.Timer = 30;
			}
		}

		public void Update(GameObject gameObject)
		{
			ref uint animationTimer = ref gameObject.Timer2;
			animationTimer++;

			// UPDATE STATE
			switch ((State)gameObject.State)
			{
				case State.IDLE:
					if(((FlagTypes)gameObject.InteractionFlags & FlagTypes.Burn) == FlagTypes.Burn)
					{
						gameObject.State = (uint)State.BURNING;
						gameObject.atlasReference.Start = GetBurningAnimation(gameObject);
						gameObject.Timer = 30;

						GameObject.globalAssets!.BurningVineSFXInstance!.Stop();
						GameObject.globalAssets!.BurningVineSFXInstance!.Play();
					}
					break;
				case State.BURNING:
					if(gameObject.Timer == 0)
					{
						gameObject.State = (uint)State.EXHAUSTED;
						gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x37);
						gameObject.atlasReference.Offset = new PxPosition(7, 7);
						gameObject.drawOrder = GameObject.DrawOrderTypes.NONE;

						gameObject.currentBoundingBox.Size = new PxSize(18, 18).ToSubpx();
						gameObject.currentBoundingBox.Position += new SubpxPosition((uint)(-1 & 0xffffffff), (uint)(-1 & 0xffffffff));
					}
					else
					{
						gameObject.atlasReference.Start = GetBurningAnimation(gameObject);
						gameObject.Timer--;
					}
					break;

				case State.EXHAUSTED:
					gameObject.Delete();
					return;
			}


		}

		public static PxPosition GetBurningAnimation(GameObject gameObject)
		{
			ref uint animationTimer = ref gameObject.Timer2;
			switch ((animationTimer >> 2) & 0x3)
			{
				case 0:
				default:
					return gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x36);
				case 1:
					return gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x36) + new PxPosition(16, 0);
				case 2:
					return gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x36) + new PxPosition(0, 16);
				case 3:
					return gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x36) + new PxPosition(16, 16);
			}
		}


		public void Interact(GameObject own, GameObject other)
		{
			IBehaviour otherBehaviour = other.behaviour;

			if( otherBehaviour == Torch.Instance &&
				(Torch.State)other.State == Torch.State.ON)
			{
				own.InteractionFlags |= (UInt32)FlagTypes.Burn;
			}
			if ( otherBehaviour == Vine.Instance &&
				(Vine.State)other.State == Vine.State.EXHAUSTED)
			{
				own.InteractionFlags |= (UInt32)FlagTypes.Burn;
			}
		}
	}
}
