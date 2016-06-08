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
        private PrimitiveBatch _primitiveBatch;

        public HardpointRendererSystem()
            : base(Aspect.All(typeof(HardpointArcDrawingComponent), typeof(HardpointComponent), typeof(HullComponent)))
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
            { HardpointSize.Medium, 200 },
            { HardpointSize.Large, 300 },
            { HardpointSize.Huge, 400 },
        };
        private ISpriteManagerService _sprites;
        private SpriteBatch _spriteBatch;

        public override void LoadContent()
        {
            _primitiveBatch = new PrimitiveBatch(ServiceLocator.Instance.GetService<GraphicsDevice>());
            _spriteBatch = ServiceLocator.Instance.GetService<SpriteBatch>();
            _sprites = ServiceLocator.Instance.GetService<ISpriteManagerService>();
        }
        public override void UnloadContent()
        {
            _primitiveBatch.Dispose();
            _primitiveBatch = null;
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            var camera = EntityWorld.GetActiveCamera();
            var projection = camera.ProjectionMatrix;
            var view =  camera.GetViewMatrix(1.0f);
            
            _primitiveBatch.Begin(ref projection, ref view);
            _spriteBatch.Begin(transformMatrix: view);
            foreach(var entity in entities.Values)
            {
                var hardpoint = entity.GetComponent<HardpointComponent>();
                var draw = entity.GetComponent<HardpointArcDrawingComponent>();
                var hull = entity.GetComponent<HullComponent>();
                Matrix parentMatrix = Matrix.Identity;
                entity.GetParent()?.GetWorldMatrix(out parentMatrix);
                var xform = Matrix.CreateTranslation(new Vector3(hull.Part.Transform.Origin, 0)) * hull.Part.Transform.Matrix * parentMatrix;
                Vector2 lineOffset;

                Color color = Color.White * 0.4f;
                float length = 50;
                Colors.TryGetValue(hardpoint.Hardpoint.Type, out color);
                Lengths.TryGetValue(hardpoint.Hardpoint.Size, out length);

                color *= draw.Alpha;

                int sign = ((hull.Part.Transform.Scale.X < 0) ^ (hull.Part.Transform.Scale.Y < 0)) ? -1 : 1;

                var step = MathHelper.Pi / NumSegments;
                float endAngle = MathHelper.WrapAngle(hardpoint.Hardpoint.FiringArc / 2);
                float angle, angle2;



                for(angle = 0; angle < endAngle; angle += step)
                {
                    angle2 = angle + step < endAngle ? angle + step : endAngle;

                    AddVertex(Vector2.Zero, color * 0f, PrimitiveType.TriangleList, ref xform);
                    AddVertex(Utilities.CreateVector2FromAngle(angle * sign) * length, color, PrimitiveType.TriangleList, ref xform);
                    AddVertex(Utilities.CreateVector2FromAngle(angle2 * sign) * length, color, PrimitiveType.TriangleList, ref xform);

                    AddVertex(Vector2.Zero, color * 0f, PrimitiveType.TriangleList, ref xform);
                    AddVertex(Utilities.CreateVector2FromAngle(-angle2 * sign) * length, color, PrimitiveType.TriangleList, ref xform);
                    AddVertex(Utilities.CreateVector2FromAngle(-angle * sign) * length, color, PrimitiveType.TriangleList, ref xform);
                    

                }

                lineOffset = Utilities.CreateVector2FromAngle(endAngle * sign) * length;

                AddVertex(Vector2.Zero, color, PrimitiveType.LineList, ref xform);
                AddVertex(lineOffset, color, PrimitiveType.LineList, ref xform);

                lineOffset = Utilities.CreateVector2FromAngle(-endAngle * sign) * length;

                AddVertex(Vector2.Zero, color, PrimitiveType.LineList, ref xform);
                AddVertex(lineOffset, color, PrimitiveType.LineList, ref xform);

                if (draw.DrawHardpointIcon)
                {
                    bool drawCircle = 
                        hardpoint.Hardpoint.Type == HardpointType.Weapon_Energy ||
                        hardpoint.Hardpoint.Type == HardpointType.Weapon_Powered ||
                        hardpoint.Hardpoint.Type == HardpointType.Weapon_Beam ||
                        hardpoint.Hardpoint.Type == HardpointType.Weapon_Hybrid ||
                        hardpoint.Hardpoint.Type == HardpointType.Weapon_Universal;

                    bool drawRectangle = 
                        hardpoint.Hardpoint.Type == HardpointType.Weapon_Ballistic ||
                        hardpoint.Hardpoint.Type == HardpointType.Weapon_Powered ||
                        hardpoint.Hardpoint.Type == HardpointType.Weapon_Composite ||
                        hardpoint.Hardpoint.Type == HardpointType.Weapon_Universal;

                    bool drawDiamond =
                        hardpoint.Hardpoint.Type == HardpointType.Weapon_Missile ||
                        hardpoint.Hardpoint.Type == HardpointType.Weapon_Composite ||
                        hardpoint.Hardpoint.Type == HardpointType.Weapon_Hybrid ||
                        hardpoint.Hardpoint.Type == HardpointType.Weapon_VLS ||
                        hardpoint.Hardpoint.Type == HardpointType.Weapon_Universal;

                    float iconSize = length / 10;

                    color = Color.LightCyan;
                    var hullPos = Vector2.Transform(Vector2.Zero, xform);

                    if (drawCircle) {
                        _spriteBatch.DrawCircle(hullPos, iconSize, 20, color, draw.Thickness);
                    }

                    if (drawRectangle)
                    {
                        _spriteBatch.DrawRectangle(hullPos + new Vector2(-iconSize, -iconSize) - Vector2.One, new Vector2(iconSize * 2), color, draw.Thickness);

                    }
                    if (drawDiamond)
                    {
                        _spriteBatch.DrawLine(hullPos + new Vector2(-iconSize,0), hullPos + new Vector2(0, -iconSize), color, draw.Thickness);
                        _spriteBatch.DrawLine(hullPos + new Vector2(0, -iconSize), hullPos + new Vector2(iconSize,0), color, draw.Thickness);
                        _spriteBatch.DrawLine(hullPos + new Vector2(iconSize,0), hullPos + new Vector2(0, iconSize), color, draw.Thickness);
                        _spriteBatch.DrawLine(hullPos + new Vector2(0, iconSize), hullPos + new Vector2(-iconSize,0), color, draw.Thickness);
                    }
                }
            }

            _primitiveBatch.End();
            _spriteBatch.End();
        }

        public static FloatRect GetIconBoundingBox(Hardpoint hardpoint)
        {
            float size = Lengths[hardpoint.Size] / 10;
            return new FloatRect(Vector2.Zero, new Vector2(size*2));
        }

        private void AddVertex(Vector2 vertex, Color color, PrimitiveType type, ref Matrix xform)
        {
            Vector2.Transform(ref vertex, ref xform, out vertex);
            _primitiveBatch.AddVertex(vertex, color, type);
        }
    }
}
