using FellSky.Game.Ships.Parts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Editor
{
    [PropertyChanged.ImplementPropertyChanged]
    public class ShipPartEditorViewModel
    {
        private IReadOnlyCollection<ShipPart> _parts;

        public IReadOnlyCollection<ShipPart> Parts {
            get { return _parts; }
            set {
                _parts = value;
                HasItems = _parts.Any();
                General = new ShipPartViewModel(_parts);
            }
        }
        public ShipPartViewModel General { get; set; }

        public bool HasItems { get; set; }

        public ShipPartEditorViewModel()
        {
        }
    }

    [PropertyChanged.ImplementPropertyChanged]
    public class ShipPartViewModel
    {
        IReadOnlyCollection<ShipPart> _parts { get; set; }

        public ShipPartViewModel(IReadOnlyCollection<ShipPart> parts)
        {
            _parts = parts;
        }

        public float? PosX {
            get {
                return _parts?.Select(p => new float?(p.Transform.Position.X)).GetAllEqualOrNothing();
            }
            set
            {
                foreach (var item in _parts)
                    item.Transform.Position = new Microsoft.Xna.Framework.Vector2(value ?? item.Transform.Position.X, item.Transform.Position.Y);
            }
        }
        public float? PosY
        {
            get
            {
                return _parts?.Select(p => new float?(p.Transform.Position.Y)).GetAllEqualOrNothing();
            }
            set
            {
                foreach (var item in _parts)
                    item.Transform.Position = new Vector2(item.Transform.Position.X, value ?? item.Transform.Position.Y);
            }
        }
        public float? Rot
        {
            get
            {
                var value = _parts?.Select(p => new float?(p.Transform.Rotation)).GetAllEqualOrNothing();
                if (value != null) return MathHelper.ToDegrees(value.Value);
                else return null;
            }
            set
            {
                foreach (var item in _parts)
                    item.Transform.Rotation = MathHelper.ToRadians(value ?? item.Transform.Rotation);
            }
        }
        public float? ScaleX
        {
            get
            {
                return _parts?.Select(p => new float?(p.Transform.Scale.X)).GetAllEqualOrNothing();
            }
            set
            {
                foreach (var item in _parts)
                    item.Transform.Scale = new Vector2(value ?? item.Transform.Scale.X, item.Transform.Scale.Y);
            }
        }
        public float? ScaleY
        {
            get
            {
                return _parts?.Select(p => new float?(p.Transform.Scale.Y)).GetAllEqualOrNothing();
            }
            set
            {
                foreach (var item in _parts)
                    item.Transform.Scale = new Vector2(item.Transform.Scale.X, value ?? item.Transform.Scale.Y);
            }
        }
        public string Name {
            get
            {
                return _parts?.Select(p => p.Name).GetAllEqualOrNothing();
            }
            set
            {
                foreach (var item in _parts)
                    item.Name = value;
            }
        }
        public string SpriteId
        {
            get
            {
                return _parts?.Select(p => p.SpriteId).GetAllEqualOrNothing();
            }
            set
            {
                foreach (var item in _parts)
                    item.SpriteId = value;
            }
        }
        public string Flags
        {
            get
            {
                return null;
                //return Parts.Select(p => p.Flags.Join(.GetAllEqualOrNothing();
            }
            set
            {
                //foreach (var item in Parts)
                //    item.Flags = value;
            }
        }
        public System.Windows.Media.Color? Color
        {
            get
            {
                return _parts?.Select(p => System.Windows.Media.Color.FromArgb(p.Color.A,p.Color.R,p.Color.G,p.Color.B)).GetAllEqualOrNothing();
            }
            set
            {
                if (value != null) {
                    System.Windows.Media.Color c = value.Value;
                foreach (var item in _parts)
                        item.Color = new Color(c.R, c.G, c.B, c.A);
                }
            }
        }
    }
}
