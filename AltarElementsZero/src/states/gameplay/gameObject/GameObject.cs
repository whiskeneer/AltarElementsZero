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

    public struct TilePosition(uint x, uint y)
    {
        public uint X = x;
        public uint Y = y;
    }

    abstract class GameObject
    {
        public SubpxPosition Position;
    }
}
