using AltarElementsZero.src.states.gameplay.vectors;

namespace AltarElementsZero.src.states.gameplay.gameObject
{
    struct BoundingBox(SubpxPosition position, SubpxSize size)
    {
        public SubpxPosition Position = position;
        public SubpxSize Size = size;

        public static BoundingBox operator +(BoundingBox left, SubpxVelocity right)
        {
            return new(left.Position + right, left.Size);
        }
        public static bool operator &(BoundingBox b1, BoundingBox b2)
        {
            if(b1.Size.X == 0 || b1.Size.Y == 0 || b2.Size.X == 0 || b2.Size.Y == 0)
            {
                return false;
            }

            uint left1 = b1.Position.X;
            uint up1 = b1.Position.Y;
            uint right1 = left1 + b1.Size.X - 1;
            uint down1 = up1 + b1.Size.Y - 1;

            uint left2 = b2.Position.X;
            uint up2 = b2.Position.Y;
            uint right2 = left2 + b2.Size.X - 1;
            uint down2 = up2 + b2.Size.Y - 1;

            return (
                left1 <= right2 &&
                left2 <= right1 &&
                up1 <= down2 &&
                up2 <= down1
                );
        }

        public void LeanAbove(BoundingBox other)
        {
            Position.Y = other.Position.Y - Size.Y;
            //Console.WriteLine("LEAN ABOVE");
        }
        public void LeanBelow(BoundingBox other)
        {
            Position.Y = other.Position.Y + other.Size.Y;
            //Console.WriteLine("LEAN BELOW");
		}
		public void LeanAtLeft(BoundingBox other)
        {
            Position.X = other.Position.X - Size.X;
            //Console.WriteLine("LEAN AT LEFT");
		}
		public void LeanAtRight(BoundingBox other)
        {
            Position.X = other.Position.X + other.Size.X;
            //Console.WriteLine("LEAN AT RIGHT");
		}
	}
}
