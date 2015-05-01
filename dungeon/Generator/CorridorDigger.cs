using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pb.Collections;

namespace dungeon.Generator
{
	public class CorridorDigger : Digger
	{
		private IVector3 position_;
		private IVector3 target_;
		private int fromComponent_;
		private int toComponent_;

		public CorridorDigger(Dungeon dungeon, int fromComponent, int component, int toComponent) :
			base(dungeon, component)
		{
			fromComponent_ = fromComponent;
			toComponent_ = toComponent;

			// Look for a joint to start from
			HashSet<IVector3> joints = dungeon.joints[fromComponent_];

		}

		public override void Step()
		{
			// Find possible movement directions
			int[] openDirs = new int[Direction.End];
			int options = 0;

			for (int d = Direction.Begin; d != Direction.End; ++d)
				if (dungeon_.Get(position_ + Direction.Vector[d] * 2) == null)
					openDirs[options++] = d;

			if (options == 0)
				return false;

			int moveDir = openDirs[random_.Next() % options];

			for (int i = 1; i <= 2; ++i)
			{
				Tile tile = dungeon_.ForceGet(position_ + Direction.Vector[moveDir] * i);
				tile.open = true;
				tile.component = component_;
			}

			position_ += Direction.Vector[moveDir] * 2;
			++currentLength_;
			
			return (currentLength_ < targetLength_);
		}
	}
}
