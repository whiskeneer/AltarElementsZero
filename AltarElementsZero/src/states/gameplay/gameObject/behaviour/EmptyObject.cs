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
            gameObject.spritesheetIndex = 0;
            gameObject.spriteEffects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
            gameObject.SpriteOffset = new();

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
            gameObject.FrictionCoefficientsBelow = new();
            gameObject.VelocityAround = new();
            gameObject.FrictionCoefficientAround = 0;

            gameObject.GroundImpulse = 0;
            gameObject.AirImpulse = new();

            gameObject.AppliedForces = new();

            gameObject.FluidVelocity = new();
            gameObject.FluidCoefficient = 0;

            gameObject.State = 0;
            gameObject.SubState = 0;
            gameObject.Timer = 0;
            gameObject.Timer2 = 0;

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
