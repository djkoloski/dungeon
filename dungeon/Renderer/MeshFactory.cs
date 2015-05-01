using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Pb.Collections;

namespace dungeon.Renderer
{
	public class MeshFactory
	{
		private List<Vertex> vertices;
		private List<UInt32> indices;
		private IVector3 lowerBound;
		private IVector3 upperBound;

		public MeshFactory()
		{
			vertices = new List<Vertex>();
			indices = new List<UInt32>();
		}

		public void Clear()
		{
			vertices = new List<Vertex>();
			indices = new List<UInt32>();
		}

		public void AddDirectionalQuad(IVector3 corner, int direction, Vector3 color)
		{
			IVector3 center = corner + Direction.Center[direction];
			IVector3 normal = Direction.Vector[direction];
			int first = vertices.Count;

			AddVertex((Vector3)center, (Vector3)normal, color);
			AddVertex((Vector3)(center + Direction.Tangent[direction]), (Vector3)normal, color);
			AddVertex((Vector3)(center + Direction.Tangent[direction] + Direction.Bitangent[direction]), (Vector3)normal, color);
			AddVertex((Vector3)(center + Direction.Bitangent[direction]), (Vector3)normal, color);

			AddQuad(first, first + 1, first + 2, first + 3);
		}

		public void AddVertex(Vector3 position, Vector3 normal, Vector3 color)
		{
			vertices.Add(new Vertex(position, normal, color));
		}

		public void AddQuad(int a, int b, int c, int d)
		{
			AddTriangle(a, b, c);
			AddTriangle(c, d, a);
		}

		public void AddTriangle(int a, int b, int c)
		{
			AddIndex(a);
			AddIndex(b);
			AddIndex(c);
		}

		public void AddIndex(int i)
		{
			indices.Add((UInt32)i);
		}

		private static Vector3 GetComponentColor(int component)
		{
			return Pb.Math.HSVToRGB(component * 3.0f / 13.0f * 360.0f);
		}

		public void RenderDungeon(Dungeon dungeon)
		{
			lowerBound = dungeon.tiles.Keys.First();
			upperBound = lowerBound;

			foreach (KeyValuePair<IVector3, Tile> pair in dungeon.tiles)
			{
				IVector3 v = pair.Key;
				Tile tile = pair.Value;

				if (!tile.open)
					continue;

				for (int d = Direction.Begin; d != Direction.End; ++d)
				{
					IVector3 npos = v + Direction.Vector[d];
					Tile neighbor = dungeon.Get(npos);

					if (neighbor != null && neighbor.open)
						continue;

					AddDirectionalQuad(v, d, GetComponentColor(tile.component));
				}

				lowerBound = IVector3.Min(lowerBound, v);
				upperBound = IVector3.Max(upperBound, v);
			}
		}

		public Mesh Build()
		{
			Mesh mesh = new Mesh();
			mesh.Build(vertices.ToArray(), indices.ToArray(), (Vector3)lowerBound, (Vector3)upperBound);
			return mesh;
		}
	}
}
