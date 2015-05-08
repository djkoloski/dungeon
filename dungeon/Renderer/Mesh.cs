using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace dungeon.Renderer
{
    /// <summary>
    /// Manages a single mesh
    /// </summary>
    public class Mesh
    {
        /// <summary>
        /// The vertex buffer object id
        /// </summary>
        private int vertexBuffer_;
        /// <summary>
        /// The index buffer object id
        /// </summary>
        private int indexBuffer_;
        /// <summary>
        /// The number of indices in the mesh
        /// </summary>
        private int indexCount_;
        
        /// <summary>
        /// Constructs a new mesh
        /// </summary>
        public Mesh()
        {
            vertexBuffer_ = -1;
            indexBuffer_ = -1;
            indexCount_ = -1;
        }
        /// <summary>
        /// Clears the mesh and destroys its buffers
        /// </summary>
        public void Clear()
        {
            if (vertexBuffer_ != -1)
            {
                GL.DeleteBuffer(vertexBuffer_);
                vertexBuffer_ = 0;
            }

            if (indexBuffer_ != -1)
            {
                GL.DeleteBuffer(indexBuffer_);
                indexBuffer_ = 0;
            }

            indexCount_ = -1;
        }
        /// <summary>
        /// Builds a new mesh from the required components
        /// </summary>
        /// <param name="vertices">The array of the vertices of the mesh</param>
        /// <param name="indices">The array of the indices of the mesh</param>
        public void Build(Vertex[] vertices, UInt32[] indices)
        {
            // Clear just in case
            Clear();

            // Make the vertex buffer
            GL.GenBuffers(1, out vertexBuffer_);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer_);
            GL.BufferData<Vertex>(
                BufferTarget.ArrayBuffer,
                new IntPtr(vertices.Length * Vertex.SizeInBytes),
                vertices,
                BufferUsageHint.StaticDraw
            );
            // Enable up the vertex attributes
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            // Set where the vertex attributes can be located
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, Vertex.PositionOffset);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, true, Vertex.SizeInBytes, Vertex.NormalOffset);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, Vertex.ColorOffset);

            // Make the index buffer
            GL.GenBuffers(1, out indexBuffer_);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer_);
            GL.BufferData<UInt32>(
                BufferTarget.ElementArrayBuffer,
                new IntPtr(indices.Length * 4),
                indices,
                BufferUsageHint.StaticDraw
            );
            // Set the index count
            indexCount_ = indices.Length;
        }
        /// <summary>
        /// Renders the mesh
        /// </summary>
        public void Render()
        {
            // Bind the index buffer and draw triangles
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer_);
            GL.DrawElements(BeginMode.Triangles, indexCount_, DrawElementsType.UnsignedInt, 0);
        }
    }
}
