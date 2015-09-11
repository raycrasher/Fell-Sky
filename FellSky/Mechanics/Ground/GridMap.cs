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

        public int[,,] Blocks { get; private set; }
        public int[,,] Walls { get; private set; }
        public int[,,] Floors { get; private set; }

        public GridMap(int xSize, int ySize, int zSize)
        {
            Blocks = new int[xSize, ySize, zSize];
            Walls = new int[xSize + 1, ySize + 1, zSize + 1];
            Floors = new int[xSize, ySize, zSize];
        }
    }
}
