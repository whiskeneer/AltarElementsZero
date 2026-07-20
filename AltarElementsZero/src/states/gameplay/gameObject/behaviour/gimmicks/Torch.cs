using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.enemies;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.gimmicks
{
	class Torch : IBehaviour
	{

		public enum State : uint
		{
			OFF,
			ON
		}
		public enum FlagTypes : UInt32
		{
			None = 0,
			TurnOn = 1 << 0,
			TurnOff = 1 << 1,
		}


		public static readonly Torch Instance = new();

		public void Init(GameObject gameObject)
		{
			gameObject.Type = GameObject.Types.PUSHABLE;
			gameObject.isPersistentAcrossChunks = false;

			gameObject.drawOrder = GameObject.DrawOrderTypes.MIDDLE;
			gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x33) + new PxPosition(5,0);
			gameObject.atlasReference.Size = new PxSize(6, 16);
			gameObject.atlasReference.Effects = SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(0, 4);

			gameObject.currentBoundingBox.Size = new PxSize(6, 12).ToSubpx();
			gameObject.State = (uint)State.OFF;
		}

		public void Update(GameObject gameObject)
		{
			ref uint animationTimer = ref gameObject.Timer;

			if (((FlagTypes)gameObject.InteractionFlags & FlagTypes.TurnOn) == FlagTypes.TurnOn)
			{
				gameObject.State = (uint)State.ON;
			}
			if(((FlagTypes)gameObject.InteractionFlags & FlagTypes.TurnOff) == FlagTypes.TurnOff)
			{
				gameObject.State = (uint)State.OFF;
			}


			animationTimer++;
			if (gameObject.State == (uint)State.OFF)
			{
				gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x33) + new PxPosition(5, 0);
			}
			else
			{
				if((animationTimer & 0x4) == 0x4)
				{
					gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x34) + new PxPosition(5, 0);
				}
				else
				{
					gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x34) + new PxPosition(16,0) + new PxPosition(5, 0);
				}
			}
			gameObject.SimulateRegularObjectPhysics();
		}
		public void Interact(GameObject own, GameObject other)
		{
			IBehaviour otherBehaviour = other.behaviour;
			if (otherBehaviour == Fire.Instance)
			{
				own.InteractionFlags |= (UInt32)FlagTypes.TurnOn;
			}
			if(otherBehaviour == Vine.Instance && (Vine.State)other.State == Vine.State.BURNING)
			{
				own.InteractionFlags |= (UInt32)FlagTypes.TurnOn;
			}

			if (otherBehaviour == WaterRegion.Instance)
			{
				own.InteractionFlags |= (Int32)FlagTypes.TurnOff;
			}

		}
	}
}
