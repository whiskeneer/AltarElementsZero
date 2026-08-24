using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.player;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.gimmicks
{
	class MovingSpringVertical : IBehaviour
	{
		public static readonly MovingSpringVertical Instance = new();

		[Flags]
		public enum FlagTypes : UInt32
		{
			None = 0,
			Pushed = 1 << 0,
		}

		public void Init(GameObject gameObject)
		{
			gameObject.Type = GameObject.Types.PUSHABLE;
			gameObject.isPersistentAcrossChunks = false;

			gameObject.drawOrder = GameObject.DrawOrderTypes.MIDDLE;

			gameObject.currentBoundingBox.Size = new PxSize(32, 16).ToSubpx();

			gameObject.atlasReference.Start = new PxPosition(512+16, 880);
			gameObject.atlasReference.Size = new PxSize(32, 16);
			gameObject.atlasReference.Effects = SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(0, 0);

			// use linkedPosition for storing original position (no actual linked object)
			gameObject.linkedPosition = gameObject.currentBoundingBox.Position;
			gameObject.secondLinkedPosition = gameObject.currentBoundingBox.Position;
		}

		public void Update(GameObject gameObject)
		{
			SubpxPosition previousPosition = gameObject.secondLinkedPosition;
			SubpxPosition currentPosition = gameObject.currentBoundingBox.Position;
			gameObject.secondLinkedPosition = currentPosition;
			SubpxPosition originalPosition = gameObject.linkedPosition;
			SubpxVelocity displacement = currentPosition - originalPosition;
			SubpxVelocity delta = currentPosition - previousPosition;

			bool pushed = ((FlagTypes)gameObject.InteractionFlags & FlagTypes.Pushed) == FlagTypes.Pushed;

			//int forceX, forceY;
			int forceY;

			if (pushed && Math.Sign(displacement.Y) == Math.Sign(delta.Y))
			{
				forceY = (-displacement.Y) >> 4;
			}
			else
			{
				forceY = (-displacement.Y) >> 2;
			}

			gameObject.AppliedForces += new Force(0, forceY);

			gameObject.SimulateRegularObjectPhysics();

			gameObject.currentVelocity.X = 0;
			gameObject.currentBoundingBox.Position.X = originalPosition.X;
		}

		public void Interact(GameObject own, GameObject other)
		{
			if (other.behaviour == Ora.Instance)
			{
				own.InteractionFlags |= (uint)FlagTypes.Pushed;
			}

		}
	}
}
