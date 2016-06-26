using Artemis.Interface;
using FellSky.Game.Ships.Parts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Ships
{
    public interface IShipPartCollection : IPersistable
    {
        string Id { get; }
        IList<ShipPart> Parts { get; }
        Color BaseDecalColor { get; set; }
        Color TrimDecalColor { get; set; }
        Dictionary<string,PartAnimation> Animations { get; }
    }

    public interface IShipPartCollectionComponent: IComponent
    {
        IShipPartCollection  Model { get; }
    }
}
