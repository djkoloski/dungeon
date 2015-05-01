using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Pb.Collections;

namespace dungeon.Generator
{
	public class DungeonFactory
	{
		private IVector3 size_;

		public DungeonFactory()
		{
			size_ = IVector3.zero;
		}

		public void SetSize(IVector3 size)
		{
			size_ = size;
		}

		public Dungeon Generate()
		{
			Dungeon dungeon = new Dungeon();

			IVector3 lastPosition = IVector3.zero;
			for (int i = 0; i < 20; ++i)
			{
				Digger digger = new Digger(dungeon, -1, lastPosition, 20);
				while (digger.Step())
				{ }
				lastPosition = digger.GetPosition();
			}

			return dungeon;
		}
	}
}
