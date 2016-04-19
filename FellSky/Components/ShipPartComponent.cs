using Artemis;
using Artemis.Interface;
using FellSky.Game.Ships.Parts;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public interface IShipPartComponent
    {
        ShipPart Part { get; }
    }


    public abstract class ShipPartComponent<T>: IShipPartComponent, IComponent
        where T :ShipPart
    {
        protected ShipPartComponent(T part, Entity ship)
        {
            Part = part;
            Ship = ship;
        }
        ShipPart IShipPartComponent.Part => Part;
        public T Part { get; private set; }
        public Entity Ship { get; private set; }
    }
}
