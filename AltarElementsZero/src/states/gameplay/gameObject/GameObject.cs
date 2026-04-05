namespace AltarElementsZero.src.states.gameplay.gameObject
{
    public struct SubpxPosition(uint x, uint y)
    {
        public uint X = x; 
        public uint Y = y;

        public readonly PxPosition ToPx()
        {
            return new PxPosition(
                X >> Configuration.Px.SubpxPower, 
                Y >> Configuration.Px.SubpxPower
				);
        }
    }
    public struct PxPosition(uint x, uint y)
    {
        public uint X = x;
        public uint Y = y;

        public readonly SubpxPosition ToSubpx()
        {
			return new SubpxPosition(
	            X << Configuration.Px.SubpxPower,
	            Y << Configuration.Px.SubpxPower
	            );
		}
        public readonly TilePosition ToTile()
        {
            return new TilePosition(
                X >> Configuration.Tile.PxPower,
                Y >> Configuration.Tile.PxPower
                );
        }
        public readonly PxPosition TileRemainder()
        {
            return new PxPosition(
                X & (uint)~-(1 << Configuration.Tile.PxPower),
				Y & (uint)~-(1 << Configuration.Tile.PxPower)
				);
        }
    }

    public struct SubpxSize(uint x, uint y)
    {
        public uint X = x;
        public uint Y = y;

		public readonly PxSize ToPx()
		{
			return new PxSize(
				X >> Configuration.Px.SubpxPower,
				Y >> Configuration.Px.SubpxPower
				);
		}
	}

    public struct PxSize(uint x, uint y)
    {
        public uint X = x;
        public uint Y = y;

        public readonly SubpxSize ToSubpx()
        {
			return new SubpxSize(
	            X << Configuration.Px.SubpxPower,
	            Y << Configuration.Px.SubpxPower
	            );
		}
    }

    public struct TilePosition(uint x, uint y)
    {
        public uint X = x;
        public uint Y = y;

        public readonly PxPosition ToPx()
        {
            return new PxPosition(
				X << Configuration.Tile.PxPower,
				Y << Configuration.Tile.PxPower
				);
        }
    }

    public struct SubpxVelocity(int x, int y)
    {
        public int X = x;
        public int Y = y;

        public static SubpxVelocity operator -(SubpxVelocity v1, SubpxVelocity v2)
        {
            return new SubpxVelocity(
                x: v1.X - v2.X,
                y: v1.Y - v2.Y
                );
        }

    }
    public struct Force(int x, int y)
    {
        public int X = x;
        public int Y = y;
    }

    abstract class GameObject
    {
        public SubpxPosition Position;
        public SubpxSize Size;
        public SubpxVelocity Velocity;
        public bool Grounded = false;
        public SubpxVelocity GroundVelocity;
        public int GroundMuKin;
        public int GroundMuSta;
        public SubpxVelocity FeetVelocity;


        public Force AppliedForces;

        public void ResetForces()
        {
            AppliedForces.X = 0;
            AppliedForces.Y = 0;
        }

        public void ApplyForce(Force force)
        {
            AppliedForces.X += force.X;
            AppliedForces.Y += force.Y;
        }
        public void ApplyFluidFriction(int viscosity, SubpxVelocity velocityDifference)
        {
            ApplyForce(new Force(
                - Math.Sign(velocityDifference.X)*(((velocityDifference.X * velocityDifference.X * viscosity) >> 16) + 1),
				- Math.Sign(velocityDifference.Y)*(((velocityDifference.Y * velocityDifference.Y * viscosity) >> 16) + 1)
				));
        }

        public void UpdateVelocity()
        {
            Velocity.X += AppliedForces.X;
            Velocity.Y += AppliedForces.Y;

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
		}
        public SubpxPosition TargetPosition()
        {
            return new SubpxPosition(
                (uint)(Position.X + Velocity.X),
				(uint)(Position.Y + Velocity.Y)
                );
        }





    }
}
