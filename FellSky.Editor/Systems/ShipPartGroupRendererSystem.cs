using Artemis;
using FellSky.Components;
using FellSky.Services;
using FellSky.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Editor.Systems
{
    public class ShipPartGroupRendererSystem: Artemis.System.EntitySystem
    {
        private Matrix _matrix;
        private SpriteBatch _spriteBatch;

        public ShipPartGroupRendererSystem()
            : base(Aspect.All(typeof(ShipPartGroupComponent), typeof(Transform)))
        {
            _spriteBatch = ServiceLocator.Instance.GetService<SpriteBatch>();
        }



        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            var camera = EntityWorld.TagManager.GetEntity(Constants.ActiveCameraTag).GetComponent<Camera>();
            if (camera == null) return;
            _matrix = camera.GetViewMatrix(1.0f);
            _spriteBatch.Begin(SpriteSortMode.Deferred, blendState: BlendState.AlphaBlend, transformMatrix: _matrix, samplerState: SamplerState.AnisotropicClamp);

            foreach (var entity in entities.Values)
            {
                ShipRendererSystem.DrawShip(_spriteBatch, entity);
            }

            _spriteBatch.End();
        }
    }
}
