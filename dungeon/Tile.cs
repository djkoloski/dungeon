using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dungeon
{
    public class Tile
    {
        public object component;
        public bool partOfRoom;

        public Tile(object component_, bool partOfRoom_)
        {
            component = component_;
            partOfRoom = partOfRoom_;
        }
    }
}
