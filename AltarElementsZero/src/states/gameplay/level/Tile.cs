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

        // SubMember position in spritesheet
        //        X0                X1                  X2              X3
		// 0X   [none]           [right]             [left|right]    [left]
		// 1X   [below]          [right|below]       ...
		// 2X   [above|below]    [right|above|below]
		// 3X   [above]          [right|above]


		private static readonly byte[] SubMemberIndices = new byte[16]{
            //      above   below   left    right
            0x00, //
            0x30, //  o       
            0x10, //          o
            0x20, //  o       o
            0x03, //                  o
            0x33, //  o               o
            0x13, //          o       o
            0x23, //  o       o       o
            0x01, //                          o
            0x31, //  o                       o
            0x11, //          o               o
            0x21, //  o       o               o
            0x02, //                  o       o
            0x32, //  o               o       o
            0x12, //          o       o       o
            0x22, //  o       o       o       o
        };
        public static byte GetSubMember(bool connectedAbove, bool connectedBelow, bool connectedAtLeft, bool connectedAtRight)
        {
            int index = 0;
            if (connectedAbove) index |= 1;
            if (connectedBelow) index |= 2;
            if (connectedAtLeft) index |= 4;
            if (connectedAtRight) index |= 8;
            return SubMemberIndices[index];
        }

        readonly public bool IsSolid()
        {
            return (Family >= Families.Ground && Family <= Families.TurbineRight2) || 
            (Family == Families.AutomaticallyTiledGround) || 
            (Family >= Families.BlobTileset0 && Family <= Families.BlobTileset7);
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
            return (Family >= Families.Ground && Family <= Families.Spikes) || 
                (Family == Families.AutomaticallyTiledGround);
        }
        readonly public bool IsAnimatedTile()
        {
            return Family >= Families.ConveyorRight && Family <= Families.TurbineRight2;
        }
        readonly public bool IsBlobTile()
        {
            return Family >= Families.BlobTileset0 && Family <= Families.BlobTileset7;
        }
        readonly public int BlobFamilyIndex()
        {
            if (IsBlobTile())
            {
                return (int)Family - (int)Families.BlobTileset0;
            }
            else
            {
                return -1;
            }
        }

        readonly public bool IsObjectSpawn()
        {
            return (Family >= Families.Toki && Family <= Families.DebugBox)
                || (Family >= Families.FanUp  && Family <= Families.TurbineRight);
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
            
            TurbineLeft,
            TurbineRight,
            TurbineLeft2,
            TurbineRight2,

            AutomaticallyTiledGround = 0x10, // static spritesheet index, where only bits 11001100 matter (masked by 0xcc)

            BlobTileset0 = 0x20,
            BlobTileset1,
            BlobTileset2,
            BlobTileset3,
            BlobTileset4,
            BlobTileset5,
            BlobTileset6,
            BlobTileset7,



			// GameObjects spawn points



			Toki = 128,
            MovingPlatform1,

            Ufo = 128 + 16,
            BreakableTile,

            Barrel = 0xd0,

            FloorButton = 0xe0,
            SwitchableDoor,

            Ora = 0xf0,
            Checkpoint,

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

			"TRB LEFT","TRBRIGHT","TRB L 2","TRB R 2","UNASSIGN","UNASSIGN",

			"AUTO GND","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",

			"BLOBSET0","BLOBSET1","BLOBSET2","BLOBSET3","BLOBSET4","BLOBSET5","BLOBSET6","BLOBSET7",
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
			"UFO",     "BRKABLE", "UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",

			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",

			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",

			"BARREL"  ,"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",

			"BUTTON",  "SWITDOOR","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",

			"ORA",     "CHECKPNT","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN",
			"UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","UNASSIGN","IMMOBILE","PUSHER",  "DEBUGBOX"
			};
    }
}
