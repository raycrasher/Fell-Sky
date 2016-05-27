using Artemis.Interface;
using FellSky.Framework;
using FellSky.Game.Ships.Modules;
using FellSky.Game.Ships.Parts;
using FellSky.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace FellSky.Game.Ships
{
    public class Ship: IShipEditorEditableModel
    {
        public string GivenName { get; set; }
        public string HullClass { get; set; }
        public string ShipType { get; set; }

        public List<ShipPart> Parts { get; set; } = new List<ShipPart>();
        public List<Hardpoint> Hardpoints { get; set; } = new List<Hardpoint>();
        public List<ModuleSlot> ModuleSlots { get; set; } = new List<ModuleSlot>();
        public List<Radiator> Radiators { get; set; } = new List<Radiator>();

        public List<ShipPartGroup> PartGroups { get; set; } = new List<ShipPartGroup>();

        IList<ShipPart> IShipEditorEditableModel.Parts => Parts;

        [ExpandableObject]
        public ShipHandlingParameters Handling { get; set; } = new ShipHandlingParameters();

        public Color BaseDecalColor { get; set; } = Color.White;
        public Color TrimDecalColor { get; set; } = Color.White;
        
        public void SaveToJsonFile(string filename)
        {
            System.IO.File.WriteAllText( filename, JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            }));
        }

        public static Ship LoadFromJsonFile(string filename)
        {
            return JsonConvert.DeserializeObject<Ship>(System.IO.File.ReadAllText(filename), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            });
        }

        public FloatRect CalculateBoundingBox(float thrusterXScale = 0.3f)
        {
            var sprites = ServiceLocator.Instance.GetService<ISpriteManagerService>().Sprites;
            return Parts.Concat(PartGroups.SelectMany(p => p.Parts))
                .Aggregate(new FloatRect(), (box, part) =>
                {
                    Sprite spr;
                    if (!sprites.TryGetValue(part.SpriteId, out spr)) return box;

                    var aabb = new FloatRect (-(spr.OriginX ?? spr.W / 2), -(spr.OriginY ?? spr.H / 2), spr.W, spr.H);
                    
                    var topleft = new Vector2(Math.Min(aabb.Left, aabb.Right), Math.Min(aabb.Top, aabb.Bottom));
                    var bottomright = new Vector2(Math.Max(aabb.Left, aabb.Right), Math.Max(aabb.Top, aabb.Bottom));

                    var matrix = part.Transform.Matrix;
                    if(part is Thruster)
                    {
                        matrix = Matrix.CreateScale(new Vector3(thrusterXScale, 1, 1)) * matrix;
                    }

                    Vector2.Transform(ref topleft, ref matrix, out topleft);
                    Vector2.Transform(ref bottomright, ref matrix, out topleft);

                    return new FloatRect(new Vector2(Math.Min(Math.Min(topleft.X, bottomright.X), box.Left), Math.Min(Math.Min(topleft.Y, bottomright.Y), box.Top)),
                        new Vector2(Math.Max(Math.Abs(topleft.X - bottomright.X), box.Width), Math.Max(Math.Abs(topleft.Y - bottomright.Y), box.Width)));
                });
            
        }
    }
}
