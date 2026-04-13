using AltarElementsZero.src.states.gameplay.gameObject;
using static AltarElementsZero.src.Configuration;

namespace AltarElementsZero.src.states.gameplay.level
{
    struct Tile(
        Tile.Families family,
        byte member
        )
    {
        public Families Family { get; set; } = family;
        public byte Member { get; set; } = member;

        public struct FrictionCoefficients(int staticMu, int kinematicMu)
        {
            public int StaticMu = staticMu;
            public int KinematicMu = kinematicMu;
        }

        readonly public bool IsSolid()
        {
            return Family >= Families.Ground && Family <= Families.FanRight;
        }
        readonly public FrictionCoefficients GetFrictionCoefficients()
        {
            if (IsSolid())
            {
                if( Family == Families.Ice)
                {
                    return new FrictionCoefficients(0, 0);
                }
                else
                {
                    return new FrictionCoefficients(400, 200);
                }
            }
            else
            {
                return new FrictionCoefficients(0, 0);
            }
        }

        readonly public int GetSurfaceVelocityAbove()
        {
            if (Family == Families.ConveyorLeft)
            {
				return -(64 << (Member & 0x3));
			}
            else if (Family == Families.ConveyorRight)
            {
                return 64 << (Member & 0x3);
			}
            else
            {
                return 0;
            }
        }

        readonly public bool IsStaticTile()
        {
            return Family >= Families.Ground && Family <= Families.Spikes;
        }
        readonly public bool IsAnimatedTile()
        {
            return Family >= Families.ConveyorRight && Family <= Families.FanRight;
        }
        readonly public bool IsObjectSpawn()
        {
            return (Family >= Families.Toki && Family <= Families.DebugBox)
                || (Family >= Families.FanUp  && Family <= Families.FanRight);
                ;
        }

        public enum Families : byte
        {   // Family       //  Member
            None,           //  ----

            Ground,         //  static spritesheet index
            Ice,            //  static spritesheet index
            Spikes,         //  static spritesheet index

            ConveyorRight,  //  6msb animated spritesheet index | 2lsb animation & physic speed
            ConveyorLeft,   //  6msb animated spritesheet index | 2lsb animation & physic speed

            FanUp,
            FanDown,
            FanLeft,
            FanRight,
            

                            // Spring,         //  6msb animated spritesheet index | direction

            // GameObjects spawn points
            Toki = 128,
            MovingPlatform1,

            DebugImmobile = 0xfd,
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

            "FAN UP",
			"FAN DOWN",
			
            "FAN LEFT",
            "FANRIGHT",

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
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","IMMOBILE","PUSHER",  "DEBUGBOX"
			};
    }
}
