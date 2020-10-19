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
        private Keyboard _keyboard;
        private Vector3 _velocity;
        private Vector3 _pos;
        private Quaternion _quaternion = Quaternion.Identity;

        public Game(Window window)
        {
            _keyboard = window.Keyboard;
            
            _mesh = new Mesh("Assets/player.obj", "Sphere");
            _shader = new Shader(File.OpenText("Assets/shader.vert"), File.OpenText("Assets/shader.frag"));

            _shader.Bind();
            var perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f), window.Width / (float)window.Height, 0.01f, 100f);
            _shader.SetUniform(Shader.GetUniformLocation(_shader, "uPerspective"), ref perspective);

            _viewLoc = Shader.GetUniformLocation(_shader, "uView");
            _view = Matrix4.LookAt(new Vector3(0, 0, -3), Vector3.Zero, Vector3.UnitY);
            _shader.SetUniform(_viewLoc, ref _view);

            _modelLoc = Shader.GetUniformLocation(_shader, "uModel");
            _model = Matrix4.Identity;
            _shader.SetUniform(_modelLoc, ref _model);
            _velocity = new Vector3(0, 0, 1);
            _pos = Vector3.Zero;
        }

        public bool Enabled => true;

        public bool Update(float dt)
        {
            if (_keyboard.IsPressed(Key.A))
            {
                _velocity.X = 100f;
            }
            if (_keyboard.IsPressed(Key.D))
            {
                _velocity.X = -100f;
            }
            _velocity.X *= dt;
            _quaternion = Quaternion.FromAxisAngle(Vector3.Cross(Vector3.UnitY,  _velocity),
                dt * MathF.PI) * _quaternion;
            _pos += _velocity * dt;
            return false;
        }

        public void Draw()
        {
            _shader.Bind();
            _mesh.Bind();
            _view = Matrix4.LookAt(new Vector3(0, _pos.Y, _pos.Z) + new Vector3(0, 3, -3), new Vector3(0, _pos.Y, _pos.Z), Vector3.UnitY);
            _shader.SetUniform(_viewLoc, ref _view);
            _model = Matrix4.CreateFromQuaternion(_quaternion);
            _model *= Matrix4.CreateTranslation(_pos);
            _shader.SetUniform(_modelLoc, ref _model);
            GL.DrawElements(PrimitiveType.Triangles, _mesh.IndexCount, DrawElementsType.UnsignedInt, 0);
        }

        public void Dispose()
        {
            _mesh.Dispose();
        }
    }
}