using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace dungeon.Renderer
{
	public class Mesh
	{
		private Vector3 lowerBound_;
		private Vector3 upperBound_;
		private int vertexBuffer_;
		private int indexBuffer_;
		private int indexCount_;

		public Vector3 lowerBound
		{
			get
			{
				return lowerBound_;
			}
		}
		public Vector3 upperBound
		{
			get
			{
				return upperBound_;
			}
		}
		public Vector3 center
		{
			get
			{
				return (lowerBound + upperBound) / 2.0f;
			}
		}
		public float radius
		{
			get
			{
				return (upperBound_ - lowerBound_).Length / 2.0f;
			}
		}

		public Mesh()
		{
			vertexBuffer_ = -1;
			indexBuffer_ = -1;
			indexCount_ = -1;
		}

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

		public void Build(Vertex[] vertices, UInt32[] indices, Vector3 lowerBound, Vector3 upperBound)
		{
			Clear();

			GL.GenBuffers(1, out vertexBuffer_);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer_);
			GL.BufferData<Vertex>(
				BufferTarget.ArrayBuffer,
				new IntPtr(vertices.Length * Vertex.SizeInBytes),
				vertices,
				BufferUsageHint.StaticDraw
			);
			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);
			GL.EnableVertexAttribArray(2);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, Vertex.PositionOffset);
			GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, true, Vertex.SizeInBytes, Vertex.NormalOffset);
			GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, Vertex.ColorOffset);

			GL.GenBuffers(1, out indexBuffer_);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer_);
			GL.BufferData<UInt32>(
				BufferTarget.ElementArrayBuffer,
				new IntPtr(indices.Length * 4),
				indices,
				BufferUsageHint.StaticDraw
			);
			indexCount_ = indices.Length;

			lowerBound_ = lowerBound;
			upperBound_ = upperBound;
		}

		public void Render()
		{
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer_);
			GL.DrawElements(BeginMode.Triangles, indexCount_, DrawElementsType.UnsignedInt, 0);
		}
	}
}
