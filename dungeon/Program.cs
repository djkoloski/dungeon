using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pb.Collections;
using dungeon.Generator;
using dungeon.Renderer;

namespace dungeon
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Window win = new Window(new IVector2(1024, 768)))
            {
                DungeonTree tree = new DungeonTree();
                int numRooms = 259;
                int[] sizes = new int[numRooms];
                sizes[0] = 32;
                int split = 6;
                for (int i = 1; i <= numRooms; i++)
                {
                    if (i > 1)
                        sizes[i - 1] = sizes[(i + (split - 2)) / split - 1] / 2;
                    tree.AddNode("c" + i, "cube:" + sizes[i - 1]);
                    if (i > 1)
                        tree.AddEdge("c" + (i + (split - 2)) / split, "c" + i);
                }

                DungeonFactory dungeonFactory = new DungeonFactory(tree);

                Dungeon dungeon = dungeonFactory.Generate();

                MeshFactory meshFactory = new MeshFactory();
                meshFactory.RenderDungeon(dungeon);

                Mesh mesh = meshFactory.Build();

                win.Init(mesh);

                win.Run();
            }
        }
    }
}
