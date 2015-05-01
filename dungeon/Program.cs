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
				DungeonFactory dungeonFactory = new DungeonFactory();
				dungeonFactory.SetSize(new IVector3(5, 5, 5));

<<<<<<< HEAD
			DungeonFactory dungeonFactory = new DungeonFactory();
=======
				Dungeon dungeon = dungeonFactory.Generate();
>>>>>>> 5851a39c1306e21a93ea187391516d2110858371

				MeshFactory meshFactory = new MeshFactory();
				meshFactory.RenderDungeon(dungeon);

				Mesh mesh = meshFactory.Build();

				win.Init(mesh);

				win.Run();
			}
		}
	}
}
