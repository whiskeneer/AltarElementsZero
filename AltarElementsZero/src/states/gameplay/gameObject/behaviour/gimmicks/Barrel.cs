using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.gimmicks
{

	class BarrelTop : IBehaviour
	{
		public static readonly BarrelTop Instance = new();

		public void Init(GameObject gameObject)
		{
			gameObject.Type = GameObject.Types.PUSHABLE;
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
		private enum State : uint
		{
			STOPPED,
			GOING_LEFT,
			GOING_RIGHT
		}

		public static readonly Barrel Instance = new();


		public void Init(GameObject gameObject)
		{
			if (gameObject.linkedObject == null) return;
			gameObject.linkedPosition = new PxPosition(3, (uint)(-16 & 0xffffffff)).ToSubpx();

			gameObject.Type = GameObject.Types.PUSHABLE;
			gameObject.isPersistentAcrossChunks = false;

			gameObject.drawOrder = GameObject.DrawOrderTypes.MIDDLE;
			gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x39);
			gameObject.atlasReference.Size = new PxSize(32, 32);
			gameObject.atlasReference.Effects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(5, 24);

			gameObject.currentBoundingBox.Size = new PxSize(22, 8).ToSubpx();
			gameObject.State = (uint)State.GOING_RIGHT;
		}

		public void Update(GameObject gameObject)
		{
			if(gameObject.linkedObject == null) return;

			// interaction with solid
			switch(gameObject.State)
			{
				case (uint)State.GOING_RIGHT:
					if(gameObject.PushedLeft)
					{
						gameObject.State = (uint) State.GOING_LEFT;
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

			// movement
			switch(gameObject.State)
			{
				case (uint)State.GOING_RIGHT:
					gameObject.previousVelocity.X = 64;
					gameObject.linkedObject.VelocityAbove= 64;
					break;
				case (uint)State.GOING_LEFT:
					gameObject.previousVelocity.X = -64;
					gameObject.linkedObject.VelocityAbove= -64;

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
						0x39 + ((gameObject.Timer >> 3) & 0x3)
						);

					break;
				case (uint)State.GOING_LEFT:
					gameObject.Timer--;
					gameObject.Timer &= 0xff;
					gameObject.atlasReference.Start =
						LegacyMapper.StartFromObjectSpritesheetIndex(
						0x39 + ((gameObject.Timer >> 3) & 0x3)
						);
					break;
				default:
					break;
			}

			gameObject.SimulateRegularObjectPhysics();

		}

		public void Interact(GameObject own, GameObject other)
		{
			IBehaviour otherBehaviour = other.behaviour;
		}

	}
}
