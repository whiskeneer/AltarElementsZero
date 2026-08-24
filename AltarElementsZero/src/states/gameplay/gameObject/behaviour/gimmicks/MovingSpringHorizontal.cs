using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.player;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour.gimmicks
{
	class MovingSpringHorizontal : IBehaviour
	{
		public static readonly MovingSpringHorizontal Instance = new();

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

			gameObject.currentBoundingBox.Size = new PxSize(16, 32).ToSubpx();

			gameObject.atlasReference.Start = new PxPosition(512,880);
			gameObject.atlasReference.Size = new PxSize(16, 32);
			gameObject.atlasReference.Effects = SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(0,0);

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
			int forceX;

			if(pushed && Math.Sign(displacement.X) == Math.Sign(delta.X) )
			{
				forceX = (-displacement.X) >> 8;
			}
			else
			{
				forceX = (-displacement.X) >> 4;
			}

			gameObject.AppliedForces += new Force(forceX, 0);

			gameObject.SimulateRegularObjectPhysics();

			gameObject.currentVelocity.Y = 0;
			gameObject.currentBoundingBox.Position.Y = originalPosition.Y;
		}

		public void Interact(GameObject own, GameObject other)
		{
			if(other.behaviour == Ora.Instance)
			{
				own.InteractionFlags |= (uint)FlagTypes.Pushed;
			}

		}
	}
}
