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
        private static DungeonTree BuildTestTree()
        {
            DungeonTree tree = new DungeonTree();
            int numRooms = 15;
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

        private static DungeonTree BuildSpaceStation()
        {
            DungeonFactory.SAME_DIR_CHANCE = 1;
            DungeonFactory.HALLWAY_STAIRWAY_CHANCE = 0;
            DungeonFactory.MIN_HALLWAY_LENGTH = 8;
            DungeonFactory.ALLOW_VERTICAL_HALLWAYS = true;
            DungeonTree tree = new DungeonTree();
            tree.AddNode("center", "rect:20,4,4");
            for (int i = 0; i < 12; i++)
            {
                string name = "offshoot" + i;
                tree.AddNode(name, "cube:" + 4);
                tree.AddEdge("center", name);
            }
            return tree;
        }

        private static DungeonTree BuildOfficeBuilding()
        {
            int height = 20;
            DungeonFactory.SAME_DIR_CHANCE = 0.33;
            DungeonFactory.HALLWAY_STAIRWAY_CHANCE = 0;
            DungeonFactory.LOWER_BOUND = new IVector3(-7, 0, -7);
            DungeonFactory.UPPER_BOUND = new IVector3(7, height, 7);
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
                DungeonTree tree = BuildOfficeBuilding();

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
