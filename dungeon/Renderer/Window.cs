using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Pb.Collections;

namespace dungeon.Renderer
{
	public class Window :
		GameWindow
	{
		private IVector2 size_;
		private Shader shader_;
		private float time_;
		
		public Mesh mesh;

		public IVector2 size
		{
			get
			{
				return size_;
			}
		}
		public Shader shader
		{
			get
			{
				return shader_;
			}
		}
		public float aspect
		{
			get
			{
				return (float)size.x / (float)size.y;
			}
		}

		public Window(IVector2 size) :
			base(size.x, size.y, OpenTK.Graphics.GraphicsMode.Default, "Dungeon Generation")
		{
			size_ = size;
			shader_ = new Shader();
			mesh = null;
			time_ = 0.0f;

			shader_.Build(File.ReadAllText("shader.vert"), File.ReadAllText("shader.frag"));

			GL.Enable(EnableCap.DepthTest);
			GL.ClearColor(0, 0, 0, 1);
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			time_ += (float)e.Time;

			Matrix4 modelViewMatrix = Matrix4.LookAt(mesh.center + new Vector3((float)Math.Cos(time_ * 0.25f), 0.0f, (float)Math.Sin(time_ * 0.25f)) * (mesh.radius + 2.0f), mesh.center, Vector3.UnitY);//Matrix4.CreateTranslation(0, 0, -10) * Matrix4.CreateRotationY(time_ * 0.25f) * Matrix4.CreateTranslation(-mesh.centerBound);
			Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 2.0f, aspect, 1.0f, 100.0f);
			
			shader_.SetModelViewMatrix(modelViewMatrix);
			shader_.SetProjectionMatrix(projectionMatrix);

			OpenTK.Input.KeyboardState state = OpenTK.Input.Keyboard.GetState();
			if (state[OpenTK.Input.Key.Escape])
				Exit();
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			GL.Viewport(0, 0, size.x, size.y);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			shader_.Use();
			mesh.Render();

			GL.Flush();
			SwapBuffers();
		}
	}
}
