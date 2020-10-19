using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Slope
{
    public sealed class MeshPrototypes
    {
        private readonly Dictionary<string, (float[] vertices, uint[] indices)> _data;

        public MeshPrototypes(Dictionary<string, (float[] vertices, uint[] indices)> data)
        {
            _data = data;
        }

        public Mesh this[string name]
        {
            get
            {
                var (vertices, indices) = _data[name];
                return new Mesh(vertices, indices);
            }
        }
    }
    
    public sealed class Mesh : IDisposable
    {
        private readonly int _vao, _vbo, _ebo;

        public Mesh(float[] vertices, uint[] indices)
        {
            IndexCount = indices.Length;
            _vao = GL.GenVertexArray();
            Bind();
            
            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);
            
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * (3 + 3 + 2), sizeof(float) * (0));
            
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, sizeof(float) * (3 + 3 + 2), sizeof(float) * (3));
            
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, sizeof(float) * (3 + 3 + 2), sizeof(float) * (3 + 3));
            
            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Length, indices, BufferUsageHint.StaticDraw);
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

        public static MeshPrototypes LoadMeshes(StreamReader reader)
        {
            var objects = new Dictionary<string, (float[], uint[])>();
            List<float> vertices = new List<float>();
            List<uint> indices = new List<uint>();
            string currentMesh = "";
            List<Vector3> positions = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> textureCoordinates = new List<Vector2>();
            Dictionary<string, uint> vertexIndices = new Dictionary<string, uint>();
            string? line = "";
            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(' ');
                if (values[0] == "o")
                {
                    if (!string.IsNullOrWhiteSpace(currentMesh))
                    {
                        objects.Add(currentMesh, (vertices.ToArray(), indices.ToArray()));
                    }
                    
                    // vertices = new List<float>();
                    // indices = new List<uint>();
                    // positions = new List<Vector3>();
                    // textureCoordinates = new List<Vector2>();
                    // vertexIndices = new Dictionary<string, uint>();
                    currentMesh = values[1];
                }
                else if (values[0] == "v")
                {
                    float x = float.Parse(values[1]);
                    float y = float.Parse(values[2]);
                    float z = float.Parse(values[3]);
                    positions.Add(new Vector3(x, y, z));
                }
                else if (values[0] == "vn")
                {
                    float x = float.Parse(values[1]);
                    float y = float.Parse(values[2]);
                    float z = float.Parse(values[3]);
                    normals.Add(new Vector3(x, y, z));
                }
                else if (values[0] == "vt")
                {
                    float x = float.Parse(values[1]);
                    float y = float.Parse(values[2]);
                    textureCoordinates.Add(new Vector2(x, y));
                }
                else if (values[0] == "f")
                {
                    for (int i = 1; i < values.Length; i++)
                    {
                        if (!vertexIndices.TryGetValue(values[i], out uint index))
                        {
                            string[] vertValue = values[i].Split('/');
                            int positionIndex = int.Parse(vertValue[0]) - 1;
                            Vector3 position = positions[positionIndex];
                            int textureCoordinateIndex = int.Parse(vertValue[1]) - 1;
                            Vector2 textureCoordinate = textureCoordinates[textureCoordinateIndex];
                            int normalIndex = int.Parse(vertValue[2]) - 1;
                            Vector3 normal = normals[normalIndex];

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
            if (!string.IsNullOrWhiteSpace(currentMesh))
            {
                objects.Add(currentMesh, (vertices.ToArray(), indices.ToArray()));
            }
            return new MeshPrototypes(objects);
        }
    }
}