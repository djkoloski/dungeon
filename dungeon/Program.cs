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
                for (int i = 1; i <= 1000; i++)
                {
                    tree.AddNode("c" + i);
                    if (i > 2)
                    {
                        tree.AddEdge("c" + i / 3, "c" + i);
                    }
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
