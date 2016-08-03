using Artemis.Interface;
using FellSky.Game.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FellSky.Components
{
    public class ShipModelComponent: IComponent
    {
        public Color BaseDecalColor;
        public ShipModel Model;
        public Color TrimDecalColor;
    }
}
