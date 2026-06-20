using AltarElementsZero.src.states.gameplay.vectors;
using Microsoft.Xna.Framework.Graphics;

namespace AltarElementsZero.src.renderer
{
	public struct AtlasReference()
	{
		public PxPosition Start; // top-left coordinates at atlas
		public PxSize Size;		 // sprite size
		public PxPosition Offset;// offset from object logic top-left
		public SpriteEffects Effects;
	}
}
