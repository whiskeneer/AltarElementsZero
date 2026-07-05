using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.player;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.gimmicks
{

	class BarrelTop : IBehaviour
	{
		public static readonly BarrelTop Instance = new();

		public void Init(GameObject gameObject)
		{
			gameObject.Type = GameObject.Types.RESERVED;
			gameObject.isPersistentAcrossChunks = false;

			gameObject.drawOrder = GameObject.DrawOrderTypes.NONE;
			gameObject.atlasReference.Start = new(96, 0);
			gameObject.atlasReference.Size = new PxSize(16, 16);
			gameObject.atlasReference.Effects = SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(0, 0);

			gameObject.currentBoundingBox.Size = new PxSize(16, 16).ToSubpx();
		}
		public void Update(GameObject gameObject)
		{
		}
		public void Interact(GameObject own, GameObject other)
		{

		}
	}
	
	class Barrel : IBehaviour
	{
		public enum State : uint
		{
			STOPPED,
			GOING_LEFT,
			GOING_RIGHT
		}

		public enum SubState : uint
		{
			NONE,
			PREVIOUSLY_HIT,
		}

		public enum FlagTypes : UInt32
		{
			None = 0,
			HitFromTheLeft = 1 << 0,
			HitFromTheRight = 1 << 1,
		}

		public static readonly Barrel Instance = new();


		public void Init(GameObject gameObject)
		{
			if (gameObject.linkedObject == null) return;

			gameObject.isPersistentAcrossChunks = false;
			gameObject.drawOrder = GameObject.DrawOrderTypes.MIDDLE;
			gameObject.atlasReference.Size = new PxSize(32, 32);
			gameObject.atlasReference.Effects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
			gameObject.Type = GameObject.Types.PUSHABLE;
			gameObject.SubState = (uint)SubState.NONE;


			switch(gameObject.spawnValue)
			{
				case 0:
				default:
					gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x38);
					gameObject.atlasReference.Offset = new PxPosition(5, 0);
					gameObject.currentBoundingBox.Size = new PxSize(22, 32).ToSubpx();
					gameObject.State = (uint)State.STOPPED;
					gameObject.linkedPosition = new PxPosition(3, 8).ToSubpx();
					gameObject.linkedObject.Type = GameObject.Types.RESERVED;
					break;
				case 1:
					gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x39);
					gameObject.atlasReference.Offset = new PxPosition(5, 24);
					gameObject.currentBoundingBox.Size = new PxSize(22, 8).ToSubpx();
					gameObject.State = (uint)State.GOING_RIGHT;
					gameObject.linkedPosition = new PxPosition(3, (uint)(-16 & 0xffffffff)).ToSubpx();
					gameObject.linkedObject.Type = GameObject.Types.PUSHABLE;
					gameObject.currentBoundingBox.Position += new PxPosition(0, 24).ToSubpx();
					break;
				case 2:
					gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x39);
					gameObject.atlasReference.Offset = new PxPosition(5, 24);
					gameObject.currentBoundingBox.Size = new PxSize(22, 8).ToSubpx();
					gameObject.State = (uint)State.GOING_LEFT;
					gameObject.linkedPosition = new PxPosition(3, (uint)(-16 & 0xffffffff)).ToSubpx();
					gameObject.linkedObject.Type = GameObject.Types.PUSHABLE;
					gameObject.currentBoundingBox.Position += new PxPosition(0, 24).ToSubpx();
					break;
			}




		}

		public void Update(GameObject gameObject)
		{
			if (gameObject.linkedObject == null) return;

			// interaction with solid
			switch (gameObject.State)
			{
				case (uint)State.GOING_RIGHT:
					if (gameObject.PushedLeft)
					{
						gameObject.State = (uint)State.GOING_LEFT;
					}
					break;
				case (uint)State.GOING_LEFT:
					if (gameObject.PushedRight)
					{
						gameObject.State = (uint)State.GOING_RIGHT;
					}
					break;
				default:
					break;
			}
			// interaction with scythe

			if((SubState)gameObject.SubState != SubState.PREVIOUSLY_HIT){
				switch (gameObject.State)
				{
					case (uint)State.GOING_RIGHT:
						if (((FlagTypes)gameObject.InteractionFlags & FlagTypes.HitFromTheRight) == FlagTypes.HitFromTheRight)
						{
							gameObject.currentBoundingBox.Size = new PxSize(22, 32).ToSubpx();
							gameObject.State = (uint)State.STOPPED;
							gameObject.linkedPosition = new PxPosition(3, 8).ToSubpx();
							gameObject.linkedObject.Type = GameObject.Types.RESERVED;
							gameObject.previousBoundingBox.Position += new PxPosition(0, (uint)(-24 & 0xffffffff)).ToSubpx();
							gameObject.currentBoundingBox.Position += new PxPosition(0, (uint)(-24 & 0xffffffff)).ToSubpx();
						}
						break;
					case (uint)State.GOING_LEFT:
						if (((FlagTypes)gameObject.InteractionFlags & FlagTypes.HitFromTheLeft) == FlagTypes.HitFromTheLeft)
						{
							gameObject.currentBoundingBox.Size = new PxSize(22, 32).ToSubpx();
							gameObject.State = (uint)State.STOPPED;
							gameObject.linkedPosition = new PxPosition(3, 8).ToSubpx();
							gameObject.linkedObject.Type = GameObject.Types.RESERVED;
							gameObject.previousBoundingBox.Position += new PxPosition(0, (uint)(-24 & 0xffffffff)).ToSubpx();
							gameObject.currentBoundingBox.Position += new PxPosition(0, (uint)(-24 & 0xffffffff)).ToSubpx();
						}
						break;
					case (uint)State.STOPPED:
						if (((FlagTypes)gameObject.InteractionFlags & FlagTypes.HitFromTheRight) == FlagTypes.HitFromTheRight)
						{
							gameObject.currentBoundingBox.Size = new PxSize(22, 8).ToSubpx();
							gameObject.State = (uint)State.GOING_LEFT;
							gameObject.linkedPosition = new PxPosition(3, (uint)(-16 & 0xffffffff)).ToSubpx();
							gameObject.linkedObject.Type = GameObject.Types.PUSHABLE;
							gameObject.previousBoundingBox.Position += new PxPosition(0, 24).ToSubpx();
							gameObject.currentBoundingBox.Position += new PxPosition(0, 24).ToSubpx();
						}
						else if (((FlagTypes)gameObject.InteractionFlags & FlagTypes.HitFromTheLeft) == FlagTypes.HitFromTheLeft)
						{
							gameObject.currentBoundingBox.Size = new PxSize(22, 8).ToSubpx();
							gameObject.State = (uint)State.GOING_RIGHT;
							gameObject.linkedPosition = new PxPosition(3, (uint)(-16 & 0xffffffff)).ToSubpx();
							gameObject.linkedObject.Type = GameObject.Types.PUSHABLE;
							gameObject.previousBoundingBox.Position += new PxPosition(0, 24).ToSubpx();
							gameObject.currentBoundingBox.Position += new PxPosition(0, 24).ToSubpx();
						}
						break;
					default:
						break;
				}
			}

			if (((FlagTypes)gameObject.InteractionFlags & FlagTypes.HitFromTheLeft) == FlagTypes.HitFromTheLeft ||
				((FlagTypes)gameObject.InteractionFlags & FlagTypes.HitFromTheRight) == FlagTypes.HitFromTheRight)
			{
				gameObject.SubState = (uint)SubState.PREVIOUSLY_HIT;
			}
			else
			{
				gameObject.SubState = (uint)SubState.NONE;
			}

			// movement
			switch (gameObject.State)
			{
				case (uint)State.GOING_RIGHT:
					gameObject.previousVelocity.X = 64;
					gameObject.linkedObject.VelocityAbove= 64;
					break;
				case (uint)State.GOING_LEFT:
					gameObject.previousVelocity.X = -64;
					gameObject.linkedObject.VelocityAbove = -64;
					break;
				case (uint)State.STOPPED:
					gameObject.previousVelocity.X = 0;
					gameObject.linkedObject.VelocityAbove = 0;
					break;
				default:
					break;
			}

			// animation
			switch (gameObject.State)
			{
				case (uint)State.GOING_RIGHT:
					gameObject.Timer++;
					gameObject.Timer &= 0xff;
					gameObject.atlasReference.Start =
						LegacyMapper.StartFromObjectSpritesheetIndex(
						0x39 + ((gameObject.Timer >> 2) & 0x3)
						);
					gameObject.atlasReference.Offset = new PxPosition(5, 24); // redundant but explicit
					break;
				case (uint)State.GOING_LEFT:
					gameObject.Timer--;
					gameObject.Timer &= 0xff;
					gameObject.atlasReference.Start =
						LegacyMapper.StartFromObjectSpritesheetIndex(
						0x39 + ((gameObject.Timer >> 2) & 0x3)
						);
					gameObject.atlasReference.Offset = new PxPosition(5, 24); // redundant but explicit
					break;
				case (uint)State.STOPPED:
					gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x38);
					gameObject.atlasReference.Offset = new PxPosition(5, 0);
					break;
				default:
					break;
			}

			gameObject.SimulateRegularObjectPhysics();

		}

		public void Interact(GameObject own, GameObject other)
		{
			IBehaviour otherBehaviour = other.behaviour;
			if(otherBehaviour == Scythe.Instance && (Scythe.State)other.State == Scythe.State.ACTIVE)
			{
				if(other.currentBoundingBox.Center().X < own.currentBoundingBox.Center().X)
				{
					own.InteractionFlags |= (uint)FlagTypes.HitFromTheLeft;
				}
				else
				{
					own.InteractionFlags |= (uint)FlagTypes.HitFromTheRight;
				}
			}
		}

	}
}
