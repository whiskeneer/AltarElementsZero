using AltarElementsZero.src.states.gameplay.vectors;

namespace AltarElementsZero.src.renderer
{
	interface IBackground
	{
		public PxPosition AtlasPosition { get; }
		public bool IsVertical { get; }
		public uint[] Distances { get; }
	}


	class Background1 : IBackground
	{
		public static readonly Background1 Instance = new Background1();
		public PxPosition AtlasPosition { get; } = new(0,128);
		public bool IsVertical { get; } = false;
		public uint[] Distances { get; } = [2, 3, 4, 8, 8, 6, 4, 4];
	}

	class Background3 : IBackground
	{
		public static readonly Background3 Instance = new Background3();
		public PxPosition AtlasPosition { get; } = new(384, 128);
		public bool IsVertical { get; } = true;
		public uint[] Distances { get; } = [2,3,4,5,6,8,8,6,5,4,3,2];
	}

}
