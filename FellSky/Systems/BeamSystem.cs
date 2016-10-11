using Artemis;
using FellSky.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using DiceNotation.Rollers;

namespace FellSky.Systems
{
    public class BeamSystem: Artemis.System.EntitySystem
    {
        private World _physicsWorld = null;
        private float _hitFraction;
        private Vector2 _hitPosition;
        private Entity _hitObject;
        private BeamHitEventArgs _beamHitEventArgs = new BeamHitEventArgs();
        private IDieRoller _dieRoller = new StandardDieRoller(new Random());

        public BeamSystem()
            : base(Aspect.All(typeof(BeamComponent)))
        { }

        public override void LoadContent()
        {
            if(EntityWorld.SystemManager.Systems.Any(s=>s is PhysicsSystem))
                _physicsWorld = EntityWorld.SystemManager.GetSystem<PhysicsSystem>()?.PhysicsWorld;
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            foreach(var beamEntity in entities.Values)
            {
                var beamComponent = beamEntity.GetComponent<BeamComponent>();
                TestBeamHit(beamEntity, beamComponent);
                AdjustIntensity(beamEntity, beamComponent);                
            }
        }

        private void TestBeamHit(Entity beamEntity, BeamComponent beamComponent)
        {
            if (_physicsWorld == null) return;
            var xform = beamEntity.GetComponent<Transform>();
            _hitFraction = 1;
            _hitObject = null;

            _physicsWorld.RayCast(
                RaycastCallback,
                xform.Position * Constants.PhysicsUnitScale, 
                (xform.Position + Utilities.CreateVector2FromAngle(xform.Rotation) * beamComponent.Beam.Range) * Constants.PhysicsUnitScale);

            if (_hitObject != null)
            {
                _beamHitEventArgs.Beam = beamEntity;
                _beamHitEventArgs.Damage = beamComponent.DamagePerSecond.Roll(_dieRoller).Value * EntityWorld.Delta * 0.001f;
                _beamHitEventArgs.HitFraction = _hitFraction;
                _beamHitEventArgs.HitObject = _hitObject;
                _beamHitEventArgs.Position = _hitPosition;

                beamEntity.FireEvent(this, EventId.BeamHit, _beamHitEventArgs);
                _hitObject.FireEvent(this, EventId.BeamHit, _beamHitEventArgs);
                EntityWorld.FireEvent(this, EventId.BeamHit, _beamHitEventArgs);
                beamComponent.Range = beamComponent.Beam.Range * _hitFraction;
            }
        }

        private float RaycastCallback(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
        {
            var objB = fixture.UserData as Entity;
            if (objB != null)
            {
                if(fraction < _hitFraction)
                {
                    _hitFraction = fraction;
                    _hitPosition = point;
                    _hitObject = objB;
                }
            }
            return -1;
        }

        private void AdjustIntensity(Entity beamEntity, BeamComponent beamComponent)
        {
            // adjust intensity
            if (beamComponent.Intensity >= 1 && beamComponent.Beam.Lifetime > 0)
            {
                beamComponent.Age += (EntityWorld.Delta / 1000f);
                if (beamComponent.Age > beamComponent.Beam.Lifetime)
                {
                    beamComponent.IsPowered = false;
                }
            }

            if (beamComponent.IsPowered)
            {
                if (beamComponent.Beam.IntensityFadeInTime > 0)
                {
                    beamComponent.Intensity += (EntityWorld.Delta / beamComponent.Beam.IntensityFadeInTime) / 1000f;
                    if (beamComponent.Intensity > 1)
                        beamComponent.Intensity = 1;
                }
                else
                {
                    beamComponent.Intensity = 1;
                }
            }
            else
            {
                if (beamComponent.Beam.IntensityFadeOutTime > 0)
                {
                    beamComponent.Intensity -= (EntityWorld.Delta / beamComponent.Beam.IntensityFadeOutTime) / 1000f;
                    if (beamComponent.Intensity < 0)
                        beamComponent.Intensity = 0;
                }
                else
                {
                    beamComponent.Intensity = 0;
                }

                if (beamComponent.Intensity <= 0)
                {
                    beamEntity.Delete();
                }
            }
        }
    }

    public class BeamHitEventArgs: EventArgs
    {
        public float Damage;
        public Vector2 Position;
        public Entity Beam;
        public Entity HitObject;
        public float HitFraction;
    }
}
