using AltarElementsZero.src.states.gameplay.gameObject;

namespace AltarElementsZero.src.states.gameplay.vectors
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

		public static SubpxPosition operator +(SubpxPosition left, SubpxSize right)
		{
			return new(
				left.X + right.X,
				left.Y + right.Y
				);
		}
		public static SubpxPosition operator -(SubpxPosition left, SubpxSize right)
		{
			return new(
				left.X - right.X,
				left.Y - right.Y
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

		public static PxPosition operator -(PxPosition left, PxSize right)
		{
			return new(
				left.X - right.X,
				left.Y - right.Y
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
}
