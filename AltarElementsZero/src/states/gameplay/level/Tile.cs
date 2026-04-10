namespace AltarElementsZero.src.states.gameplay.level
{
    struct Tile(
        Tile.Families family,
        byte member
        )
    {
        public Families Family { get; set; } = family;
        public byte Member { get; set; } = member;

        readonly public bool IsStaticTile()
        {
            return Family >= Families.Ground && Family <= Families.Spikes;
        }
        readonly public bool IsAnimatedTile()
        {
            return Family >= Families.ConveyorRight && Family <= Families.ConveyorLeft;
        }
        readonly public bool IsObjectSpawn()
        {
            return Family >= Families.Toki && Family <= Families.DebugBox;
        }

        public enum Families : byte
        {   // Family       //  Member
            None,           //  ----

            Ground,         //  static spritesheet index
            Ice,            //  static spritesheet index
            Spikes,         //  static spritesheet index

            ConveyorRight,  //  6msb animated spritesheet index | 2lsb animation & physic speed
            ConveyorLeft,   //  6msb animated spritesheet index | 2lsb animation & physic speed
                            // Spring,         //  6msb animated spritesheet index | direction

            // GameObjects spawn points
            Toki = 128,
            MovingPlatform1,

            DebugPusher = 0xfe,
            DebugBox = 0xff


        }
        public static readonly string[] FamilyDescriptors = new string[256]{
            "NONE",

            "GROUND",
            "ICE",
            "SPIKE",

            "CNVRIGHT",
            "CNVLEFT",

            "UNASSIGN",
			"UNASSIGN",

			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",

			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",

			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",

			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",

			"TOKI",    "MOVINGP1",
            
            "UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",

			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",

			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",

			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","PUSHER","DEBUGBOX"
			};
    }
}
