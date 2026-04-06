namespace AltarElementsZero.src.states.gameplay.vectors
{
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
}
