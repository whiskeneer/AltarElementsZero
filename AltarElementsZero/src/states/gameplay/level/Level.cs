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

        public void ReplaceAutomaticallyTiled()
        {
            for (int row = 0; row < Configuration.Level.Tile.Height; row++)
            {
                for (int col = 0; col < Configuration.Level.Tile.Width; col++)
                {
                    Tile tile = GetTile(col, row);

                    if(tile.Family == Tile.Families.AutomaticallyTiledGround)
                    {
                        byte metaMember = (byte)(tile.Member & 0xcc);
                        bool foundAbove = false;
                        bool foundBelow = false;
                        bool foundAtLeft = false;
                        bool foundAtRight = false;
                        
                        Tile tileAbove = GetTile(col, row - 1);
                        Tile tileBelow = GetTile(col, row + 1);
                        Tile tileAtLeft = GetTile(col - 1, row);
                        Tile tileAtRight = GetTile(col + 1, row);

                        if(tileAbove.Family == Tile.Families.AutomaticallyTiledGround)
                        {
                            foundAbove = (metaMember == (tileAbove.Member & 0xcc));  
                        }
						if (tileBelow.Family == Tile.Families.AutomaticallyTiledGround)
						{
							foundBelow = (metaMember == (tileBelow.Member & 0xcc));
						}
						if (tileAtLeft.Family == Tile.Families.AutomaticallyTiledGround)
						{
							foundAtLeft = (metaMember == (tileAtLeft.Member & 0xcc));
						}
						if (tileAtRight.Family == Tile.Families.AutomaticallyTiledGround)
						{
							foundAtRight = (metaMember == (tileAtRight.Member & 0xcc));
						}

                        byte subMember = Tile.GetSubMember(foundAbove, foundBelow, foundAtLeft, foundAtRight);
                        
                        tile.Member = (byte)(metaMember | subMember);

                        SetTile(col, row, tile);
					}
                }
            }
            for (int row = 0; row < Configuration.Level.Tile.Height; row++)
            {
                for (int col = 0; col < Configuration.Level.Tile.Width; col++)
                {
                    Tile tile = GetTile(col, row);

                    if (tile.Family == Tile.Families.AutomaticallyTiledGround)
                    {
                        tile.Family = Tile.Families.Ground;
						SetTile(col, row, tile);
					}
                }
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
        public void UpdateTilesAround(int x, int y)
        {
            for (int i = y - 1; i <= y + 1; i++)
            {
				for (int j = x - 1; j <= x + 1; j++)
                {
                    UpdateTile(j, i);
                }
			}
		}

        public static readonly byte[] BlobIndexToRender = new byte[256]{

        //          R     U    U+R   L    L+R   L+U  L+U+R   D    D+R   D+U   D+U+R  D+L  D+L+R D+L+U D+L+U+R
			0x06, 0x01, 0x31, 0x28, 0x0e, 0x35, 0x36, 0x34, 0x08, 0x09, 0x2e, 0x0d, 0x05, 0x29, 0x26, 0x12, // 
			0x06, 0x01, 0x31, 0x32, 0x0e, 0x35, 0x36, 0x13, 0x08, 0x09, 0x2e, 0x20, 0x05, 0x29, 0x26, 0x0a, //          UR
			0x06, 0x01, 0x31, 0x28, 0x0e, 0x35, 0x2d, 0x33, 0x08, 0x09, 0x2e, 0x0d, 0x05, 0x29, 0x1e, 0x22, //       LU
			0x06, 0x01, 0x31, 0x32, 0x0e, 0x35, 0x2d, 0x21, 0x08, 0x09, 0x2e, 0x20, 0x05, 0x29, 0x1e, 0x2c, //       LU+UR
			0x06, 0x01, 0x31, 0x28, 0x0e, 0x35, 0x36, 0x34, 0x08, 0x09, 0x2e, 0x0d, 0x16, 0x04, 0x1a, 0x11, //    DL
			0x06, 0x01, 0x31, 0x32, 0x0e, 0x35, 0x36, 0x13, 0x08, 0x09, 0x2e, 0x20, 0x16, 0x04, 0x1a, 0x2b, //    DL+   UR
			0x06, 0x01, 0x31, 0x28, 0x0e, 0x35, 0x2d, 0x33, 0x08, 0x09, 0x2e, 0x0d, 0x16, 0x04, 0x0c, 0x25, //    DL+LU
			0x06, 0x01, 0x31, 0x32, 0x0e, 0x35, 0x2d, 0x21, 0x08, 0x09, 0x2e, 0x20, 0x16, 0x04, 0x0c, 0x1d, //    DL+LU+UR
			0x06, 0x01, 0x31, 0x28, 0x0e, 0x35, 0x36, 0x34, 0x08, 0x1b, 0x2e, 0x10, 0x05, 0x02, 0x26, 0x2a, // DR
			0x06, 0x01, 0x31, 0x32, 0x0e, 0x35, 0x36, 0x13, 0x08, 0x1b, 0x2e, 0x18, 0x05, 0x02, 0x26, 0x23, // DR+      UR 
			0x06, 0x01, 0x31, 0x28, 0x0e, 0x35, 0x2d, 0x33, 0x08, 0x1b, 0x2e, 0x10, 0x05, 0x02, 0x1e, 0x14, // DR+   LU
			0x06, 0x01, 0x31, 0x32, 0x0e, 0x35, 0x2d, 0x21, 0x08, 0x1b, 0x2e, 0x18, 0x05, 0x02, 0x1e, 0x0b, // DR+   LU+UR
			0x06, 0x01, 0x31, 0x28, 0x0e, 0x35, 0x36, 0x34, 0x08, 0x1b, 0x2e, 0x10, 0x16, 0x03, 0x1a, 0x15, // DR+DL
			0x06, 0x01, 0x31, 0x32, 0x0e, 0x35, 0x36, 0x13, 0x08, 0x1b, 0x2e, 0x18, 0x16, 0x03, 0x1a, 0x1c, // DR+DL+   UR
			0x06, 0x01, 0x31, 0x28, 0x0e, 0x35, 0x2d, 0x33, 0x08, 0x1b, 0x2e, 0x10, 0x16, 0x03, 0x0c, 0x19, // DR+DL+LU
			0x06, 0x01, 0x31, 0x32, 0x0e, 0x35, 0x2d, 0x21, 0x08, 0x1b, 0x2e, 0x18, 0x16, 0x03, 0x0c, 0x24  // DR+DL+LU+UR
		};
        public void UpdateTile(int x, int y)
        {
			if (x < 0 || y < 0 ||
		        x >= Configuration.Level.Tile.Width ||
		        y >= Configuration.Level.Tile.Height)
			{
				return;
			}
            Tile tile = GetTile(x, y);
            if (!tile.IsBlobTile()) return;
            byte member = tile.Member;
            if ((member & 0x7) == 0x7 || (member & 0x38) == 0x38) return;
            
            int blobFamilyIndex = tile.BlobFamilyIndex();

            // match => bits        7 |6 |5 |4 |3|2|1|0
            byte match = 0; //      DR|DL|LU|UR|D|L|U|R

            Tile other; 
            other = GetTile(x + 1, y);
            if (other.IsBlobTile() && other.BlobFamilyIndex() == blobFamilyIndex) match |= 0x1;
			other = GetTile(x, y - 1);
			if (other.IsBlobTile() && other.BlobFamilyIndex() == blobFamilyIndex) match |= 0x2;
			other = GetTile(x - 1, y);
			if (other.IsBlobTile() && other.BlobFamilyIndex() == blobFamilyIndex) match |= 0x4;
			other = GetTile(x, y + 1);
			if (other.IsBlobTile() && other.BlobFamilyIndex() == blobFamilyIndex) match |= 0x8;
			other = GetTile(x + 1, y - 1);
			if (other.IsBlobTile() && other.BlobFamilyIndex() == blobFamilyIndex) match |= 0x10;
			other = GetTile(x - 1, y - 1);
			if (other.IsBlobTile() && other.BlobFamilyIndex() == blobFamilyIndex) match |= 0x20;
			other = GetTile(x - 1, y + 1);
			if (other.IsBlobTile() && other.BlobFamilyIndex() == blobFamilyIndex) match |= 0x40;
			other = GetTile(x + 1, y + 1);
			if (other.IsBlobTile() && other.BlobFamilyIndex() == blobFamilyIndex) match |= 0x80;

            tile.Member = BlobIndexToRender[match];
            SetTile(x, y, tile);
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
