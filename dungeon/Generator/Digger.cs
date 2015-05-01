using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pb.Collections;

namespace dungeon.Generator
{
    public class Digger
    {
        private static Random random_ = new Random();

        private Dungeon dungeon_;
        private int component_;
        private IVector3 position_;
        private int currentLength_;
        private int maxLength_;

        /// <summary>
        /// Makes a new digger at the given location.
        /// </summary>
        /// <param name="dungeon"></param>
        /// <param name="component"></param>
        /// <param name="position"></param>
        /// <param name="maxLength"></param>
        public Digger(Dungeon dungeon, int component, IVector3 position, int maxLength)
        {
            dungeon_ = dungeon;
            if (component >= 0)
                component_ = component;
            else
                component_ = dungeon.AddComponent();

            position_ = position;
            currentLength_ = 0;
            maxLength_ = maxLength;
        }

        public IVector3 GetPosition()
        {
            return position_;
        }

        /// <summary>
        /// Steps the digger once
        /// </summary>
        /// <returns>Whether the digger should step again</returns>
        public virtual bool Step()
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

            return (currentLength_ < maxLength_);
        }
    }
}
