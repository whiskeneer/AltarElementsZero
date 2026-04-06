using AltarElementsZero.src.states.gameplay.vectors;

namespace AltarElementsZero.src.states.gameplay.gameObject
{

    sealed class GameObject
    {
		public static GameObject GetTestObject()
		{
			GameObject testObject = new();
			testObject.Size = new PxSize(
				(uint)Configuration.Tile.Px.Width,
				(uint)Configuration.Tile.Px.Height
				).ToSubpx();
			return testObject;
		}

		public SubpxPosition Position;
        public SubpxSize Size;
        public SubpxVelocity Velocity;
        public bool Grounded = false;
        public SubpxVelocity MediumVelocity;
        public SubpxVelocity GroundVelocity;
        public int GroundMuKin;
        public int GroundMuSta;
        public SubpxVelocity FeetVelocity; // Refers to efforts to move on ground
        // public SubpxVelocity WingVelocity; // Refers to efforts to move on air ( + ground )

        // BEHAVIOUR
        public bool Exist = false;
        public bool IsMobile = false;
        public bool IsSolid = false;
        public bool IsVisible = false;

        public Force AppliedForces;

        public void ResetForces()
        {
            AppliedForces.X = 0;
            AppliedForces.Y = 0;
        }

        public void ApplyWingVelocity(SubpxVelocity wingVelocity)
        {
            ResetForces();

            SubpxVelocity targetAirVelocity = wingVelocity - MediumVelocity;
            int deltaAirVelocity = targetAirVelocity.X - Velocity.X;

			//if (!Grounded)
			//{
			// Air horizontal acceleration = acceleration on ice, with GroundMu = 0
			ApplyForce(new Force(
                // Should I add a vertical value to this vector, for symmetry?
                Math.Sign(deltaAirVelocity)*(Math.Abs(deltaAirVelocity) >> 5),
                0
				));
			//}

			UpdateVelocity();

        }

        public void ApplyForce(Force force)
        {
            AppliedForces.X += force.X;
            AppliedForces.Y += force.Y;
        }
        public void ApplyFluidFriction(int viscosity, SubpxVelocity velocityDifference)
        {
            ApplyForce(new Force(
                - Math.Sign(velocityDifference.X)*(((velocityDifference.X * velocityDifference.X * viscosity) >> 16)),
				- Math.Sign(velocityDifference.Y)*(((velocityDifference.Y * velocityDifference.Y * viscosity) >> 16))
				));
        }

        public void UpdateVelocity()
        {
            Velocity.X += AppliedForces.X;
            Velocity.Y += AppliedForces.Y;
		}
        public SubpxPosition TargetPosition()
        {
			if (Velocity.X > Configuration.MaxMovement.Width)
			{
				Velocity.X = Configuration.MaxMovement.Width;
			}
			if (Velocity.X < -Configuration.MaxMovement.Width)
			{
				Velocity.X = -Configuration.MaxMovement.Width;
			}

			if (Velocity.Y > Configuration.MaxMovement.Height)
			{
				Velocity.Y = Configuration.MaxMovement.Height;
			}
			if (Velocity.Y < -Configuration.MaxMovement.Height)
			{
				Velocity.Y = -Configuration.MaxMovement.Height;
			}

			return new SubpxPosition(
                (uint)(Position.X + Velocity.X),
				(uint)(Position.Y + Velocity.Y)
                );
        }





    }
}
