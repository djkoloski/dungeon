using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dungeon
{
    public class Tile
    {
        //Brutish way to specify special directional properties of a tile.
        public static readonly String DIR_KEY = "DIR_KEY";

        public object component;
        public bool isPartOfRoom;
        public string roomType;
        public Dictionary<object, object> roomInfo = new Dictionary<object, object>();

        public Tile(object component_, bool isPartOfRoom_)
        {
            component = component_;
            isPartOfRoom = isPartOfRoom_;
        }
    }
}
