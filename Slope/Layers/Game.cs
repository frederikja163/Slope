using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Slope.Layers
{
    public sealed class Game : ILayer
    {
        private const float Speed = 10f;
        private readonly Mesh _playerMesh, _cubeMesh, _testObstacle;
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

            var meshes = Mesh.LoadMeshes(Assets.Get("misc.obj"));
            _playerMesh = meshes["Player"];
            _cubeMesh = meshes["Cube"];
            meshes = Mesh.LoadMeshes(Assets.Get("testObstacle.obj"));
            _testObstacle = meshes["Plane"];
            
            _shader = new Shader(Assets.Get("shader.vert"), Assets.Get("shader.frag"));

            _shader.Bind();
            var perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f), window.Width / (float)window.Height, 0.01f, 100f);
            _shader.SetUniform(Shader.GetUniformLocation(_shader, "uPerspective"), ref perspective);

            _viewLoc = Shader.GetUniformLocation(_shader, "uView");
            _view = Matrix4.LookAt(new Vector3(0, 1, -3), Vector3.Zero, Vector3.UnitY);
            _shader.SetUniform(_viewLoc, ref _view);

            _modelLoc = Shader.GetUniformLocation(_shader, "uModel");
            _model = Matrix4.Identity;
            _shader.SetUniform(_modelLoc, ref _model);
            _velocity = new Vector3(0, 0, Speed);
            _pos = new Vector3(0, 1f, 0);

            window.Keyboard.OnKey = (sender, args) =>
            {
                if (args.Key == Key.Escape)
                {
                    _pos = new Vector3(0, 1, 0);
                }

                return false;
            };
        }

        public bool Enabled => true;

        public bool Update(float dt)
        {
            if (_keyboard.IsPressed(Key.A))
            {
                _velocity.X = MathHelper.Lerp(_velocity.X, Speed, dt);
            }
            if (_keyboard.IsPressed(Key.D))
            {
                _velocity.X = MathHelper.Lerp(_velocity.X, -Speed, dt);
            }
            _velocity.X = MathHelper.Lerp(_velocity.X, 0, dt);
            _quaternion = Quaternion.FromAxisAngle(Vector3.Cross(Vector3.UnitY,  _velocity),
                              (_velocity.LengthFast * dt) / (MathF.PI)) * _quaternion;
            _pos += _velocity * dt;
            return false;
        }

        public void Draw()
        {
            _shader.Bind();
            _shader.SetUniform(_viewLoc, ref _view);
            
            _playerMesh.Bind();
            _view = Matrix4.LookAt(_pos + new Vector3(0, 1, -3), _pos, Vector3.UnitY);
            _shader.SetUniform(_viewLoc, ref _view);
            _model = Matrix4.CreateFromQuaternion(_quaternion);
            _model *= Matrix4.CreateTranslation(_pos);
            _shader.SetUniform(_modelLoc, ref _model);
            GL.DrawElements(PrimitiveType.Triangles, _playerMesh.IndexCount, DrawElementsType.UnsignedInt, 0);
            
            _cubeMesh.Bind();
            var identity = Matrix4.CreateScale(10) * Matrix4.CreateTranslation(0, 0, -5);
            _shader.SetUniform(_modelLoc, ref identity);
            // GL.DrawElements(PrimitiveType.Triangles, _cubeMesh.IndexCount, DrawElementsType.UnsignedInt, 0);
            
            _testObstacle.Bind();
            GL.DrawElements(PrimitiveType.Triangles, _testObstacle.IndexCount, DrawElementsType.UnsignedInt, 0);
        }

        public void Dispose()
        {
            _playerMesh.Dispose();
        }
    }
}