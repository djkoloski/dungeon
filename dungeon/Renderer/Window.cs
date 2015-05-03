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
		private Mesh mesh_;

		private Vector3 eyePos_;
		private float eyeTheta_;
		private float eyePhi_;

		private Matrix4 modelViewMatrix_;
		private Matrix4 projectionMatrix_;

		private OpenTK.Input.MouseState mouseState_;

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

		private Vector3 eyeForward_
		{
			get
			{
				return new Vector3(
					-(float)Math.Sin(eyeTheta_),
					(float)Math.Sin(eyePhi_),
					(float)(Math.Cos(eyeTheta_) * Math.Cos(eyePhi_))
				).Normalized();
			}
		}
		private Vector3 eyeRight_
		{
			get
			{
				return Vector3.Cross(eyeForward_, Vector3.UnitY);
			}
		}
		private Vector3 eyeUp_
		{
			get
			{
				return Vector3.Cross(eyeRight_, eyeForward_);
			}
		}

		public Window(IVector2 size) :
			base(size.x, size.y, OpenTK.Graphics.GraphicsMode.Default, "Dungeon Generation")
		{
			size_ = size;
			shader_ = new Shader();
			mesh_ = null;
			time_ = 0.0f;

			shader_.Build(File.ReadAllText("shader.vert"), File.ReadAllText("shader.frag"));

			GL.Enable(EnableCap.DepthTest);
			GL.ClearColor(0, 0, 0, 1);
		}

		public void Init(Mesh mesh)
		{
			mesh_ = mesh;
			eyePos_ = mesh_.center + new Vector3(0, 0, -1) * (mesh_.radius + 2.0f);
			eyeTheta_ = 0.0f;
			eyePhi_ = 0.0f;

			projectionMatrix_ = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 2.0f, aspect, 1.0f, 100.0f);
			
			mouseState_ = OpenTK.Input.Mouse.GetCursorState();
			CursorVisible = false;
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			float delta = (float)e.Time;
			time_ += delta;

			modelViewMatrix_ = Matrix4.LookAt(eyePos_, eyePos_ + eyeForward_, Vector3.UnitY);

			OpenTK.Input.KeyboardState state = OpenTK.Input.Keyboard.GetState();
			if (state[OpenTK.Input.Key.Escape])
				Exit();

			float moveDelta = 20.0f * delta;
			// Move the camera around
            if (state[OpenTK.Input.Key.Q])
				eyePos_ -= eyeUp_ * moveDelta;
			if (state[OpenTK.Input.Key.W])
				eyePos_ += eyeForward_ * moveDelta;
			if (state[OpenTK.Input.Key.E])
				eyePos_ += eyeUp_ * moveDelta;
			if (state[OpenTK.Input.Key.A])
				eyePos_ -= eyeRight_ * moveDelta;
			if (state[OpenTK.Input.Key.S])
				eyePos_ -= eyeForward_ * moveDelta;
			if (state[OpenTK.Input.Key.D])
				eyePos_ += eyeRight_ * moveDelta;
				
			OpenTK.Input.MouseState nextMouseState = OpenTK.Input.Mouse.GetCursorState();
			eyeTheta_ += (nextMouseState.X - mouseState_.X) / 200.0f;
			eyePhi_ += (nextMouseState.Y - mouseState_.Y) / -200.0f;
			
			mouseState_ = nextMouseState;
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			GL.Viewport(0, 0, size.x, size.y);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			shader_.Use();
			shader_.SetModelViewMatrix(modelViewMatrix_);
			shader_.SetProjectionMatrix(projectionMatrix_);

			mesh_.Render();

			GL.Flush();
			SwapBuffers();
		}
	}
}
