namespace AltarElementsZero.src.states.gameplay.level
{
    struct Tile(
        Tile.Families family,
        byte member
        )
    {
        public Families Family = family; 
        public byte Member = member;

        public enum Families : byte
        {
            None,
            Terrain,
        }
    }
}
