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
        private static DungeonTree BuildTree()
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
                DungeonTree tree = BuildTree();

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
