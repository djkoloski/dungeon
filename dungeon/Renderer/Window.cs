using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Pb.Collections;
using dungeon.Generator;

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

        private DungeonFactory generator;
        private bool render;
        private bool spaceDownLastFrame;
        private bool kDownLastFrame;
        private bool rDownLastFrame;
        private bool animate;

        private float lastFPSCheck;
        private int frames;

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
            render = true;

            shader_.Build(File.ReadAllText("shader.vert"), File.ReadAllText("shader.frag"));

            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0, 0, 0, 1);
        }

        public void RebuildMesh()
        {
            MeshFactory factory = new MeshFactory();
            factory.RenderDungeon(generator);

            if (mesh_ != null)
                mesh_.Clear();
            mesh_ = factory.Build();
        }
        public void Init(DungeonFactory generator_)
        {
            generator = generator_;
            mesh_ = null;
            eyePos_ = Vector3.Zero;
            eyeTheta_ = 0.0f;
            eyePhi_ = 0.0f;

            RebuildMesh();

            projectionMatrix_ = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 2.0f, aspect, 1.0f, 100.0f);

            CursorVisible = false;
        }
        public void Step()
        {
            if (!generator.Step())
            {
                Console.WriteLine("Rendering Completed!");
                animate = false;
                RebuildMesh();
            }

            if (render)
                RebuildMesh();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            float delta = (float)e.Time;
            time_ += delta;

            ++frames;
            if (time_ > lastFPSCheck + 1.0f)
            {
                Title = "Dungeon Generation: " + frames + " FPS";
                frames = 0;
                lastFPSCheck = time_;
            }

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

            OpenTK.Input.MouseState mouseState = OpenTK.Input.Mouse.GetCursorState();
            eyeTheta_ = (mouseState.X - size_.x / 2) / 200.0f;
            eyePhi_ = (mouseState.Y - size_.y / 2) / -200.0f;

            if (state[OpenTK.Input.Key.K])
            {
                if (!kDownLastFrame)
                    animate = !animate;
                kDownLastFrame = true;
            }
            else
                kDownLastFrame = false;

            if (state[OpenTK.Input.Key.R])
            {
                if (!rDownLastFrame)
                    render = !render;
                rDownLastFrame = true;
            }
            else
                rDownLastFrame = false;

            if (state[OpenTK.Input.Key.Space])
            {
                if (!spaceDownLastFrame)
                    Step();
                spaceDownLastFrame = true;
            }
            else
                spaceDownLastFrame = false;

            if (animate)
                Step();
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
