using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dungeon
{
	public class Tile
	{
		public bool open;
		public int component;

		public Tile()
		{
			open = false;
			component = -1;
		}
	}
}
