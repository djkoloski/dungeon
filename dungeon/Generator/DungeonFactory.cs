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
        private static readonly int MIN_HALLWAY_LENGTH = 50;
        //Allow hallways from the same room to merge? (if true will break MIN_HALLWAY_LENGTH)
        private static bool ALLOW_HALLWAY_MERGING = false;
        DungeonTree tree;

        public DungeonFactory(DungeonTree tree_)
        {
            tree = tree_;
        }

        public Dungeon Generate()
        {
            DungeonManifest manifest = new DungeonManifest(tree);

            if (tree.Begin().Done)
                return manifest.dungeon;//Quit on empty trees

            DigRoomIfPossible(manifest, tree.root_, new Joint(IVector3.zero, 0, 0));

            foreach (DungeonTreeEdge edge in tree.GetEdges())
            {
                DigEdge(manifest, edge);
            }

            return manifest.dungeon;
        }


        private bool DigRoomIfPossible(DungeonManifest manifest, DungeonTreeNode room, Joint joint)
        {
            if (room.type.StartsWith("cube:"))
            {
                int size = int.Parse(room.type.Substring(room.type.IndexOf(":") + 1));
                return PlaceBlockIfPossible(manifest, room, joint, new IVector3(size, size, size));
            }
            return PlaceBlockIfPossible(manifest, room, joint, new IVector3(4, 4, 4));
        }

        /// <summary>
        /// Attempts to place the given block, assuming x is parallel to the joint direction, y is the tangent direction, and z is the bitangent direction.
        /// </summary>
        /// <param name="manifest"></param>
        /// <param name="joint"></param>
        /// <param name="blockSize"></param>
        private bool PlaceBlockIfPossible(DungeonManifest manifest, DungeonTreeNode room, Joint joint, IVector3 blockSize)
        {
            IVector3.IntervalEnumerable bufferedRange = IVector3.Range(blockSize + IVector3.one * MIN_SPACING * 2);
            IVector3 dir = Direction.Vector[joint.direction];
            IVector3 tan = Direction.Tangent[joint.direction];
            IVector3 btn = Direction.Bitangent[joint.direction];
            //Check if placement is legal
            foreach (IVector3 index in bufferedRange)
                if (manifest.dungeon.tiles.ContainsKey(joint.GetExitLocation() + dir * index.x + tan * index.y + btn * index.z))
                    return false;
            //Add in the leading hallways
            if (room.parent != null)
                for (int i = 0; i < MIN_SPACING; i++)
                {
                    manifest.dungeon.tiles[joint.GetExitLocation()] = new Tile(room.parent);
                    joint = new Joint(joint.GetExitLocation(), joint.direction, joint.distanceFromSource + 1);
                }
            IVector3.IntervalEnumerable range = IVector3.Range(blockSize);
            //Place the block, add doorways if needed.
            foreach (IVector3 index in range)
            {
                IVector3 loc = joint.GetExitLocation() + dir * index.x + tan * index.y + btn * index.z;
                manifest.dungeon.tiles[loc] = new Tile(room);
            }
            for (int i = 0; i < blockSize.x; i++)
            {
                for (int j = 0; j < blockSize.y; j++)
                {
                    AddJointIfPossible(manifest, room, new Joint(joint.GetExitLocation() + dir * i + tan * j, Direction.GetDirection(IVector3.zero - btn), 0));
                    AddJointIfPossible(manifest, room, new Joint(joint.GetExitLocation() + dir * i + tan * j + btn * (blockSize.z - 1), Direction.GetDirection(btn), 0));
                }
                for (int j = 0; j < blockSize.z; j++)
                {
                    AddJointIfPossible(manifest, room, new Joint(joint.GetExitLocation() + dir * i + btn * j, Direction.GetDirection(IVector3.zero - tan), 0));
                    AddJointIfPossible(manifest, room, new Joint(joint.GetExitLocation() + dir * i + tan * (blockSize.y - 1) + btn * j, Direction.GetDirection(tan), 0));
                }
            }
            for (int i = 0; i < blockSize.y; i++)
            {
                for (int j = 0; j < blockSize.z; j++)
                {
                    AddJointIfPossible(manifest, room, new Joint(joint.GetExitLocation() + tan * i + btn * j, Direction.GetDirection(IVector3.zero - dir), 0));
                    AddJointIfPossible(manifest, room, new Joint(joint.GetExitLocation() + dir * (blockSize.x - 1) + tan * i + btn * j, Direction.GetDirection(dir), 0));
                }
            }
            return true;
        }

        private bool AddJointIfPossible(DungeonManifest manifest, DungeonTreeNode room, Joint joint)
        {
            if (joint.direction % 3 != 1 && CanDig(manifest.dungeon.tiles, joint, room))
            {
                manifest.joints[room].Add(joint);
                return true;
            }
            return false;
        }

        private Joint DigHallwayFrom(DungeonManifest manifest, Joint joint, DungeonTreeEdge edge)
        {
            manifest.joints[edge.from].Remove(joint);
            if (!CanDig(manifest.dungeon.tiles, joint, edge.from))
                return null;
            manifest.dungeon.tiles[joint.location + Direction.Vector[joint.direction]] = new Tile(edge);
            WeightedRandomList<Joint> newJoints = new WeightedRandomList<Joint>();
            //Add the immediate exit spot, then (FOR NOW) add exit joints to all its neighbors
            for (int i = 0; i < 6; i++)
            {
                Joint newJoint = new Joint(joint.GetExitLocation(), i, joint.distanceFromSource + 1);
                bool tryAddingJoint = i % 3 != 1 || joint.direction % 3 != 1;
                if (AddJointIfPossible(manifest, edge.from, newJoint))
                    if (i == joint.direction && i % 3 != 1)
                        newJoints.Add(newJoint, 30);
                    else
                        newJoints.Add(newJoint, 1);
            }
            if (newJoints.Any())
                return newJoints.Get();
            return null;
        }

        private void DigEdge(DungeonManifest manifest, DungeonTreeEdge edge)
        {
            bool completed = false;
            Joint joint = getAvailableJoint(manifest, edge);
            while (joint != null)
            {
                if (joint.distanceFromSource > MIN_HALLWAY_LENGTH)
                    if (DigRoomIfPossible(manifest, edge.to, joint))
                    {
                        completed = true;
                        break;
                    }
                joint = DigHallwayFrom(manifest, joint, edge);
                if (joint == null)
                    joint = getAvailableJoint(manifest, edge);
            }
            if (!completed)
                System.Console.WriteLine("Failed to make " + edge);
        }

        private Joint getAvailableJoint(DungeonManifest manifest, DungeonTreeEdge edge)
        {
            if (manifest.joints[edge.from].Any())
            {
                int index = Dungeon.RAND.Next() % manifest.joints[edge.from].Count();
                Joint ret = manifest.joints[edge.from][index];
                manifest.joints[edge.from].RemoveAt(index);
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

    class DungeonManifest
    {
        public Dungeon dungeon = new Dungeon();
        public Dictionary<DungeonTreeNode, List<Joint>> joints = new Dictionary<DungeonTreeNode, List<Joint>>();
        public DungeonTree tree;

        public DungeonManifest(DungeonTree tree)
        {
            this.tree = tree;
            foreach (DungeonTreeNode obj in tree.GetNodes())
            {
                joints[obj] = new List<Joint>();
            }
        }
    }
}
