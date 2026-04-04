namespace AltarElementsZero.src.states.gameplay.level
{
    class Level
    {
        public readonly Tile[] tiles = new Tile[
            Configuration.Level.Tile.Height * Configuration.Level.Tile.Width 
            ];
        public Tile GetTile(int x, int y)
        {
            // is there a more efficient way of checking this?
            if( x < 0 || y < 0 ||
                x >= Configuration.Level.Tile.Width ||
                y >= Configuration.Level.Tile.Height)
            {
                return new Tile(Tile.Families.None, 0);
            }
            return tiles[x + y * Configuration.Level.Tile.Width];
        }
        public void SetTile(int x, int y, Tile tile)
        {
			if (x < 0 || y < 0 ||
				x >= Configuration.Level.Tile.Width ||
				y >= Configuration.Level.Tile.Height)
			{
				return;
			}
			tiles[x + y * Configuration.Level.Tile.Width] = tile;
		}
        public void SetAll(Tile tile)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = tile;
            }
        }
    }
}
