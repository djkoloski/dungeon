using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace dungeon.Renderer
{
    public class Shader
    {
        private int program_;

        private int modelViewMatrixUniform_;
        private Matrix4 modelViewMatrix_;
        private int projectionMatrixUniform_;
        private Matrix4 projectionMatrix_;

        public Shader()
        {
            program_ = -1;
            modelViewMatrixUniform_ = -1;
            projectionMatrixUniform_ = -1;
        }

        public void Clear()
        {
            if (program_ != -1)
            {
                GL.DeleteProgram(program_);
                program_ = -1;
            }
            modelViewMatrixUniform_ = -1;
            modelViewMatrix_ = default(Matrix4);
            projectionMatrixUniform_ = -1;
            projectionMatrix_ = default(Matrix4);
        }

        public void Build(string vertexSource, string fragmentSource)
        {
            Clear();

            string infoLog;

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexSource);
            GL.CompileShader(vertexShader);
            infoLog = GL.GetShaderInfoLog(vertexShader);
            if (infoLog != "")
            {
                Console.WriteLine("Error while compiling vertex shader:");
                Console.WriteLine(infoLog);
            }

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentSource);
            GL.CompileShader(fragmentShader);
            infoLog = GL.GetShaderInfoLog(vertexShader);
            if (infoLog != "")
            {
                Console.WriteLine("Error while compiling fragment shader:");
                Console.WriteLine(infoLog);
            }

            program_ = GL.CreateProgram();
            GL.AttachShader(program_, vertexShader);
            GL.AttachShader(program_, fragmentShader);
            GL.LinkProgram(program_);
            infoLog = GL.GetProgramInfoLog(program_);
            if (infoLog != "")
            {
                Console.WriteLine("Error while linking shader program:");
                Console.WriteLine(infoLog);
            }

            GL.BindAttribLocation(program_, 0, "vPosition");
            GL.BindAttribLocation(program_, 1, "vNormal");
            GL.BindAttribLocation(program_, 2, "vColor");
            modelViewMatrixUniform_ = GL.GetUniformLocation(program_, "modelViewMatrix");
            projectionMatrixUniform_ = GL.GetUniformLocation(program_, "projectionMatrix");
        }

        public void SetModelViewMatrix(Matrix4 modelViewMatrix)
        {
            modelViewMatrix_ = modelViewMatrix;
        }

        public void SetProjectionMatrix(Matrix4 projectionMatrix)
        {
            projectionMatrix_ = projectionMatrix;
        }

        public void Use()
        {
            GL.UseProgram(program_);
            GL.UniformMatrix4(modelViewMatrixUniform_, false, ref modelViewMatrix_);
            GL.UniformMatrix4(projectionMatrixUniform_, false, ref projectionMatrix_);
        }
    }
}
