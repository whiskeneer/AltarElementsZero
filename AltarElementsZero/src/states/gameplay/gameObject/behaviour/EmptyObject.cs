using AltarElementsZero.src.renderer;
using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.states.gameplay.gameObject.behaviour
{
    class EmptyObject : IBehaviour
    {
        public static readonly EmptyObject Instance = new();

        public void Init(GameObject gameObject)
        {
            //gameObject.exists = false;
            gameObject.Type = GameObject.Types.NONEXISTENT;

            gameObject.spawnValue = 0;
			gameObject.isPersistentAcrossChunks = false;
            gameObject.drawOrder = GameObject.DrawOrderTypes.NONE;

			gameObject.atlasReference.Start = LegacyMapper.StartFromObjectSpritesheetIndex(0x0);
			gameObject.atlasReference.Size = new PxSize(0,0);
			gameObject.atlasReference.Effects = SpriteEffects.None;
			gameObject.atlasReference.Offset = new PxPosition(0, 0);
			gameObject.atlasReference.RepeatX = (byte)(0);
			gameObject.atlasReference.RepeatY = (byte)(0);

			gameObject.previousVelocity = new();
            gameObject.currentVelocity = new();
            gameObject.previousBoundingBox = new();
            gameObject.currentBoundingBox = new();

            gameObject.PushedUp = false;
            gameObject.PushedDown = false;
            gameObject.PushedLeft = false;
            gameObject.PushedRight = false;

            gameObject.PushedPreviouslyUp = false;
            gameObject.PushedPreviouslyDown = false;
            gameObject.PushedPreviouslyLeft = false;
            gameObject.PushedPreviouslyRight = false;

            gameObject.VelocityBelow = 0;
            gameObject.VelocityAbove = 0;
			gameObject.FrictionCoefficientsBelow = new();
            gameObject.VelocityAround = new();
            gameObject.FrictionCoefficientAround = 0;

            gameObject.GroundImpulse = 0;
            gameObject.AirImpulse = new();

            gameObject.AppliedForces = new();

            gameObject.FluidVelocity = new();
            gameObject.FluidCoefficient = 0;
            gameObject.FluidGravity = new();


			gameObject.State = 0;
            gameObject.SubState = 0;
            gameObject.Timer = 0;
            gameObject.Timer2 = 0;
            gameObject.Timer3 = 0;
            gameObject.Timer4 = 0;

            gameObject.SavedSpeed = 0;

            gameObject.InteractionFlags = 0;

            gameObject.linkedObject = null;
            gameObject.linkedPosition = new();

            gameObject.secondLinkedObject = null;
            gameObject.secondLinkedPosition = new();


			return;
        }
        public void Update(GameObject gameObject)
        {
            return;
        }
		public void Interact(GameObject own, GameObject other)
        {
            return;
        }

	}
}
