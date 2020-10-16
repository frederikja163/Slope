using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Slope.Layers
{
    public sealed class Game : ILayer
    {
        private readonly Mesh _mesh;
        private readonly Shader _shader;
        private readonly int _viewLoc, _modelLoc;
        private Matrix4 _view, _model;

        public Game(Window window)
        {
            window.Keyboard.OnKey += (sender, args) =>
            {
                if ((args.State & KeyState.Pressed) == KeyState.Pressed)
                {
                    if (args.Key == Key.W)
                    {
                        _model *= Matrix4.CreateRotationX(.5f);
                    }
                    else if (args.Key == Key.S)
                    {
                        _model *= Matrix4.CreateRotationX(-.5f);
                    }
                }
                return false;
            };
            
            _mesh = new Mesh("Assets/player.obj", "Sphere");
            _shader = new Shader(File.OpenText("Assets/shader.vert"), File.OpenText("Assets/shader.frag"));

            _shader.Bind();
            var perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f), window.Width / (float)window.Height, 0.01f, 100f);
            _shader.SetUniform(Shader.GetUniformLocation(_shader, "uPerspective"), ref perspective);

            _viewLoc = Shader.GetUniformLocation(_shader, "uView");
            _view = Matrix4.LookAt(new Vector3(0, 3, -3), Vector3.Zero, Vector3.UnitY);
            _shader.SetUniform(_viewLoc, ref _view);

            _modelLoc = Shader.GetUniformLocation(_shader, "uModel");
            _model = Matrix4.Identity;
            _shader.SetUniform(_modelLoc, ref _model);
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
            _shader.SetUniform(_viewLoc, ref _view);
            _shader.SetUniform(_modelLoc, ref _model);
            GL.DrawElements(PrimitiveType.Triangles, _mesh.IndexCount, DrawElementsType.UnsignedInt, 0);
        }

        public void Dispose()
        {
            _mesh.Dispose();
        }
    }
}