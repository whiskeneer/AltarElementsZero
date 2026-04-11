using AltarElementsZero.src.states.gameplay.vectors;

namespace AltarElementsZero.src.states.gameplay.gameObject
{
    struct BoundingBox(SubpxPosition position, SubpxSize size)
    {
        public SubpxPosition Position = position;
        public SubpxSize Size = size;

        public readonly SubpxPosition Center()
        {
            return new SubpxPosition(
                Position.X + (Size.X >> 1), 
                Position.Y + (Size.Y >> 1)
                );
        }

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


        public static void SeparateHorizontally(ref BoundingBox b1,ref BoundingBox b2, uint maxDiff)
        {
            SubpxPosition c1 = b1.Center();
            SubpxPosition c2 = b2.Center();

            int diff = (int)c2.X - (int)c1.X;

            if (diff > 0)
            {   // c1 at left of c2
                if (diff > maxDiff) diff = (int)maxDiff;

                uint overlapping = (uint) ((b1.Size.X >> 1) + (b1.Size.X & 1) + (b2.Size.X >> 1) - diff);

                if (overlapping > maxDiff) overlapping = maxDiff;

                //overlapping = (overlapping >> 1) + (overlapping & 1);

                uint ov1 = overlapping >> 1;
                uint ov2 = overlapping - ov1;

                b1.Position.X -= ov1;
                b2.Position.X += ov2;

            }
            else
            { // c2 at left of c1
                if (diff < -maxDiff) diff = -(int)maxDiff;

				uint overlapping = (uint)((b2.Size.X >> 1) + (b2.Size.X & 1) + (b1.Size.X >> 1) + diff);

				if (overlapping > maxDiff) overlapping = maxDiff;

				//overlapping = (overlapping >> 1) + (overlapping & 1);

				uint ov2 = overlapping >> 1;
				uint ov1 = overlapping - ov2;

                b2.Position.X -= ov2;
				b1.Position.X += ov1;
			}

        }

        public static void SeparateVertically(ref BoundingBox b1,ref BoundingBox b2, uint maxDiff)
        {
            SubpxPosition c1 = b1.Center();
            SubpxPosition c2 = b2.Center();

            int diff = (int)c2.Y - (int)c1.Y;

            if(diff > 0)
            { // c1 above c2
                if(diff > maxDiff) diff = (int)maxDiff;

				uint overlapping = (uint)((b1.Size.Y >> 1) + (b1.Size.Y & 1) + (b2.Size.Y >> 1) - diff);

				if (overlapping > maxDiff) overlapping = maxDiff;

				//overlapping = (overlapping >> 1) + (overlapping & 1);


				uint ov1 = overlapping >> 1;
				uint ov2 = overlapping - ov1;

				b1.Position.Y -= ov1;
				b2.Position.Y += ov2;
			}
            else
            { // c2 above c1
                if(diff < -maxDiff) diff = -(int)maxDiff;

				uint overlapping = (uint)((b2.Size.Y >> 1) + (b2.Size.Y & 1) + (b1.Size.Y >> 1) + diff);

				if (overlapping > maxDiff) overlapping = maxDiff;

				//overlapping = (overlapping >> 1) + (overlapping & 1);


				uint ov2 = overlapping >> 1;
				uint ov1 = overlapping - ov2;

				b2.Position.Y -= ov2;
				b1.Position.Y += ov1;
			}
        }

        public static void Separate(ref BoundingBox b1, ref BoundingBox b2)//, SubpxVelocity maxVelocity)
        {
            SubpxPosition c1 = b1.Center();
            SubpxPosition c2 = b2.Center();

            SubpxVelocity diff = c2 - c1;

            if(diff.X > 0)
            {
                if (diff.Y > 0)
                { // down right
                    uint overlappingX = (uint)((b1.Size.X >> 1) + (b1.Size.X & 1) + (b2.Size.X >> 1) - diff.X);
                    uint ov1X = overlappingX >> 1;
                    uint ov2X = overlappingX - ov1X;


                    uint overlappingY = (uint)((b1.Size.Y >> 1) + (b1.Size.Y & 1) + (b2.Size.Y >> 1) - diff.Y);
                    uint ov1Y = overlappingY >> 1;
                    uint ov2Y = overlappingY - ov1Y;

                    if(overlappingX < overlappingY)
                    {
						b1.Position.X -= ov1X;
						b2.Position.X += ov2X;
                    }
                    else
                    {
						b1.Position.Y -= ov1Y;
						b2.Position.Y += ov2Y;
					}
                    
				}
                else
                { // up right
					uint overlappingX = (uint)((b1.Size.X >> 1) + (b1.Size.X & 1) + (b2.Size.X >> 1) - diff.X);
					uint ov1X = overlappingX >> 1;
					uint ov2X = overlappingX - ov1X;

					uint overlappingY = (uint)((b2.Size.Y >> 1) + (b2.Size.Y & 1) + (b1.Size.Y >> 1) + diff.Y);
					uint ov2Y = overlappingY >> 1;
					uint ov1Y = overlappingY - ov2Y;

					if (overlappingX < overlappingY)
					{
						b1.Position.X -= ov1X;
						b2.Position.X += ov2X;
					}
					else
					{
						b2.Position.Y -= ov2Y;
						b1.Position.Y += ov1Y;
					}
				}
            }
            else
            {
				if (diff.Y > 0)
				{ // down left
					uint overlappingX = (uint)((b2.Size.X >> 1) + (b2.Size.X & 1) + (b1.Size.X >> 1) + diff.X);
					uint ov2X = overlappingX >> 1;
					uint ov1X = overlappingX - ov2X;

					uint overlappingY = (uint)((b1.Size.Y >> 1) + (b1.Size.Y & 1) + (b2.Size.Y >> 1) - diff.Y);
					uint ov1Y = overlappingY >> 1;
					uint ov2Y = overlappingY - ov1Y;

					if (overlappingX < overlappingY)
					{
						b2.Position.X -= ov2X;
						b1.Position.X += ov1X;
					}
					else
					{
						b1.Position.Y -= ov1Y;
						b2.Position.Y += ov2Y;
					}
				}
				else
				{ // up left
                    uint overlappingX = (uint)((b2.Size.X >> 1) + (b2.Size.X & 1) + (b1.Size.X >> 1) + diff.X);
                    uint ov2X = overlappingX >> 1;
                    uint ov1X = overlappingX - ov2X;

                    uint overlappingY = (uint)((b2.Size.Y >> 1) + (b2.Size.Y & 1) + (b1.Size.Y >> 1) + diff.Y);
                    uint ov2Y = overlappingY >> 1;
                    uint ov1Y = overlappingY - ov2Y;


					if (overlappingX < overlappingY)
					{
						b2.Position.X -= ov2X;
						b1.Position.X += ov1X;
					}
					else
					{
						b2.Position.Y -= ov2Y;
						b1.Position.Y += ov1Y;
					}
				}
			}
            
        }

	}
}
