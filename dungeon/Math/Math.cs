using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pb.Collections;
using OpenTK;

namespace Pb
{
	/// <summary>
	/// Contains mathematical helper functions
	/// </summary>
	public static class Math
	{
		/// <summary>
		/// Divides the first integer by the second and rounds down to the nearest integer without loss of precision
		/// </summary>
		/// <param name="a">The dividend</param>
		/// <param name="b">The divisor</param>
		/// <returns>The quotient rounded down to the nearest integer</returns>
		public static int FloorDivide(int a, int b)
		{
			if (b < 0)
			{
				a = -a;
				b = -b;
			}

			if (a < 0)
				return (a - b + 1) / b;
			return a / b;
		}
		/// <summary>
		/// Performs a component-wise floor divide on two integer vectors
		/// </summary>
		/// <param name="a">The dividend</param>
		/// <param name="b">The divisor</param>
		/// <returns>The quotient rounded down to the nearest integer</returns>
		public static IVector2 FloorDivide(IVector2 a, IVector2 b)
		{
			return new IVector2(FloorDivide(a.x, b.x), FloorDivide(a.y, b.y));
		}
		/// <summary>
		/// Performs a component-wise floor divide on two integer vectors
		/// </summary>
		/// <param name="a">The dividend</param>
		/// <param name="b">The divisor</param>
		/// <returns>The quotient rounded down to the nearest integer</returns>
		public static IVector3 FloorDivide(IVector3 a, IVector3 b)
		{
			return new IVector3(FloorDivide(a.x, b.x), FloorDivide(a.y, b.y), FloorDivide(a.z, b.z));
		}
		/// <summary>
		/// Divides the first integer by the second and rounds up to the nearest integer without loss of precision
		/// </summary>
		/// <param name="a">The dividend</param>
		/// <param name="b">The divisor</param>
		/// <returns>The quotient rounded up to the nearest integer</returns>
		public static int CeilDivide(int a, int b)
		{
			if (b < 0)
			{
				a = -a;
				b = -b;
			}

			if (a < 0)
				return a / b;
			return (a + b - 1) / b;
		}
		/// <summary>
		/// Performs a component-wise ceil divide on two integer vectors
		/// </summary>
		/// <param name="a">The dividend</param>
		/// <param name="b">The divisor</param>
		/// <returns>The quotient rounded up to the nearest integer</returns>
		public static IVector2 CeilDivide(IVector2 a, IVector2 b)
		{
			return new IVector2(CeilDivide(a.x, b.x), CeilDivide(a.y, b.y));
		}
		/// <summary>
		/// Performs a component-wise ceil divide on two integer vectors
		/// </summary>
		/// <param name="a">The dividend</param>
		/// <param name="b">The divisor</param>
		/// <returns>The quotient rounded up to the nearest integer</returns>
		public static IVector3 CeilDivide(IVector3 a, IVector3 b)
		{
			return new IVector3(CeilDivide(a.x, b.x), CeilDivide(a.y, b.y), CeilDivide(a.z, b.z));
		}
		/// <summary>
		/// Calculates the RGB color corresponding to the given HSV coordinates
		/// </summary>
		/// <param name="h">The hue component of the color</param>
		/// <param name="s">The saturation component of the color</param>
		/// <param name="v">The value component of the color</param>
		/// <returns>The RGB color corresponding to the given HSV coordinates</returns>
		public static Vector3 HSVToRGB(float h, float s = 1.0f, float v = 1.0f)
		{
			h = h % 360.0f;

			float c = v * s;
			float x = c * (1.0f - System.Math.Abs(h / 60.0f % 2.0f - 1.0f));
			float m = v - c;

			float rp = 0.0f;
			float gp = 0.0f;
			float bp = 0.0f;

			if (h >= 0.0f && h < 60.0f)
			{
				rp = c;
				gp = x;
			}
			else if (h >= 60.0f && h < 120.0f)
			{
				rp = x;
				gp = c;
			}
			else if (h >= 120.0f && h < 180.0f)
			{
				gp = c;
				bp = x;
			}
			else if (h >= 180.0f && h < 240.0f)
			{
				gp = x;
				bp = c;
			}
			else if (h >= 240.0f && h < 300.0f)
			{
				rp = x;
				bp = c;
			}
			else
			{
				rp = c;
				bp = x;
			}

			return new Vector3(rp + m, gp + m, bp + m);
		}
		/// <summary>
		/// Maps integers to natural numbers (0 included)
		/// </summary>
		/// <param name="x">An integer</param>
		/// <returns>A unique natural number corresponding to that integer</returns>
		public static uint ZToN(int x)
		{
			return (uint)x;
		}
		/// <summary>
		/// Maps natural numbers (0 included) to integers
		/// </summary>
		/// <param name="x">A natural number</param>
		/// <returns>A unique integer corresponding to that natural number</returns>
		public static int NToZ(uint x)
		{
			return (int)x;
		}
		/// <summary>
		/// Pairs two integers into a natural number
		/// </summary>
		/// <param name="x">The first integer to pair</param>
		/// <param name="y">The second integer to pair</param>
		/// <returns>A natural number corresponding to the pair of the two integers</returns>
		public static uint Pair(int x, int y)
		{
			return Pair(ZToN(x), ZToN(y));
		}
		/// <summary>
		/// Pairs three integers into a natural number
		/// </summary>
		/// <param name="x">The first integer to pair</param>
		/// <param name="y">The second integer to pair</param>
		/// <param name="z">The third integer to pair</param>
		/// <returns>A natural number corresponding to the pair of the three integers</returns>
		public static uint Pair(int x, int y, int z)
		{
			return Pair(ZToN(x), ZToN(y), ZToN(z));
		}
		/// <summary>
		/// Pairs the X and Y components of an integer vector
		/// </summary>
		/// <param name="v">The integer vector</param>
		/// <returns>The pair of the two components</returns>
		public static uint Pair(IVector2 v)
		{
			return Pair(v.x, v.y);
		}
		/// <summary>
		/// Pairs the X, Y, and Z components of an integer vector
		/// </summary>
		/// <param name="v">The integer vector</param>
		/// <returns>The pair of the three components</returns>
		public static uint Pair(IVector3 v)
		{
			return Pair(v.x, v.y, v.z);
		}
		/// <summary>
		/// Pairs two natural numbers into a natural number
		/// </summary>
		/// <param name="x">The first natural number to pair</param>
		/// <param name="y">The second natural number to pair</param>
		/// <returns>A natural number corresponding to the pair of the two natural numbers</returns>
		public static uint Pair(uint x, uint y)
		{
			return (x + y) * (x + y + 1) / 2 + y;
		}
		/// <summary>
		/// Pairs three natural numbers into a natural number
		/// </summary>
		/// <param name="x">The first natural number to pair</param>
		/// <param name="y">The second natural number to pair</param>
		/// <param name="y">The third natural number to pair</param>
		/// <returns>A natural number corresponding to the pair of the three natural numbers</returns>
		public static uint Pair(uint x, uint y, uint z)
		{
			return Pair(Pair(x, y), z);
		}
		/// <summary>
		/// Determines the pair of integers that a natural number corresponds to
		/// </summary>
		/// <param name="n">The natural number</param>
		/// <param name="x">The first integer of the pair</param>
		/// <param name="y">The second integer of the pair</param>
		public static void Unpair(uint n, out int x, out int y)
		{
			uint ux;
			uint uy;
			Unpair(n, out ux, out uy);
			x = NToZ(ux);
			y = NToZ(uy);
		}
		/// <summary>
		/// Determines the triplet of integers that a natural number corresponds to
		/// </summary>
		/// <param name="n">The natural number</param>
		/// <param name="x">The first integer of the pair</param>
		/// <param name="y">The second integer of the pair</param>
		/// <param name="z">The third integer of the pair</param>
		public static void Unpair(uint n, out int x, out int y, out int z)
		{
			uint ux;
			uint uy;
			uint uz;
			Unpair(n, out ux, out uy, out uz);
			x = NToZ(ux);
			y = NToZ(uy);
			z = NToZ(uz);
		}
		/// <summary>
		/// Determines the IVector2 that a natural number corresponds to
		/// </summary>
		/// <param name="n">The natural number</param>
		/// <returns>The IVector2 the natural number corresponds to</returns>
		public static IVector2 Unpair2i(uint n)
		{
			int x;
			int y;
			Unpair(n, out x, out y);
			return new IVector2(x, y);
		}
		/// <summary>
		/// Determines the IVector3 that a natural number corresponds to
		/// </summary>
		/// <param name="n">The natural number</param>
		/// <returns>The IVector3 the natural number corresponds to</returns>
		public static IVector3 Unpair3i(uint n)
		{
			int x;
			int y;
			int z;
			Unpair(n, out x, out y, out z);
			return new IVector3(x, y, z);
		}
		/// <summary>
		/// Determines the pair of natural numbers that a natural number corresponds to
		/// </summary>
		/// <param name="n">The natural number</param>
		/// <param name="x">The first natural number of the pair</param>
		/// <param name="y">The second natural number of the pair</param>
		public static void Unpair(uint n, out uint x, out uint y)
		{
			uint w = (uint)System.Math.Floor((System.Math.Sqrt(8.0 * n + 1.0) - 1.0) / 2.0);
			uint t = (w * w + w) / 2;
			y = n - t;
			x = w - y;
		}
		/// <summary>
		/// Determines the triplet of natural numbers that a natural number corresponds to
		/// </summary>
		/// <param name="n">The natural number</param>
		/// <param name="x">The first natural number of the triplet</param>
		/// <param name="y">The second natural number of the triplet</param>
		/// <param name="z">The third natural number of the triplet</param>
		public static void Unpair(uint n, out uint x, out uint y, out uint z)
		{
			uint temp;
			Unpair(n, out temp, out z);
			Unpair(temp, out x, out y);
		}
	}
}