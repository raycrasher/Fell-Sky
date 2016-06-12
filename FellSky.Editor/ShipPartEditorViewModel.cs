using Artemis;
using FellSky.Components;
using FellSky.Game.Ships.Parts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace FellSky.Editor
{
    [PropertyChanged.ImplementPropertyChanged]
    public class ShipPartEditorViewModel
    {
        private ObservableCollection<Entity> _parts;

        public ObservableCollection<Entity> Parts {
            get { return _parts; }
            set {
                if (_parts != null)
                    _parts.CollectionChanged -= OnCollectionChanged;
                _parts = value;
                if (_parts != null)
                    _parts.CollectionChanged += OnCollectionChanged;
                else
                {
                    HasItems = false;
                    ShowHullPanel = false;
                    ShowThrusterPanel = false;
                    ShowHardpointPanel = false;
                }
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs evt)
        {
            HasItems = _parts.Any();
            ShowHullPanel = _parts.Any(e => e.HasComponent<HullComponent>());
            ShowThrusterPanel = _parts.Any(e => e.HasComponent<ThrusterComponent>());
            ShowHardpointPanel = _parts.Any(e => e.HasComponent<HardpointComponent>());

            General = new ShipPartViewModel(_parts.Select(e => e.GetComponent<IShipPartComponent>().Part).ToList());
            if (ShowHardpointPanel)
                Hardpoints = new HardpointEditorViewModel(_parts.Select(e => e.GetComponent<HardpointComponent>()?.Hardpoint).Where(h => h != null).ToList());
            if (ShowHullPanel)
                Hulls = new HullEditorViewModel(_parts.Select(e => e.GetComponent<HullComponent>()?.Part).Where(h => h != null).ToList());
        }

        public ShipPartViewModel General { get; set; }
        public HardpointEditorViewModel Hardpoints { get; set; }
        public HullEditorViewModel Hulls { get; set; }

        public bool ShowThrusterPanel { get; set; } = true;
        public bool ShowHullPanel { get; set; } = true;
        public bool ShowHardpointPanel { get; set; } = true;
        public bool HasItems { get; set; } = true;

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
    
    [PropertyChanged.ImplementPropertyChanged]
    public class HardpointEditorViewModel
    {
        IReadOnlyCollection<Hardpoint> _hardpoints { get; set; }
        public HardpointEditorViewModel(IReadOnlyCollection<Hardpoint> hardpoints)
        {
            _hardpoints = hardpoints;
        }

        public HardpointSize? Size
        {
            get
            {
                return _hardpoints?.Select(p => new HardpointSize?(p.Size)).GetAllEqualOrNothing();
            }
            set
            {
                foreach (var item in _hardpoints)
                    item.Size = value ?? item.Size;
            }
        }

        public HardpointType? Type
        {
            get
            {
                return _hardpoints?.Select(p => new HardpointType?(p.Type)).GetAllEqualOrNothing();
            }
            set
            {
                foreach (var item in _hardpoints)
                    item.Type = value ?? item.Type;
            }
        }

        public float? FiringArc
        {
            get
            {
                return _hardpoints?.Select(p => new float?(p.FiringArc)).GetAllEqualOrNothing();
            }
            set
            {
                foreach (var item in _hardpoints)
                    item.FiringArc = value ?? item.FiringArc;
            }
        }
    }

    [PropertyChanged.ImplementPropertyChanged]
    public class HullEditorViewModel
    {
        IReadOnlyCollection<Hull> _hulls { get; set; }
        public HullEditorViewModel(IReadOnlyCollection<Hull> hulls)
        {
            _hulls = hulls;
        }

        public HullColorType? ColorType
        {
            get
            {
                return _hulls?.Select(p => new HullColorType?(p.ColorType)).GetAllEqualOrNothing();
            }
            set
            {
                foreach (var item in _hulls)
                    item.ColorType = value ?? item.ColorType;
            }
        }
    }
}
