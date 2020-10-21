using System;
using System.IO;
using System.Reflection;

namespace Slope
{
    public static class Assets
    {
        public static Assembly? Assembly { get; set; }
        
        public static StreamReader Get(string name)
        {
            var stream = Assembly?.GetManifestResourceStream("Slope.Assets." + name);
            #if DEBUG
            if (stream == null)
            {
                throw new Exception("Asset does not exist: " + name);
            }
            #endif
            return new StreamReader(stream);
        }
    }
}