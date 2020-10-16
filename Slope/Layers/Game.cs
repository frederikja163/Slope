namespace Slope.Layers
{
    public sealed class Game : ILayer
    {
        public bool Enabled { get; private set; }

        public bool Update(float dt)
        {
            return false;
        }

        public void Draw()
        {
        }

        public void Dispose()
        {
        }
    }
}