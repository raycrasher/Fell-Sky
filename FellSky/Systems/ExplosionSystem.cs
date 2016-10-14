using Artemis;
using Artemis.Manager;
using FellSky.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems
{
    public class ExplosionSystem: Artemis.System.EntitySystem
    {
    }

    public class ExplosionRenderer : Artemis.System.EntitySystem
    {
        public ExplosionRenderer()
            : base(Aspect.All(typeof(ExplosionComponent)))
        {
        }

        private void DrawFireball(Entity entity, ExplosionComponent explosion)
        {

        }

        private void DrawBlastWave(Entity entity, ExplosionComponent explosion)
        {

        }

        private void DoScreenShake(Entity entity, ExplosionComponent explosion)
        {

        }
    }
}