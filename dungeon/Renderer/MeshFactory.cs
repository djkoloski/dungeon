using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Pb.Collections;
using dungeon.Generator;

namespace dungeon.Renderer
{
    /// <summary>
    /// A factory for generating meshes
    /// </summary>
    public class MeshFactory
    {
        /// <summary>
        /// The vertices of the mesh
        /// </summary>
        private List<Vertex> vertices;
        /// <summary>
        /// The indices of the mesh
        /// </summary>
        private List<UInt32> indices;
        /// <summary>
        /// The lower bound of the mesh
        /// </summary>
        private IVector3 lowerBound;
        /// <summary>
        /// The upper bound of the mesh
        /// </summary>
        private IVector3 upperBound;

        /// <summary>
        /// Constructs a new mesh factory
        /// </summary>
        public MeshFactory()
        {
            vertices = new List<Vertex>();
            indices = new List<UInt32>();
            lowerBound = IVector3.zero;
            upperBound = IVector3.zero;
        }
        /// <summary>
        /// Clears the contents of the mesh factory
        /// </summary>
        public void Clear()
        {
            vertices = new List<Vertex>();
            indices = new List<UInt32>();
            lowerBound = IVector3.zero;
            upperBound = IVector3.zero;
        }
        /// <summary>
        /// Adds a quad at a corner with a given direction and color
        /// </summary>
        /// <param name="corner">The lower left coordinate of the voxel the quad belongs to</param>
        /// <param name="direction">The direction the quad faces</param>
        /// <param name="color">The color of the quad</param>
        public void AddDirectionalQuad(IVector3 corner, int direction, Vector3 color)
        {
            // Get the center, normal, and first index of the vertices
            IVector3 center = corner + Direction.Center[direction];
            IVector3 normal = Direction.Vector[direction];
            int first = vertices.Count;

            // Add four vertices
            AddVertex((Vector3)center, (Vector3)normal, color);
            AddVertex((Vector3)(center + Direction.Vector[Direction.Tangent[direction]]), (Vector3)normal, color);
            AddVertex((Vector3)(center + Direction.Vector[Direction.Tangent[direction]] + Direction.Vector[Direction.Bitangent[direction]]), (Vector3)normal, color);
            AddVertex((Vector3)(center + Direction.Vector[Direction.Bitangent[direction]]), (Vector3)normal, color);

            // Add the indices for the quad
            AddQuad(first, first + 1, first + 2, first + 3);
        }

        /// <summary>
        /// Adds a single vertex to the mesh factory
        /// </summary>
        /// <param name="position">The position of the vertex</param>
        /// <param name="normal">The normal of the vertex</param>
        /// <param name="color">The color of the vertex</param>
        public void AddVertex(Vector3 position, Vector3 normal, Vector3 color)
        {
            vertices.Add(new Vertex(position, normal, color));
        }
        /// <summary>
        /// Adds the indices for a quad to the mesh factory
        /// </summary>
        /// <param name="a">The first index counterclockwise</param>
        /// <param name="b">The second index counterclockwise</param>
        /// <param name="c">The third index counterclockwise</param>
        /// <param name="d">The fourth index counterclockwise</param>
        public void AddQuad(int a, int b, int c, int d)
        {
            AddTriangle(a, b, c);
            AddTriangle(c, d, a);
        }
        /// <summary>
        /// Adds the indices for a triangle to the mesh factory
        /// </summary>
        /// <param name="a">The first index counterclockwise</param>
        /// <param name="b">The second index counterclockwise</param>
        /// <param name="c">The third index counterclockwise</param>
        public void AddTriangle(int a, int b, int c)
        {
            AddIndex(a);
            AddIndex(b);
            AddIndex(c);
        }
        /// <summary>
        /// Adds a single index to the mesh factory
        /// </summary>
        /// <param name="i">The index to add</param>
        public void AddIndex(int i)
        {
            indices.Add((UInt32)i);
        }

        private static Vector3 GetTileColor(Tile tile)
        {
            if (tile.isPartOfRoom)
                return Vector3.One;
            return new Vector3(1, 0, 0);
        }
        /// <summary>
        /// Renders the voxels of a dungeon into the mesh factory
        /// </summary>
        /// <param name="dungeon">The dungeon to render</param>
        public void RenderDungeon(DungeonFactory generator)
        {
            // Get the tiles map
            Dictionary<IVector3, Tile> tiles = generator.dungeon.tiles;

            // Get an initial value for the lower and upper bounds
            lowerBound = new IVector3(int.MaxValue, int.MaxValue, int.MaxValue);
            upperBound = new IVector3(int.MinValue, int.MinValue, int.MinValue);

            // Loop over all the tiles in the dungeon
            foreach (KeyValuePair<IVector3, Tile> pair in tiles)
            {
                // Get the coordinate and tile
                IVector3 v = pair.Key;
                Tile tile = pair.Value;

                if (tile.roomInfo.ContainsKey(Tile.DIR_KEY))
                {
                    //Assuming dirs only have two possibilities for now. Our goal is to construct the triangle representing this shape.
                    //Also, assume vertical dir is first, then horizontal.
                    int[] dirs = (int[])tile.roomInfo[Tile.DIR_KEY];
                    //Find the vector perpendicular to both of the given directions...
                    IVector3 perp = IVector3.Cross(Direction.Vector[dirs[0]], Direction.Vector[dirs[1]]);
                    //Working with an imaginary scaled up model (x2) of the cube to put the tile in.
                    //These variables are named with the reference of facing outward from the center towards the normal.
                    Vector3 backLeft = (Vector3)(v + (IVector3.one + Direction.Vector[dirs[0]] - Direction.Vector[dirs[1]] + perp) / 2);
                    Vector3 backRight = (Vector3)(v + (IVector3.one + Direction.Vector[dirs[0]] - Direction.Vector[dirs[1]] - perp) / 2);
                    Vector3 frontRight = (Vector3)(v + (IVector3.one - Direction.Vector[dirs[0]] + Direction.Vector[dirs[1]] - perp) / 2);
                    Vector3 frontLeft = (Vector3)(v + (IVector3.one - Direction.Vector[dirs[0]] + Direction.Vector[dirs[1]] + perp) / 2);
                    Vector3 forwardLeft = (Vector3)(v + (IVector3.one + Direction.Vector[dirs[0]] + Direction.Vector[dirs[1]] + perp) / 2);
                    Vector3 forwardRight = (Vector3)(v + (IVector3.one + Direction.Vector[dirs[0]] + Direction.Vector[dirs[1]] - perp) / 2);
                    Vector3 normal = -(Vector3)Direction.Vector[dirs[0]];
                    Vector3 sideNormal = (Vector3)Direction.Vector[dirs[1]];

                    AddQuadFromPoints(backLeft, backRight, frontRight, frontLeft, normal, GetTileColor(tile));
                    //HACK
                    if (dirs[0] == Direction.Down)
                    {
                        sideNormal *= -1;
                    }
                    AddTriangleFromPoints(backLeft, forwardLeft, frontLeft, sideNormal, GetTileColor(tile));
                    AddTriangleFromPoints(forwardRight, frontRight, backRight, sideNormal, GetTileColor(tile));
                }
                else
                {
                    // Loop over all the directions from the position
                    for (int d = Direction.Begin; d != Direction.End; ++d)
                    {
                        IVector3 neighbor = v + Direction.Vector[d];
                        // Get the neighboring voxel
                        if (generator.dungeon.tiles.ContainsKey(neighbor))
                        {
                            Dictionary<object, object> roomInfo = generator.dungeon.tiles[neighbor].roomInfo;
                            if (!roomInfo.ContainsKey(Tile.DIR_KEY) || ((int[])roomInfo[Tile.DIR_KEY])[1] == Direction.Reverse[d])
                                continue;
                        }
                        AddDirectionalQuad(v, d, GetTileColor(tile));
                    }
                }

                // Update the bounds
                lowerBound = IVector3.Min(lowerBound, v);
                upperBound = IVector3.Max(upperBound, v);
            }
        }
        public void AddTriangleFromPoints(Vector3 a, Vector3 b, Vector3 c, Vector3 normal, Vector3 color)
        {
            int first = vertices.Count;
            AddVertex(a, normal, color);
            AddVertex(b, normal, color);
            AddVertex(c, normal, color);
            AddTriangle(first, first + 1, first + 2);
        }
        public void AddQuadFromPoints(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 normal, Vector3 color)
        {
            int first = vertices.Count;
            AddVertex(a, normal, color);
            AddVertex(b, normal, color);
            AddVertex(c, normal, color);
            AddVertex(d, normal, color);
            AddQuad(first, first + 1, first + 2, first + 3);
        }
        /// <summary>
        /// Builds a mesh from the contents of the factory
        /// </summary>
        /// <returns>A new mesh with the same content as the mesh factory</returns>
        public Mesh Build()
        {
            Mesh mesh = new Mesh();
            mesh.Build(vertices.ToArray(), indices.ToArray(), (Vector3)lowerBound, (Vector3)upperBound);
            return mesh;
        }
    }
}
