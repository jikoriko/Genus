using System;
using System.IO;

using OpenTK.Graphics.OpenGL;

namespace Genus2D.Graphics
{
    public class Shader
    {
        private int _shaderProgramID;
        private int _vertexShaderID;
        private int _fragmentShaderID;

        public Shader(string vertexShader, string fragmentShader)
        {
            LoadShader(vertexShader, fragmentShader);
        }

        private void LoadShader(string vertexShader, string fragmentShader)
        {
            StreamReader reader;
            _vertexShaderID = GL.CreateShader(ShaderType.VertexShader);
            reader = new StreamReader(vertexShader);
            GL.ShaderSource(_vertexShaderID, reader.ReadToEnd());
            reader.Close();
            GL.CompileShader(_vertexShaderID);

            int result;
            GL.GetShader(_vertexShaderID, ShaderParameter.CompileStatus, out result);
            if (result == 0)
            {
                throw new Exception("Failed to compile vertex shader!" + GL.GetShaderInfoLog(_vertexShaderID));
            }

            _fragmentShaderID = GL.CreateShader(ShaderType.FragmentShader);
            reader = new StreamReader(fragmentShader);
            GL.ShaderSource(_fragmentShaderID, reader.ReadToEnd());
            reader.Close();
            GL.CompileShader(_fragmentShaderID);

            GL.GetShader(_fragmentShaderID, ShaderParameter.CompileStatus, out result);
            if (result == 0)
            {
                throw new Exception("Failed to compile vertex shader!" + GL.GetShaderInfoLog(_fragmentShaderID));
            }

            _shaderProgramID = GL.CreateProgram();
            GL.AttachShader(_shaderProgramID, _vertexShaderID);
            GL.AttachShader(_shaderProgramID, _fragmentShaderID);
            GL.LinkProgram(_shaderProgramID);
        }

        public int GetID()
        {
            return _shaderProgramID;
        }

        public int GetAttribLocation(string variable)
        {
            return GL.GetAttribLocation(_shaderProgramID, variable);
        }

        public int GetUniformLocation(string variable)
        {
            return GL.GetUniformLocation(_shaderProgramID, variable);
        }

        public virtual void Bind()
        {
            GL.UseProgram(_shaderProgramID);
        }

        public void Destroy()
        {
            GL.DetachShader(_shaderProgramID, _vertexShaderID);
            GL.DetachShader(_shaderProgramID, _fragmentShaderID);
            GL.DeleteShader(_vertexShaderID);
            GL.DeleteShader(_fragmentShaderID);
            GL.DeleteProgram(_shaderProgramID);
        }

    }
}
