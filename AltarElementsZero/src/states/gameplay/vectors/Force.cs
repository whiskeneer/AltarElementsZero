namespace AltarElementsZero.src.states.gameplay.vectors
{
	public struct Force(int x, int y)
	{
		public int X = x;
		public int Y = y;
		public static Force operator +(Force a, Force b) {
				return new(
					a.X + b.X,
					a.Y + b.Y
					);
		}
	

	}

}
