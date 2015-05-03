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

            DigRoom(manifest, tree.root_, IVector3.zero);

            foreach (DungeonTreeEdge edge in tree.GetEdges())
            {
                DigEdge(manifest, edge);
            }

            return manifest.dungeon;
        }


        private void DigRoom(DungeonManifest manifest, DungeonTreeNode room, IVector3 location)
        {
            switch (room.type)
            {
                default:
                    manifest.dungeon.tiles[location] = new Tile(room);
                    for (int dir = Direction.Begin; dir < Direction.End; dir++)
                    {
                        AddJointIfPossible(manifest, room, new Joint(location, dir, 0));
                    }
                    return;
            }
        }

        private bool CanDigRoom(DungeonManifest manifest, DungeonTreeNode room, IVector3 location)
        {
            switch (room.type)
            {
                default:
                    return !manifest.dungeon.tiles.ContainsKey(location);
            }
        }

        private bool AddJointIfPossible(DungeonManifest manifest, DungeonTreeNode room, Joint joint)
        {
            if (!manifest.dungeon.tiles.ContainsKey(joint.GetExitLocation()))
            {
                manifest.joints[room].Add(joint);
                return true;
            }
            return false;
        }

        private Joint DigHallwayFrom(DungeonManifest manifest, Joint joint, DungeonTreeEdge edge)
        {
            manifest.joints[edge.from].Remove(joint);
            if (manifest.dungeon.tiles.ContainsKey(joint.GetExitLocation()))
            {
                return null;
            }
            manifest.dungeon.tiles[joint.GetExitLocation()] = new Tile(edge);
            WeightedRandomList<Joint> newJoints = new WeightedRandomList<Joint>();
            //Add the immediate exit spot, then (FOR NOW) add exit joints to all its neighbors
            for (int i = 0; i < 6; i++)
            {
                Joint newJoint = new Joint(joint.GetExitLocation(), i, joint.distanceFromSource + 1);
                bool tryAddingJoint = i % 3 != 1 || joint.direction % 3 != 1;
                if (AddJointIfPossible(manifest, edge.from, newJoint))
                {
                    if (i == joint.direction && i % 3 != 1)
                        newJoints.Add(newJoint, 30);//LMFAO WORST HACK EVER HOLD UP
                    else
                        newJoints.Add(newJoint, 1);
                }
            }
            if (newJoints.Any())
            {
                return newJoints.Get();
            }
            return null;
        }

        private void DigEdge(DungeonManifest manifest, DungeonTreeEdge edge)
        {
            Joint joint = getAvailableJoint(manifest, edge);
            while (joint != null)
            {
                if (joint.distanceFromSource > edge.minDist)
                {
                    if (CanDigRoom(manifest, edge.to, joint.GetExitLocation()))
                    {
                        DigRoom(manifest, edge.to, joint.GetExitLocation());
                        break;
                    }
                }
                joint = DigHallwayFrom(manifest, joint, edge);
                if (joint == null)
                {
                    joint = getAvailableJoint(manifest, edge);
                }
            }
            System.Console.WriteLine("Done digging " + edge);
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
