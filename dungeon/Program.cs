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
			Window win = new Window(new IVector2(1024, 768));

			DungeonFactory dungeonFactory = new DungeonFactory();
			dungeonFactory.SetSize(new IVector3(5, 5, 5));

			Dungeon dungeon = dungeonFactory.Generate();

			MeshFactory meshFactory = new MeshFactory();
			meshFactory.RenderDungeon(dungeon);

			Mesh mesh = meshFactory.Build();

			win.mesh = mesh;

			win.Run();
			win.Dispose();
		}
	}
}
