namespace AltarElementsZero.src.states.gameplay.vectors
{
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
}
