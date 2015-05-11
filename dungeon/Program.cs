using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pb.Collections;
using dungeon.Generator;
using dungeon.Renderer;
using System.Diagnostics;

namespace dungeon
{
    class Program
    {
        private static DungeonTree BuildBinaryTree()
        {
            DungeonTree tree = new DungeonTree();
            int numRooms = 63;
            int[] sizes = new int[numRooms];
            sizes[0] = 16;
            int split = 2;
            for (int i = 1; i <= numRooms; i++)
            {
                if (i > 1)
                    sizes[i - 1] = Math.Max(1, sizes[(i + (split - 2)) / split - 1] / 2);
                tree.AddNode("c" + i, "cube:" + sizes[i - 1]);
                if (i > 1)
                    tree.AddEdge("c" + (i + (split - 2)) / split, "c" + i);
            }
            return tree;
        }

        private static DungeonTree BuildTypicalTree()
        {
            DungeonTree tree = new DungeonTree();
            for (int i = 0; i < 10; i++)
            {
                tree.AddNode("center" + i, "cube:" + (20 - i));
                if (i > 0)
                {
                    tree.AddEdge("center" + (i - 1), "center" + i);
                }
                int numSubRooms = Dungeon.RAND.Next() % 5;
                for (int j = 0; j < numSubRooms; j++)
                {
                    tree.AddNode("offshoot" + i + "," + j, "cube:" + (Dungeon.RAND.Next() % 4 + 2));
                    tree.AddEdge("center" + i, "offshoot" + i + "," + j);
                }
            }
            int numRooms = 63;
            int[] sizes = new int[numRooms];
            sizes[0] = 16;
            int split = 2;
            for (int i = 1; i <= numRooms; i++)
            {
                if (i > 1)
                    sizes[i - 1] = Math.Max(1, sizes[(i + (split - 2)) / split - 1] / 2);
                tree.AddNode("c" + i, "cube:" + sizes[i - 1]);
                if (i > 1)
                    tree.AddEdge("c" + (i + (split - 2)) / split, "c" + i);
            }
            return tree;
        }

        private static DungeonTree BuildFlatDungeon()
        {
            List<int> horizOnly = new List<int> { 0, 2, 3, 5 };
            DungeonFactory.HALLWAY_STAIRWAY_CHANCE = 0;
            DungeonFactory.MIN_HALLWAY_LENGTH = 20;
            DungeonFactory.ALLOW_VERTICAL_HALLWAYS = false;
            DungeonTree tree = new DungeonTree();
            for (int i = 0; i < 500; i++)
            {
                string name = "offshoot" + i;
                DungeonTreeNode node = tree.AddNode(name, "cube:1");
                node.entryJointDirs = horizOnly;
                node.exitJointDirs = horizOnly;
                if (i > 0)
                {
                    string parent = "offshoot" + (Dungeon.RAND.Next() % i);
                    tree.AddEdge(parent, name);
                }
            }
            return tree;
        }

        private static DungeonTree BuildSpaceStation()
        {
            DungeonFactory.SAME_DIR_CHANCE = 1;
            DungeonFactory.HALLWAY_STAIRWAY_CHANCE = 0;
            DungeonFactory.MIN_HALLWAY_LENGTH = 20;
            DungeonFactory.PORTION_FAKE_HALLWAYS_ON_HALLWAYS /= 3;
            DungeonFactory.FAKE_HALLWAY_MAX_LENGTH *= 3;
            DungeonFactory.ALLOW_VERTICAL_HALLWAYS = true;
            DungeonTree tree = new DungeonTree();
            tree.AddNode("center", "rect:40,4,4");
            for (int i = 0; i < 16; i++)
            {
                string name = "offshoot" + i;
                tree.AddNode(name, "cube:" + 4);
                tree.AddEdge("center", name);
            }
            return tree;
        }

        private static DungeonTree BuildDavidSpaceStation()
        {
            DungeonFactory.ALLOW_VERTICAL_HALLWAYS = true;
            DungeonFactory.MIN_HALLWAY_LENGTH = 4;
            DungeonFactory.PORTION_FAKE_HALLWAYS_ON_HALLWAYS = 0;
            DungeonFactory.SAME_DIR_CHANCE = 1;
            DungeonFactory.LOWER_BOUND = new IVector3(-10, -999, -10);
            DungeonFactory.UPPER_BOUND = new IVector3(11, 999, 11);

            int numCenterRooms = 4;

            DungeonTree tree = new DungeonTree();
            tree.startDirection = 1;
            for (int i = 0; i < numCenterRooms; i++)
            {
                int s = 20 - i;
                tree.AddNode("center" + i, "rect:1," + s + "," + s);
                if (i > 0)
                {
                    tree.AddNode("center" + -i, "rect:1," + s + "," + s);
                    tree.AddEdge("center" + (i - 1), "center" + i);
                    tree.AddEdge("center" + -(i - 1), "center" + -i);
                }
            }
            return tree;
        }

        private static DungeonTree BuildCity()
        {
            DungeonFactory.ALLOW_VERTICAL_HALLWAYS = true;
            DungeonFactory.MIN_SPACING = 1;
            DungeonFactory.MIN_HALLWAY_LENGTH = 0;
            DungeonFactory.PORTION_FAKE_HALLWAYS_ON_HALLWAYS = 0;
            DungeonFactory.SAME_DIR_CHANCE = 1;
            DungeonFactory.LOWER_BOUND = new IVector3(-10, 0, -10);
            DungeonFactory.UPPER_BOUND = new IVector3(10, 999, 10);

            DungeonTree tree = new DungeonTree();
            tree.AddNode("center", "rect:1,20,20");
            tree.startDirection = 1;
            for (int i = 0; i < 3; i++)
            {
                tree.AddNode("flatbuilding" + i, "rect:1,3,3");
                tree.AddEdge("center", "flatbuilding" + i);
            }
            tree.AddNode("plaza", "rect:1,5,5");
            tree.AddEdge("center", "plaza");
            for (int i = 0; i < 5; i++)
            {
                tree.AddNode("plazabuilding" + i, "rect:" + (int)(Dungeon.RAND.Next() % 14) + ",1,1");
                tree.AddEdge("plaza", "plazabuilding" + i);
            }
            for (int i = 0; i < 35; i++)
            {
                tree.AddNode("building" + i, "rect:" + (int)(Dungeon.RAND.Next() % 10) + ",1,1");
                tree.AddEdge("center", "building" + i);
            }
            return tree;
        }

        private static DungeonTree BuildOfficeBuilding()
        {
            int height = 20;
            DungeonFactory.SAME_DIR_CHANCE = 0.33;
            DungeonFactory.HALLWAY_STAIRWAY_CHANCE = 0;
            DungeonFactory.LOWER_BOUND = new IVector3(-7, 0, -7);
            DungeonFactory.UPPER_BOUND = new IVector3(10, height, 10);
            //DungeonFactory.ALLOW_HALLWAY_MERGING = true;
            DungeonFactory.MIN_SPACING = 1;
            DungeonFactory.MIN_HALLWAY_LENGTH = 12;
            DungeonFactory.PORTION_FAKE_HALLWAYS_ON_HALLWAYS = 10;
            DungeonTree tree = new DungeonTree();
            tree.AddNode("center", "rect:3,3," + height);
            for (int i = 0; i < height * 3; i++)
            {
                string name = "objective" + i;
                tree.AddNode(name, "cube:" + 1);
                tree.AddEdge("center", name);
            }
            return tree;
        }


        private static DungeonTree BuildTreehouse()
        {
            DungeonFactory.HALLWAY_STAIRWAY_CHANCE = 0;
            DungeonFactory.ALLOW_VERTICAL_HALLWAYS = true;
            DungeonFactory.MIN_SPACING = 1;
            DungeonFactory.SAME_DIR_CHANCE = 0.33;
            DungeonFactory.MIN_HALLWAY_LENGTH = 4;
            DungeonFactory.MAX_HALLWAY_LENGTH = 16;
            DungeonFactory.PORTION_FAKE_HALLWAYS_ON_HALLWAYS = 0;
            DungeonTree tree = new DungeonTree();
            tree.startDirection = 1;
            int numRooms = 63;
            int split = 2;
            for (int i = 1; i <= numRooms; i++)
            {
                if (i == 1)
                    tree.AddNode("c" + i, "rect:1,5,5").exitJointDirs = new List<int> { 1 };
                else
                {
                    DungeonTreeNode node = tree.AddNode("c" + i, "rect:1,7,7");
                    node.entryJointDirs = new List<int> { 4 };
                    node.exitJointDirs = new List<int> { 1 };
                    tree.AddEdge("c" + (i + (split - 2)) / split, "c" + i);
                }
            }
            return tree;
        }

        static void Main(string[] args)
        {
            bool immediate = false;

            foreach (string arg in args)
            {
                if (arg == "--immediate")
                    immediate = true;
            }
            using (Window window = new Window(new IVector2(1024, 768)))
            {
                DungeonTree tree = BuildTreehouse();

                DungeonFactory dungeonFactory = new DungeonFactory(tree);
                dungeonFactory.BeginGeneration();

                if (immediate)
                {
                    Stopwatch timer = new Stopwatch();
                    timer.Start();
                    while (dungeonFactory.Step()) { }
                    timer.Stop();
                    Console.WriteLine("Generation took " + timer.ElapsedMilliseconds + " milliseconds");
                }

                window.Init(dungeonFactory);

                window.Run();
            }
        }
    }
}
