using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Slope
{
    public sealed class Shader : IDisposable
    {
        public ref struct ShaderCreationParameters
        {
            public StreamReader? VertexStream { get; set; }
            public StreamReader? FragmentStream { get; set; }
        }
        
        private readonly int _handle;

        public Shader(StreamReader vertexStream, StreamReader fragmentStream) : this(
            new ShaderCreationParameters(){VertexStream = vertexStream, FragmentStream = fragmentStream})
        {
            
        }

        public Shader(ShaderCreationParameters param)
        {
            _handle = GL.CreateProgram();
            var shaders = new List<int>();
            void CreateAndAttachShader(ShaderType type, StreamReader stream)
            {
                int shader = GL.CreateShader(type);
                GL.ShaderSource(shader, stream.ReadToEnd());
                #if DEBUG
                GL.GetShaderInfoLog(shader, out string il);
                if (!string.IsNullOrWhiteSpace(il))
                {
                    throw new Exception("Shader compile error: " + il);
                }
                #endif
                GL.AttachShader(_handle, shader);
                shaders.Add(shader);
            }

            if (param.VertexStream != null) CreateAndAttachShader(ShaderType.VertexShader, param.VertexStream);
            if (param.FragmentStream != null) CreateAndAttachShader(ShaderType.FragmentShader, param.FragmentStream);
            GL.LinkProgram(_handle);
            #if DEBUG
            GL.GetProgramInfoLog(_handle, out string il);
            if (!string.IsNullOrWhiteSpace(il))
            {
                throw new Exception("Shader link error: " + il);
            }
            #endif
            foreach (var shader in shaders)
            {
                GL.DetachShader(_handle, shader);
                GL.DeleteShader(shader);
            }
        }
        
        public void SetUniform(int loc, int value)
        {
            GL.Uniform1(loc, value);
        }
        
        public void SetUniform(int loc, Vector2 value)
        {
            GL.Uniform2(loc, value);
        }
        
        public void SetUniform(int loc, Vector3 value)
        {
            GL.Uniform3(loc, value);
        }
        
        public void SetUniform(int loc, Vector4 value)
        {
            GL.Uniform4(loc, value);
        }
        
        public void SetUniform(int loc, ref Matrix4 value)
        {
            GL.UniformMatrix4(loc, false, ref value);
        }

        public void Bind()
        {
            GL.UseProgram(_handle);
        }

        public void Dispose()
        {
            GL.DeleteProgram(_handle);
        }

        public static int GetUniformLocation(Shader shader, string name)
        {
            return GL.GetUniformLocation(shader._handle, name);
        }
    }
}