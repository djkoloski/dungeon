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

        //***** General maze properties *****
        private static int MIN_SPACING = 2; //The minimum space around each hallway or room from anything else.
        private static IVector3 LOWER_BOUND = IVector3.one * -1000000;// Lower bound on locations
        private static IVector3 UPPER_BOUND = IVector3.one * 1000000;// Upper bound on locations

        //***** Hallway properties *****
        private static int SAME_DIR_WEIGHT = 10;//Chances for a hallway to keep its direction
        private static int DIFF_DIR_WEIGHT = 1;//Chances to change direction
        private static int MIN_HALLWAY_LENGTH = 10;//The minimum length of a hallway before making a room

        private static bool ALLOW_VERTICAL_HALLWAYS = false;//Allow hallways to grow up and down just like horizontal ones?
        private static bool ALLOW_HALLWAY_MERGING = false;//Allow hallways from the same room to merge? (if true will break MIN_HALLWAY_LENGTH)

        //***** Backtracking properties *****
        private static int FAKE_HALLWAY_MAX_LENGTH = (int)(MIN_HALLWAY_LENGTH * 1.5); //The maximum distance out a fake hallway will go. (TODO)
        private static double PORTION_FAKE_HALLWAYS = 2;//What multiple of real hallway tiles should be fake?

        DungeonTree tree;

        public Dungeon dungeon;
        Dictionary<DungeonTreeNode, List<Joint>> joints;

        bool backwards = false;
        Queue<DungeonTreeEdge> edgesToDig;//List of edges to dig out
        DungeonTreeEdge currentEdge;//The edge that's currently being dug out
        //backpropagation stuff
        Queue<Joint> backtrackJoints;
        DungeonTreeEdge noiseEdge;
        int noiseHallwayLength = 0;

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
            if (!backwards)
            {
                //If an edge just finished, then find a new one and prepare to start digging it.
                if (currentEdge == null)
                {
                    currentJoint = null;
                    while (currentJoint == null)
                    {
                        if (!edgesToDig.Any())
                        {
                            backwards = true;
                            backtrackJoints = new Queue<Joint>();
                            noiseEdge = new DungeonTreeEdge(tree.root_, null);
                            foreach (List<Joint> jointList in joints.Values)
                            {
                                foreach (Joint joint in jointList)
                                {
                                    backtrackJoints.Enqueue(joint);
                                }
                            }
                            return true;
                        }
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
            }
            else
            {
                if (currentJoint == null)
                {
                    while (Dungeon.RAND.NextDouble() > PORTION_FAKE_HALLWAYS || currentJoint == null)
                    {
                        if (!backtrackJoints.Any())
                            return false;
                        currentJoint = backtrackJoints.Dequeue();
                    }
                    noiseHallwayLength = 0;
                }
                currentJoint = DigHallwayFrom(currentJoint, noiseEdge);
                noiseHallwayLength++;
                if (noiseHallwayLength == FAKE_HALLWAY_MAX_LENGTH)
                {
                    currentJoint = null;
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
        /// <param name="room">The room node to build</param>
        /// <param name="joint"></param>
        /// <param name="blockSize"></param>
        private bool PlaceBlockIfPossible(DungeonTreeNode room, Joint joint, IVector3 blockSize)
        {
            /*
             * FIXME The rooms are currently only placed exactly on the corners of joints
             * This can actually be remedied easily, just make the room's position between
             *   ([-width + 1, 0], [-height + 1, 0]) instead of just (0, 0) relative to the joint.
             * This, however, gives us more options of places to put the room, only some of which will be valid.
             * A candidate list would have to be populated by looking at all possible locations and then a random one picked out of it.
             */

            // Get the enumerations for each relevant direction (vector, tangent, bitangent)
            int vecdir = joint.direction;
            int tandir = Direction.Tangent[vecdir];
            int btndir = Direction.Bitangent[vecdir];

            // Get the vectors for each relevant direction (vector, tangent, bitangent)
            IVector3 dir = Direction.Vector[vecdir];
            IVector3 tan = Direction.Vector[tandir];
            IVector3 btn = Direction.Vector[btndir];

            //Figure out where it's legal to place this (have to try all the spots on the side of the block)
            bool[,] linesBlocked = new bool[(blockSize.y + MIN_SPACING) * 2 - 1, (blockSize.z + MIN_SPACING) * 2 - 1];
            for (int j = 0; j < linesBlocked.GetLength(0); j++)
                for (int k = 0; k < linesBlocked.GetLength(1); k++)
                    if (!linesBlocked[j, k])
                        for (int i = 0; i < blockSize.x + MIN_SPACING * 2; i++)
                            if (dungeon.tiles.ContainsKey(joint.GetExitLocation() + dir * i + tan * (j - MIN_SPACING) + btn * (k - MIN_SPACING)))
                            {
                                for (int dj = Math.Max(0, j - blockSize.y + 1 - MIN_SPACING); dj < Math.Min(linesBlocked.GetLength(0), j + blockSize.y + MIN_SPACING); dj++)
                                    for (int dk = Math.Max(0, k - blockSize.z + 1 - MIN_SPACING); dk < Math.Min(linesBlocked.GetLength(1), k + blockSize.z + MIN_SPACING); dk++)
                                        linesBlocked[dj, dk] = true;
                                break;
                            }
            List<IVector2> legalSpots = new List<IVector2>();
            for (int j = 0; j < blockSize.y; j++)
                for (int k = 0; k < blockSize.z; k++)
                    if (!linesBlocked[j + MIN_SPACING, k + MIN_SPACING])
                        legalSpots.Add(new IVector2(j, k));

            if (!legalSpots.Any())
                return false;

            //Pick which spot to use (shift the block back by x and up by y... kinda)
            IVector2 spot = legalSpots[Dungeon.RAND.Next() % legalSpots.Count()];
            IVector3 delta = tan * spot.x + btn * spot.y;

            //Add in the leading hallways
            if (room.parent != null)
                for (int i = 0; i < MIN_SPACING - 1; i++)
                {
                    dungeon.tiles[joint.GetExitLocation()] = new Tile(room.parent, false);
                    joint = new Joint(joint.GetExitLocation(), joint.direction, joint.distanceFromSource + 1);
                }

            //Place the block, add doorways if needed.
            foreach (IVector3 index in IVector3.Range(blockSize))
            {
                IVector3 loc = joint.GetExitLocation() + dir * index.x + tan * index.y + btn * index.z - delta;
                dungeon.tiles[loc] = new Tile(room, true);
            }
            for (int i = 0; i < blockSize.x; i++)
            {
                for (int j = 0; j < blockSize.y; j++)
                {
                    AddJointIfPossible(room, new Joint(joint.GetExitLocation() + dir * i + tan * j - delta, Direction.Reverse[btndir], 0));
                    AddJointIfPossible(room, new Joint(joint.GetExitLocation() + dir * i + tan * j + btn * (blockSize.z - 1) - delta, btndir, 0));
                }
                for (int j = 0; j < blockSize.z; j++)
                {
                    AddJointIfPossible(room, new Joint(joint.GetExitLocation() + dir * i + btn * j - delta, Direction.Reverse[tandir], 0));
                    AddJointIfPossible(room, new Joint(joint.GetExitLocation() + dir * i + tan * (blockSize.y - 1) + btn * j - delta, tandir, 0));
                }
            }
            for (int i = 0; i < blockSize.y; i++)
            {
                for (int j = 0; j < blockSize.z; j++)
                {
                    AddJointIfPossible(room, new Joint(joint.GetExitLocation() + tan * i + btn * j - delta, Direction.Reverse[vecdir], 0));
                    AddJointIfPossible(room, new Joint(joint.GetExitLocation() + dir * (blockSize.x - 1) + tan * i + btn * j - delta, vecdir, 0));
                }
            }
            return true;
        }

        private bool AddJointIfPossible(DungeonTreeNode room, Joint joint)
        {
            if ((ALLOW_VERTICAL_HALLWAYS || joint.direction % 3 != 1) && CanDigHallway(joint, room))
            {
                joints[room].Add(joint);
                return true;
            }
            return false;
        }

        private Joint DigHallwayFrom(Joint joint, DungeonTreeEdge edge)
        {
            joints[edge.from].Remove(joint);
            if (!CanDigHallway(joint, edge.from))
            {
                if (edge.to != null) TrimSearchTree(edge, joint.location);
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
                {
                    if (edge.to == null)
                    {
                        backtrackJoints.Enqueue(newJoint);
                    }
                    if (i == joint.direction && i % 3 != 1)
                        newJoints.Add(newJoint, SAME_DIR_WEIGHT);
                    else
                        newJoints.Add(newJoint, DIFF_DIR_WEIGHT);
                }
            }
            if (newJoints.Any())
                return newJoints.Get();
            if (edge.to != null) TrimSearchTree(edge, joint.GetExitLocation());
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

        private bool CanDigHallway(Joint joint, DungeonTreeNode source)
        {
            if (!CanPlaceAt(joint.GetExitLocation() + Direction.Vector[joint.direction]))
                if (!ALLOW_HALLWAY_MERGING)
                    return false;
                else if (dungeon.tiles[joint.GetExitLocation() + Direction.Vector[joint.direction]].component != source)
                    return true;

            for (int i = -MIN_SPACING; i <= MIN_SPACING; i++)
                for (int j = -MIN_SPACING; j <= MIN_SPACING; j++)
                    if (!CanPlaceAt(joint.GetExitLocation() + Direction.Vector[Direction.Tangent[joint.direction]] * i + Direction.Vector[Direction.Bitangent[joint.direction]] * j))
                        return false;
            return true;
        }

        private bool CanPlaceAt(IVector3 location)
        {
            return !dungeon.tiles.ContainsKey(location) && (location - LOWER_BOUND).inInterval(UPPER_BOUND - LOWER_BOUND);
        }
    }
}
