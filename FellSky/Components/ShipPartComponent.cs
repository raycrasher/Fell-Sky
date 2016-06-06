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
    public interface IShipPartComponent: IComponent
    {
        ShipPart Part { get; }
        Entity Ship { get; }
    }


    public abstract class ShipPartComponent<T>: IShipPartComponent
        where T :ShipPart
    {
        protected ShipPartComponent(T part, Entity ship)
        {
            Part = part;
        }
        ShipPart IShipPartComponent.Part => Part;
        public T Part { get; private set; }
        public Entity Ship { get; private set; }
    }
}
