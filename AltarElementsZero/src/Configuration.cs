using System.Drawing;

namespace AltarElementsZero.src
{
    public static class Configuration
    {
        // Game logic

        public static class Px
        {
            public static readonly int SubpxPower = 6;
            public static readonly Dimensions Subpx = new(1 << SubpxPower, 1 << SubpxPower);
        }
        public static class Tile
        {
            public static readonly int PxPower = 4;
            public static readonly Dimensions Px = new(1 << PxPower, 1 << PxPower);
            public static readonly Dimensions Subpx = new(
                Px.Width * Configuration.Px.Subpx.Width,
                Px.Height * Configuration.Px.Subpx.Height
                );
        }
        public static class Chunk
        {
            public static readonly Dimensions Tile = new(12, 8);
            public static readonly Dimensions Px = new(
                Tile.Width * Configuration.Tile.Px.Width,
                Tile.Height * Configuration.Tile.Px.Height
                );
			public static readonly Dimensions Subpx = new(
				Px.Width * Configuration.Px.Subpx.Width,
				Px.Height * Configuration.Px.Subpx.Height
				);
		}
        public static class Level
        {
            public static readonly Dimensions Chunk = new(64, 64);
            public static readonly Dimensions Tile = new(
                Chunk.Width * Configuration.Chunk.Tile.Width,
                Chunk.Height * Configuration.Chunk.Tile.Height
                );
			public static readonly Dimensions Px = new(
	            Tile.Width * Configuration.Tile.Px.Width,
	            Tile.Height * Configuration.Tile.Px.Height
	            );
			public static readonly Dimensions Subpx = new(
				Px.Width * Configuration.Px.Subpx.Width,
				Px.Height * Configuration.Px.Subpx.Height
				);
		}

        // Game display
        public static class VisibleScreen
        {
            public static readonly Dimensions Px = new(
                12 * Tile.Px.Width,
                8 * Tile.Px.Height
                );
            public static readonly int Scale = 4;
            public static readonly bool IsFullScreen = false;
            public static readonly bool SynchronizeWithVerticalRetrace = true;
		}
    }
}
