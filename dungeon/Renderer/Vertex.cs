using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Pb.Collections;

namespace dungeon.Renderer
{
    [System.Serializable]
    public struct Vertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector3 color;

        public static int SizeInBytes
        {
            get
            {
                return 3 * Vector3.SizeInBytes;
            }
        }
        public static int PositionOffset
        {
            get
            {
                return 0;
            }
        }
        public static int NormalOffset
        {
            get
            {
                return Vector3.SizeInBytes;
            }
        }
        public static int ColorOffset
        {
            get
            {
                return 2 * Vector3.SizeInBytes;
            }
        }

        public Vertex(Vector3 position_, Vector3 normal_, Vector3 color_)
        {
            position = position_;
            normal = normal_;
            color = color_;
        }
    }
}