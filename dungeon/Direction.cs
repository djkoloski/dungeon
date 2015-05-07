using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pb.Collections;

namespace dungeon
{
    public static class Direction
    {
        public static int Right = 0;
        public static int Up = 1;
        public static int Forward = 2;
        public static int Left = 3;
        public static int Down = 4;
        public static int Back = 5;

        public static int Begin = 0;
        public static int End = 6;

        public static int GetDirection(IVector3 vector)
		{//David if you ever see this I am so sorry I couldn't find a better way.
		/*
		 * Hmm, this could be remedied for now by making the tangent/bitangent/reverse return directions instead of vectors
		 * I changed it and it works fine, but we might need this again in the future maybe
		 * <3 David
		 */
            for (int i = 0; i < Vector.Count(); i++)
                if (Vector[i] == vector)
                    return i;
            return -1;
        }

		public static int[] Tangent = new int[6]{
			Back,
			Left,
			Down,
			Up,
			Forward,
			Right
		};
		public static int[] Bitangent = new int[6]{
			Down,
			Back,
			Left,
			Forward,
			Right,
			Up
		};
		public static int[] Reverse = new int[6]{
			Left,
			Down,
			Back,
			Right,
			Up,
			Forward
		};
        public static IVector3[] Vector = new IVector3[6]{
			IVector3.right,
			IVector3.up,
			IVector3.forward,
			IVector3.left,
			IVector3.down,
			IVector3.back
		};
        public static IVector3[] Center = new IVector3[6]{
			IVector3.one,
			IVector3.one,
			IVector3.one,
			IVector3.zero,
			IVector3.zero,
			IVector3.zero
		};
    }
}
