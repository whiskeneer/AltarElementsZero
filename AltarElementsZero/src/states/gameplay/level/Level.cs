using System.Text.Json;

namespace AltarElementsZero.src.states.gameplay.level
{
    class Level
    {
        public readonly Tile[] tiles;
        public readonly Chunk[] chunks;

		public Level(string? tilesFileName, string? chunksFileName)
        {
            if(tilesFileName == null)
            {
                tiles = new Tile[
                    Configuration.Level.Tile.Height * Configuration.Level.Tile.Width
                    ];
            }
            else
            {
                var json = File.ReadAllText(tilesFileName);
                tiles = JsonSerializer.Deserialize<Tile[]>(json)!;
            }

            if(chunksFileName == null)
            {
				chunks = new Chunk[
	                Configuration.Level.Chunk.Height * Configuration.Level.Chunk.Width
	                ];
			}
            else
            {
				var json = File.ReadAllText(chunksFileName);
				chunks = JsonSerializer.Deserialize<Chunk[]>(json)!;
			}

		}

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

        public Chunk GetChunk(int x, int y)
        {
            if(x < 0 || y < 0 ||
                x >= Configuration.Level.Chunk.Width ||
                y >= Configuration.Level.Chunk.Height)
            {
                return new Chunk ();
            }
            return chunks[x +  y * Configuration.Level.Chunk.Width];
        }
        public void SetChunk(int x, int y, Chunk chunk)
        {
			if (x < 0 || y < 0 ||
	            x >= Configuration.Level.Chunk.Width ||
	            y >= Configuration.Level.Chunk.Height)
			{
				return;
			}
            chunks[x + y * Configuration.Level.Chunk.Width] = chunk;
		}

        public void InitChunk()
        {
            for (int y = 0; y < Configuration.Level.Chunk.Height; y++)
            {
                for (int x = 0; x < Configuration.Level.Chunk.Width; x++)
                {
                    ref Chunk chunk = ref chunks[x + y * Configuration.Level.Chunk.Width];

                    chunk.Top = (byte)y;
                    chunk.Bottom = (byte)y;
                    chunk.Left = (byte)x;
                    chunk.Right = (byte)x;

                    chunk.BackgroundIndex = (byte)0;
                    chunk.Reserved1 = (byte)0;
                    chunk.Reserved2 = (byte)0;
                    chunk.Reserved3 = (byte)0;
				}
            }
        }


        // For now, I'll be using json files to store level data.
        // Later, I'll use a more efficient way (like binary)
        //public void LoadFromFile(string tilesFileName)
        //{
        //    var json = File.ReadAllText(tilesFileName);
            
        //}

        public void SaveToFile(string? tilesFileName, string? chunksFileName) 
        {
            if(tilesFileName != null)
            {
                var json = JsonSerializer.Serialize(tiles);
                File.WriteAllText(tilesFileName, json);
            }
            
            if(chunksFileName != null)
            {
                var json = JsonSerializer.Serialize(chunks);
                File.WriteAllText(chunksFileName, json);
            }

        }
    }
}
