using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Slope
{
    public sealed class Mesh : IDisposable
    {
        private readonly int _vao, _vbo, _ebo;
        
        public Mesh(string meshPath, string meshName)
        {
            using var file = File.OpenRead(meshPath);
            var sr = new StreamReader(file);

            while (sr.ReadLine() != "o " + meshName && !sr.EndOfStream) ;
            if (sr.EndOfStream)
            {
                return;
            }

            List<float> vertices = new List<float>();
            List<uint> indices = new List<uint>();
            var positions = new List<Vector3>();
            var normals = new List<Vector3>();
            var textureCoordinates = new List<Vector2>();
            var vertexIndices = new Dictionary<string, uint>();
            var line = "";
            while (!sr.EndOfStream && !(line = sr.ReadLine()).StartsWith("o "))
            {
                var values = line.Split(' ');
                if (values[0] == "v")
                {
                    var x = float.Parse(values[1]);
                    var y = float.Parse(values[2]);
                    var z = float.Parse(values[3]);
                    positions.Add(new Vector3(x, y, z));
                }
                else if (values[0] == "vn")
                {
                    var x = float.Parse(values[1]);
                    var y = float.Parse(values[2]);
                    var z = float.Parse(values[3]);
                    normals.Add(new Vector3(x, y, z));
                }
                else if (values[0] == "vt")
                {
                    var x = float.Parse(values[1]);
                    var y = float.Parse(values[2]);
                    textureCoordinates.Add(new Vector2(x, y));
                }
                else if (values[0] == "f")
                {
                    for (int i = 1; i < values.Length; i++)
                    {
                        if (!vertexIndices.TryGetValue(values[i], out var index))
                        {
                            var vertValue = values[i].Split('/');
                            var positionIndex = int.Parse(vertValue[0]) - 1;
                            var position = positions[positionIndex];
                            var textureCoordinateIndex = int.Parse(vertValue[1]) - 1;
                            var textureCoordinate = textureCoordinates[textureCoordinateIndex];
                            var normalIndex = int.Parse(vertValue[2]) - 1;
                            var normal = normals[normalIndex];

                            vertices.Add(position.X);
                            vertices.Add(position.Y);
                            vertices.Add(position.Z);
                            vertices.Add(normal.X);
                            vertices.Add(normal.Y);
                            vertices.Add(normal.Z);
                            vertices.Add(textureCoordinate.X);
                            vertices.Add(textureCoordinate.Y);
                            index = (uint) vertices.Count / (3 + 3 + 2) - 1;
                            vertexIndices.Add(values[i], index);
                        }

                        indices.Add(index);
                    }
                }
            }

            var verts = vertices.ToArray();
            var ind = indices.ToArray();
            IndexCount = ind.Length;
            _vao = GL.GenVertexArray();
            Bind();
            
            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * verts.Length, verts, BufferUsageHint.StaticDraw);
            
            _ebo = GL.GenBuffer();
                
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * ind.Length, ind, BufferUsageHint.StaticDraw);
            
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * (3 + 3 + 2), sizeof(float) * (0));
            
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, sizeof(float) * (3 + 3 + 2), sizeof(float) * (3));
            
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, sizeof(float) * (3 + 3 + 2), sizeof(float) * (3 + 3));
        }
        
        public int IndexCount { get; }

        public void Bind()
        {
            GL.BindVertexArray(_vao);
        }

        public void Dispose()
        {
            GL.DeleteVertexArray(_vao);
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);
        }
    }
}