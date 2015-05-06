using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Pb.Collections;
using dungeon.Util;

namespace dungeon.Generator
{
    public class DungeonFactory
    {
        //The minimum space around each hallway or room from anything else.
        private static readonly int MIN_SPACING = 2;
        //The minimum length of a hallway before making a room
        private static readonly int MIN_HALLWAY_LENGTH = 10;
        //Allow hallways from the same room to merge? (if true will break MIN_HALLWAY_LENGTH)
        private static bool ALLOW_HALLWAY_MERGING = false;

        private static int SAME_DIR_WEIGHT = 10;//Chances for a hallway to keep its direction
        private static int DIFF_DIR_WEIGHT = 1;//Chances to change direction
        DungeonTree tree;

        public Dungeon dungeon;
        Dictionary<DungeonTreeNode, List<Joint>> joints;

        Queue<DungeonTreeEdge> edgesToDig;//List of edges to dig out
        DungeonTreeEdge currentEdge;//The edge that's currently being dug out

        Joint currentJoint;//The joint that is currently being dug from

        public DungeonFactory(DungeonTree tree_)
        {
            tree = tree_;
        }

        public void BeginGeneration()
        {
            dungeon = new Dungeon();
            joints = new Dictionary<DungeonTreeNode, List<Joint>>();
            edgesToDig = new Queue<DungeonTreeEdge>();
            foreach (DungeonTreeNode obj in tree.GetNodes())
            {
                joints[obj] = new List<Joint>();
            }
            DigRoomIfPossible(tree.root_, new Joint(IVector3.zero, 0, 0));
            foreach (DungeonTreeEdge edge in tree.GetEdges())
            {
                edgesToDig.Enqueue(edge);//lol screw efficiency
            }
        }

        public bool Step()
        {
            //If an edge just finished, then find a new one and prepare to start digging it.
            if (currentEdge == null)
            {
                currentJoint = null;
                while (currentJoint == null)
                {
                    if (!edgesToDig.Any())
                        return false;
                    currentEdge = edgesToDig.Dequeue();
                    currentJoint = getAvailableJoint(currentEdge);
                }
            }
            //If our current joint is ready to plant the room, try to plant
            if (currentJoint.distanceFromSource > MIN_HALLWAY_LENGTH)
                if (DigRoomIfPossible(currentEdge.to, currentJoint))
                {
                    currentEdge = null;
                    return true;
                }
            //Otherwise, dig a hallway forward if possible. If not, then find a new joint.
            while ((currentJoint = DigHallwayFrom(currentJoint, currentEdge)) == null)
            {
                currentJoint = getAvailableJoint(currentEdge);
                if (currentJoint == null)
                {
                    System.Console.WriteLine("Failed to make " + currentEdge);
                    currentEdge = null;
                    break;
                }
            }
            return true;
        }

        private bool TrimSearchTree(DungeonTreeEdge edge, IVector3 end)
        {
            int neighbors = 0;
            for (int i = 0; i < 6; i++)
            {
                IVector3 neighbor = end + Direction.Vector[i];
                if (dungeon.tiles.ContainsKey(neighbor))
                    neighbors++;
            }
            if (neighbors < 2)
            {
                dungeon.tiles.Remove(end);
                for (int i = 0; i < 6; i++)
                {
                    joints[edge.from].Remove(new Joint(end, i, -1));
                    IVector3 neighbor = end + Direction.Vector[i];
                    if (dungeon.tiles.ContainsKey(neighbor) && dungeon.tiles[neighbor].partOfRoom == false)
                        TrimSearchTree(edge, neighbor);
                }
            }
            return false;
        }

        private bool DigRoomIfPossible(DungeonTreeNode room, Joint joint)
        {
            if (room.type.StartsWith("cube:"))
            {
                int size = int.Parse(room.type.Substring(room.type.IndexOf(":") + 1));
                return PlaceBlockIfPossible(room, joint, new IVector3(size, size, size));
            }
            return PlaceBlockIfPossible(room, joint, new IVector3(4, 4, 4));
        }

        /// <summary>
        /// Attempts to place the given block, assuming x is parallel to the joint direction, y is the tangent direction, and z is the bitangent direction.
        /// </summary>
        /// <param name="manifest"></param>
        /// <param name="joint"></param>
        /// <param name="blockSize"></param>
        private bool PlaceBlockIfPossible(DungeonTreeNode room, Joint joint, IVector3 blockSize)
        {
            IVector3.IntervalEnumerable bufferedRange = IVector3.Range(blockSize + IVector3.one * MIN_SPACING * 2);
            IVector3 dir = Direction.Vector[joint.direction];
            IVector3 tan = Direction.Tangent[joint.direction];
            IVector3 btn = Direction.Bitangent[joint.direction];
            //Check if placement is legal
            foreach (IVector3 index in bufferedRange)
                if (dungeon.tiles.ContainsKey(joint.GetExitLocation() + dir * index.x + tan * index.y + btn * index.z))
                    return false;
            //Add in the leading hallways
            if (room.parent != null)
                for (int i = 0; i < MIN_SPACING; i++)
                {
                    dungeon.tiles[joint.GetExitLocation()] = new Tile(room.parent, false);
                    joint = new Joint(joint.GetExitLocation(), joint.direction, joint.distanceFromSource + 1);
                }
            IVector3.IntervalEnumerable range = IVector3.Range(blockSize);
            //Place the block, add doorways if needed.
            foreach (IVector3 index in range)
            {
                IVector3 loc = joint.GetExitLocation() + dir * index.x + tan * index.y + btn * index.z;
                dungeon.tiles[loc] = new Tile(room, true);
            }
            for (int i = 0; i < blockSize.x; i++)
            {
                for (int j = 0; j < blockSize.y; j++)
                {
                    AddJointIfPossible(room, new Joint(joint.GetExitLocation() + dir * i + tan * j, Direction.GetDirection(IVector3.zero - btn), 0));
                    AddJointIfPossible(room, new Joint(joint.GetExitLocation() + dir * i + tan * j + btn * (blockSize.z - 1), Direction.GetDirection(btn), 0));
                }
                for (int j = 0; j < blockSize.z; j++)
                {
                    AddJointIfPossible(room, new Joint(joint.GetExitLocation() + dir * i + btn * j, Direction.GetDirection(IVector3.zero - tan), 0));
                    AddJointIfPossible(room, new Joint(joint.GetExitLocation() + dir * i + tan * (blockSize.y - 1) + btn * j, Direction.GetDirection(tan), 0));
                }
            }
            for (int i = 0; i < blockSize.y; i++)
            {
                for (int j = 0; j < blockSize.z; j++)
                {
                    AddJointIfPossible(room, new Joint(joint.GetExitLocation() + tan * i + btn * j, Direction.GetDirection(IVector3.zero - dir), 0));
                    AddJointIfPossible(room, new Joint(joint.GetExitLocation() + dir * (blockSize.x - 1) + tan * i + btn * j, Direction.GetDirection(dir), 0));
                }
            }
            return true;
        }

        private bool AddJointIfPossible(DungeonTreeNode room, Joint joint)
        {
            if (joint.direction % 3 != 1 && CanDig(dungeon.tiles, joint, room))
            {
                joints[room].Add(joint);
                return true;
            }
            return false;
        }

        private Joint DigHallwayFrom(Joint joint, DungeonTreeEdge edge)
        {
            joints[edge.from].Remove(joint);
            if (!CanDig(dungeon.tiles, joint, edge.from))
            {
                TrimSearchTree(edge, joint.location);
                return null;
            }
            dungeon.tiles[joint.location + Direction.Vector[joint.direction]] = new Tile(edge, false);
            WeightedRandomList<Joint> newJoints = new WeightedRandomList<Joint>();
            //Add the immediate exit spot, then (FOR NOW) add exit joints to all its neighbors
            for (int i = 0; i < 6; i++)
            {
                Joint newJoint = new Joint(joint.GetExitLocation(), i, joint.distanceFromSource + 1);
                bool tryAddingJoint = i % 3 != 1 || joint.direction % 3 != 1;
                if (AddJointIfPossible(edge.from, newJoint))
                    if (i == joint.direction && i % 3 != 1)
                        newJoints.Add(newJoint, SAME_DIR_WEIGHT);
                    else
                        newJoints.Add(newJoint, DIFF_DIR_WEIGHT);
            }
            if (newJoints.Any())
                return newJoints.Get();
            TrimSearchTree(edge, joint.GetExitLocation());
            return null;
        }

        private Joint getAvailableJoint(DungeonTreeEdge edge)
        {
            if (joints[edge.from].Any())
            {
                int index = Dungeon.RAND.Next() % joints[edge.from].Count();
                Joint ret = joints[edge.from][index];
                joints[edge.from].RemoveAt(index);
                return ret;
            }
            return null;
        }

        private bool CanDig(Dictionary<IVector3, Tile> tiles, Joint joint, DungeonTreeNode room)
        {
            if (tiles.ContainsKey(joint.GetExitLocation() + Direction.Vector[joint.direction]))
                if (!ALLOW_HALLWAY_MERGING)
                    return false;
                else if (tiles[joint.GetExitLocation() + Direction.Vector[joint.direction]].component != room)
                    return true;

            for (int i = -MIN_SPACING; i <= MIN_SPACING; i++)
                for (int j = -MIN_SPACING; j <= MIN_SPACING; j++)
                    if (tiles.ContainsKey(joint.GetExitLocation() + Direction.Tangent[joint.direction] * i + Direction.Bitangent[joint.direction] * j))
                        return false;
            return true;
        }
    }
}
