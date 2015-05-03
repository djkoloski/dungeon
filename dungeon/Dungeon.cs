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
        public static Random RAND = new Random(1);
        // The tiles in the dungeon
        public Dictionary<IVector3, Tile> tiles;

        public Dungeon()
        {
            tiles = new Dictionary<IVector3, Tile>();
        }

        public Tile Get(IVector3 v)
        {
            if (tiles.ContainsKey(v))
                return tiles[v];
            return null;
        }
        public Tile ForceGet(IVector3 v, object digObject = null)
        {
            if (tiles.ContainsKey(v)) return tiles[v];
            Tile tile = new Tile(digObject);
            tiles[v] = tile;
            return tile;
        }
    }
}
