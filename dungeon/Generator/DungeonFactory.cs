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
		private List<ComponentConstraint> constraints_;
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

			for (int i = 0; i < constraints_.Count; ++i)
			{
				Digger digger = new Digger(dungeon, 20, start, Direction.Right, i);
				while (digger.Step())
				{ }
			}

			return dungeon;
		}
	}
}
