using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pb.Collections;
using dungeon.Generator;

namespace dungeon
{
    public class Dungeon
    {
        public static Random RAND = new Random(2);
        // The tiles in the dungeon
        private Dictionary<IVector3, Tile> tiles;

        public HashSet<IVector3> removedTiles = new HashSet<IVector3>();
        public HashSet<IVector3> addedTiles = new HashSet<IVector3>();

        public Dungeon()
        {
            tiles = new Dictionary<IVector3, Tile>();
        }

        public Dictionary<IVector3, Tile> getTilesSeparately()
        {
            return tiles;
        }

        public Tile getTile(IVector3 loc)
        {
            return tiles[loc];
        }

        public bool hasTile(IVector3 loc)
        {
            return tiles.ContainsKey(loc);
        }

        public void AddTile(IVector3 loc, Tile tile)
        {
            tiles[loc] = tile;
            addedTiles.Add(loc);
            removedTiles.Remove(loc);
        }

        public void RemoveTile(IVector3 loc)
        {
            tiles.Remove(loc);
            removedTiles.Add(loc);
            addedTiles.Remove(loc);
        }
    }
}
