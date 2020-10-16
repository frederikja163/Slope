using System;
using System.Collections;
using System.Collections.Generic;

namespace Slope
{
    public interface ILayer : IDisposable
    {
        bool Enabled { get; }
        
        bool Update(float dt);
        void Draw();
    }

    public sealed class LayerStack : IDisposable
    {
        private ILayer[] _layers;

        public LayerStack(params ILayer[] layers)
        {
            _layers = layers;
        }
        
        public void Update(float dt)
        {
            foreach (var layer in _layers)
            {
                if (layer.Enabled)
                {
                    if (layer.Update(dt))
                    {
                        return;
                    }
                }
            }
        }

        public void Draw()
        {
            foreach (var layer in _layers)
            {
                if (layer.Enabled)
                {
                    layer.Draw();
                }
            }
        }
        
        public void Dispose()
        {
            foreach (var layer in _layers)
            {
                layer.Dispose();
            }
        }

        // private void Iterate(Func<ILayer, int, bool> func)
        // {
        //     for (int i = 0; i < _layers.Count; i++)
        //     {
        //         if (func(_layers[i], i))
        //         {
        //             break;
        //         }
        //     }
        // }
        //
        // private void Iterate(Func<ILayer, bool> func)
        // {
        //     Iterate((l, _) => func(l));
        // }
        //
        // private void Iterate(Action<ILayer, int> action)
        // {
        //     Iterate((l, i) =>
        //     {
        //         action(l, i);
        //         return false;
        //     });
        // }
        //
        // private void Iterate(Action<ILayer> action)
        // {
        //     Iterate((l, _) =>
        //     {
        //         action(l);
        //         return false;
        //     });
        // }
        //
        // public int PushLayer(ILayer layer)
        // {
        //     _layers.Add(layer);
        //     return _layers.Count - 1;
        // }
    }
}