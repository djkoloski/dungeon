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
		public static IVector3[] Tangent = new IVector3[6]{
			IVector3.back,
			IVector3.left,
			IVector3.down,
			IVector3.up,
			IVector3.forward,
			IVector3.right
		};
		public static IVector3[] Bitangent = new IVector3[6]{
			IVector3.down,
			IVector3.back,
			IVector3.left,
			IVector3.forward,
			IVector3.right,
			IVector3.up
		};
		public static IVector3[] Reverse = new IVector3[6]{
			IVector3.left,
			IVector3.down,
			IVector3.back,
			IVector3.right,
			IVector3.up,
			IVector3.forward
		};
	}
}
