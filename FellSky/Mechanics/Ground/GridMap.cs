using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Mechanics.Ground
{
    public enum GridDirection
    {
        E, SE, S, SW, W, NW, N, NE
    }

    public class GridMap
    {
        public const int GlobalTileSize = 64;

        public Point3 Size { get; private set; }

        public int[,,] Blocks { get; private set; }
        public int[,,] Walls { get; private set; }
        public int[,,] Floors { get; private set; }

        public GridMap(Point3 size)
        {
            Blocks = new int[size.X, size.Y, size.Z];
            Walls = new int[size.X + 1, size.Y + 1, size.Z + 1];
            Floors = new int[size.X, size.Y, size.Z];
            Size = size;
        }

        
    }
}
