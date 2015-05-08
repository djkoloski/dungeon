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
        public static Random RAND = new Random();
        // The tiles in the dungeon
        public Dictionary<IVector3, Tile> tiles;

		public Dungeon()
		{
			tiles = new Dictionary<IVector3,Tile>();
		}
    }
}
