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
        private static readonly int MIN_HALLWAY_SPACING = 1;
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

            DigRoomIfPossible(manifest, tree.root_, IVector3.zero);

            foreach (DungeonTreeEdge edge in tree.GetEdges())
            {
                DigEdge(manifest, edge);
            }

            return manifest.dungeon;
        }


        private void DigRoomIfPossible(DungeonManifest manifest, DungeonTreeNode room, Joint joint)
        {
            switch (room.type)
            {
                default:
                    if (manifest.dungeon.tiles.ContainsKey(joint.location))
                        return;
                    manifest.dungeon.tiles[joint.location] = new Tile(room);
                    for (int i = 0; i < 6; i++)
                        manifest.joints[room].Add(new Joint(joint.location, i, 0));
                    return;
            }
        }

        private void PlaceBlockIfPossible(DungeonManifest manifest, Joint joint, IVector3 blockSize)
        {
            
        }

        private bool AddJointIfPossible(DungeonManifest manifest, DungeonTreeNode room, Joint joint)
        {
            if (CanDig(manifest.dungeon.tiles, joint))
            {
                manifest.joints[room].Add(joint);
                return true;
            }
            return false;
        }

        private Joint DigHallwayFrom(DungeonManifest manifest, Joint joint, DungeonTreeEdge edge)
        {
            manifest.joints[edge.from].Remove(joint);
            if (!CanDig(manifest.dungeon.tiles, joint))
            {
                return null;
            }
            manifest.dungeon.tiles[joint.location + Direction.Vector[joint.direction]] = new Tile(edge);
            WeightedRandomList<Joint> newJoints = new WeightedRandomList<Joint>();
            //Add the immediate exit spot, then (FOR NOW) add exit joints to all its neighbors
            for (int i = 0; i < 6; i++)
            {
                Joint newJoint = new Joint(joint.GetExitLocation(), i, joint.distanceFromSource + 1);
                bool tryAddingJoint = i % 3 != 1 || joint.direction % 3 != 1;
                if (AddJointIfPossible(manifest, edge.from, newJoint))
                {
                    if (i == joint.direction && i % 3 != 1)
                        newJoints.Add(newJoint, 30);
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
            //TODO: Deal with failure
            Joint joint = getAvailableJoint(manifest, edge);
            while (joint != null)
            {
                if (joint.distanceFromSource > DungeonTreeEdge.MIN_DEPTH)
                {
                    DigRoomIfPossible(manifest, edge.to, joint.GetExitLocation());
                    break;
                }
                joint = DigHallwayFrom(manifest, joint, edge);
                if (joint == null)
                    joint = getAvailableJoint(manifest, edge);
            }
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

        private bool CanDig(Dictionary<IVector3, Tile> tiles, Joint joint)
        {
            for (int i = -MIN_HALLWAY_SPACING; i <= MIN_HALLWAY_SPACING; i++)
                for (int j = -MIN_HALLWAY_SPACING; j <= MIN_HALLWAY_SPACING; j++)
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
