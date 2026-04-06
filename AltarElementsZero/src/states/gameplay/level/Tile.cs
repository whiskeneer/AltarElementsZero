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
        {   // Family       //  Member
            None,           //  ----

            Ground,         //  static spritesheet index
            Ice,            //  static spritesheet index
            Spikes,         //  static spritesheet index

            ConveyorRight,  //  6msb animated spritesheet index | 2lsb animation & physic speed
            ConveyorLeft,   //  6msb animated spritesheet index | 2lsb animation & physic speed
			// Spring,         //  6msb animated spritesheet index | direction

		}
    }
}
