using Artemis;
using FellSky.Components;
using FellSky.Framework;
using FellSky.Game.Ships.Parts;
using FellSky.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems
{
    public class HardpointRendererSystem: Artemis.System.EntitySystem
    {
        private PrimitiveBatch _batch;

        public HardpointRendererSystem()
            : base(Aspect.All(typeof(HardpointArcDrawingComponent), typeof(HardpointComponent)))
        {
        }

        public static float NumSegments = 60;
        public static Dictionary<HardpointType, Color> Colors = new Dictionary<HardpointType, Color>
        {
            { HardpointType.Weapon_Universal, Color.LightSlateGray * 0.4f },
            { HardpointType.Weapon_Missile, Color.Green * 0.4f },
            { HardpointType.Weapon_Ballistic, Color.PaleGoldenrod * 0.4f },
        };

        public static Dictionary<HardpointSize, float> Lengths = new Dictionary<HardpointSize, float>
        {
            { HardpointSize.Small, 100 },
            { HardpointSize.Medium, 250 },
            { HardpointSize.Large, 400 },
            { HardpointSize.Huge, 650 },
        };

        public override void LoadContent()
        {
            _batch = new PrimitiveBatch(ServiceLocator.Instance.GetService<GraphicsDevice>());
        }
        public override void UnloadContent()
        {
            _batch.Dispose();
            _batch = null;
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            var camera = EntityWorld.GetActiveCamera();
            var projection = camera.ProjectionMatrix;
            var view = camera.GetViewMatrix(1.0f);
            _batch.Begin(ref projection, ref view);

            foreach(var entity in entities.Values)
            {
                var hardpoint = entity.GetComponent<HardpointComponent>();
                var draw = entity.GetComponent<HardpointArcDrawingComponent>();
                var hull = entity.GetComponent<HullComponent>();
                var xform = entity.GetComponent<LocalTransformComponent>().ParentWorldMatrix;
                Vector2 hullPos, offset;
                var tmp = hull.Part.Transform.Position;

                Vector2.Transform(ref tmp, ref xform, out hullPos);
                tmp = new Vector2(1,0);
                Vector2.Transform(ref tmp, ref xform, out tmp);
                var rot = hull.Part.Transform.Rotation;
                rot = hull.Part.Transform.Rotation + tmp.ToAngleRadians();

                float beginAngle = MathHelper.WrapAngle(rot - hardpoint.Hardpoint.FiringArc / 2) + MathHelper.TwoPi;
                float endAngle = MathHelper.WrapAngle(rot + hardpoint.Hardpoint.FiringArc / 2) + MathHelper.TwoPi;

                var step = MathHelper.TwoPi / NumSegments;
                if (beginAngle > endAngle)
                {
                    beginAngle -= MathHelper.TwoPi;
                }

                if(hardpoint.Hardpoint.FiringArc >= MathHelper.TwoPi)
                {
                    beginAngle = 0;
                    endAngle = MathHelper.TwoPi;
                }
                Color color = Color.White * 0.4f;
                float length = 50;
                Colors.TryGetValue(hardpoint.Hardpoint.Type, out color);
                Lengths.TryGetValue(hardpoint.Hardpoint.Size, out length);

                
                offset = Utilities.CreateVector2FromAngle(beginAngle) * length;

                _batch.AddVertex(hullPos, color, PrimitiveType.LineList);
                _batch.AddVertex(hullPos + offset, color, PrimitiveType.LineList);

                
                float angle;
                for(angle = beginAngle; angle < endAngle; angle += step)
                {
                    _batch.AddVertex(hullPos, color * 0f, PrimitiveType.TriangleList);
                    _batch.AddVertex(hullPos + Utilities.CreateVector2FromAngle(angle) * length, color, PrimitiveType.TriangleList);
                    var angle2 = angle + step < endAngle ? angle + step : endAngle;
                    _batch.AddVertex(hullPos + Utilities.CreateVector2FromAngle(angle2) * length, color, PrimitiveType.TriangleList);
                }

                offset = Utilities.CreateVector2FromAngle(endAngle) * length;

                _batch.AddVertex(hullPos, color, PrimitiveType.LineList);
                _batch.AddVertex(hullPos + offset, color, PrimitiveType.LineList);

                if (draw.DrawHardpointIcon)
                {
                    // draw square
                }
            }

            _batch.End();
        }
        
    }
}
