using FellSky.Models.Ships.Parts;

namespace FellSky.Components
{
    public class ShipPartComponent<T>
        where T : ShipPart
    {
        public ShipPartComponent(T part)
        {
            Part = part;
        }

        public T Part { get; set; }
    }
}
