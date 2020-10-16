using System.IO;
using System.Reflection;
using OpenTK.Graphics.OpenGL4;

namespace Slope.Layers
{
    public sealed class Game : ILayer
    {
        private readonly Mesh _mesh;
        private readonly Shader _shader;

        public Game()
        {
            _mesh = new Mesh("Assets/player.obj", "Sphere");
            _shader = new Shader(File.OpenText("Assets/shader.vert"), File.OpenText("Assets/shader.frag"));
        }

        public bool Enabled => true;

        public bool Update(float dt)
        {
            return false;
        }

        public void Draw()
        {
            _shader.Bind();
            _mesh.Bind();
            GL.DrawElements(PrimitiveType.Triangles, _mesh.IndexCount, DrawElementsType.UnsignedInt, 0);
        }

        public void Dispose()
        {
            _mesh.Dispose();
        }
    }
}