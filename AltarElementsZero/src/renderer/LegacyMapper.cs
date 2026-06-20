using AltarElementsZero.src.states.gameplay.vectors;

namespace AltarElementsZero.src.renderer
{
	// The purpose of this class is to translate the old object spritesheet index
	// into the current atlasReference
	static class LegacyMapper
	{
		public static PxPosition StartFromObjectSpritesheetIndex(uint i)
		{
			return new(
				(uint)(32 * (i & 0x7) + 256),
				(uint)(32 * (i >> 3) + 768)
			);
		}
		public static PxPosition StartFromOraSpritesheetIndex(uint i)
		{
			return new(
			  (uint)(32 * (i & 0x7)),
			  (uint)(32 * (i >> 3) + 768)
		  );
		}
	}
}
