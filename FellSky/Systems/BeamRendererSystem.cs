using Artemis;
using FellSky.Components;
using FellSky.Framework;
using FellSky.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems
{
    public class BeamRendererSystem: Artemis.System.EntitySystem
    {
        private SpriteBatch _batch;
        private GraphicsDevice _device;
        private Transform _originXform = new Transform();

        public BeamRendererSystem()
            : base(Aspect.All(typeof(Transform), typeof(BeamComponent)))
        {
            _device = ServiceLocator.Instance.GetService<GraphicsDevice>();
            _batch = ServiceLocator.Instance.GetService<SpriteBatch>();
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            var camera = EntityWorld.GetActiveCamera();
            
            _batch.Begin(transformMatrix: camera.GetViewMatrix(1.0f));
            
            foreach(var item in entities.Values)
            {
                var beam = item.GetComponent<BeamComponent>();
                var sprite = item.GetComponent<SpriteComponent>();
                Matrix matrix;
                beam.Origin.GetWorldMatrix(out matrix);
                _originXform.CopyValuesFrom(ref matrix);
                _originXform.Origin = new Vector2(0, (sprite.TextureRect.Height * beam.Scale.Y) / 2);
                int numSections = (int) Math.Ceiling(beam.Range / sprite.TextureRect.Width * beam.Scale.X);
                var offset = Utilities.CreateVector2FromAngle(_originXform.Rotation) * beam.Scale.X;
                for(int i = 0; i < numSections; i++)
                {
                    sprite.Draw(_batch, _originXform, new Color(0,0,255,128));
                    _originXform.Position += offset;
                }
            }
            _batch.End();
        }
    }
}
