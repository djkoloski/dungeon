using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pb.Collections;

namespace dungeon
{
    public class Dungeon
    {
        // The tiles in the dungeon
        public Dictionary<IVector3, Tile> tiles;
        // List of joints by component:
        //   Component 0 has all joints in joints[0]
        public List<HashSet<IVector3>> joints = new List<HashSet<IVector3>>();
        public List<HashSet<IVector3>> closedJoints = new List<HashSet<IVector3>>();

        public Dungeon()
        {
            tiles = new Dictionary<IVector3, Tile>();
        }

        public IVector3 getRandomJoint(string cur)
        {
            //This should get a random open joint for the given room's component, or return a closed one if no open ones could be found
            throw new NotImplementedException();
        }

        public Tile Get(IVector3 v)
        {
            if (tiles.ContainsKey(v))
                return tiles[v];
            return null;
        }
        public Tile ForceGet(IVector3 v)
        {
            if (tiles.ContainsKey(v))
                return tiles[v];
            Tile tile = new Tile();
            tiles[v] = tile;
            return tile;
        }
        public int AddComponent()
        {
            joints.Add(new HashSet<IVector3>());
            closedJoints.Add(new HashSet<IVector3>());
            return joints.Count - 1;
        }
        public void AddJoint(int component, IVector3 pos)
        {
            joints[component].Add(pos);
        }
        public void RemoveJoint(int component, IVector3 pos)
        {
            joints[component].Remove(pos);
            closedJoints[component].Add(pos);
        }
    }
}
