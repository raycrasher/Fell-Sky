using Artemis.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FellSky.Framework;
using Artemis;
using Microsoft.Xna.Framework.Graphics;
using FellSky.Components;
using Microsoft.Xna.Framework;
using FellSky.Services;

namespace FellSky.Systems
{
    public class ParticleSystem : EntitySystem
    {
        private class Particle: Transform
        {
            public SpriteComponent Sprite;
            public FloatColor Color, ColorDelta;
            public TimeSpan TimeToLive;
            public float RadialAcceleration;
            public float TangentialAcceleration;
            public Vector2 Velocity;
            public ParticleEmitterComponent Emitter;
            public float AngularVelocity;
            public float Size, SizeDelta;
        }

        private SpriteBatch _spriteBatch;

        private List<Particle> _particles = new List<Particle>();
        private ITimerService _time;

        public int MaxParticles { get; set; } = 10000;

        public ParticleSystem()
            : base(Aspect.All(typeof(ParticleEmitterComponent), typeof(SpriteComponent), typeof(Transform)))
        {
            _time = ServiceLocator.Instance.GetService<ITimerService>();
            _spriteBatch = ServiceLocator.Instance.GetService<SpriteBatch>();
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            var camera = EntityWorld.TagManager.GetEntity(Constants.ActiveCameraTag).GetComponent<Camera>();
            _spriteBatch.Begin();
            
            foreach(var entity in entities.Values)
            {
                var emitter = entity.GetComponent<ParticleEmitterComponent>();
                var xform = entity.GetComponent<Transform>();
                var sprite = entity.GetComponent<SpriteComponent>();
                EmitParticles(emitter, sprite, xform);
            }

            int offset = 0;
            for(int i = 0; i < _particles.Count; i++)
            {

                var particle = _particles[i];
                if (particle.TimeToLive <= TimeSpan.Zero)
                {
                    offset++;
                    continue;
                } else
                {
                    if(offset > 0)
                    {
                        _particles[i - offset] = particle;
                        _particles[i] = null;
                    }
                    UpdateParticle(particle);
                    particle.Sprite.Draw(_spriteBatch, particle, particle.Color);
                }
            }
            if(offset > 0)
                _particles.RemoveRange(_particles.Count - offset, offset);
            
            _spriteBatch.End();
        }

        private void EmitParticles(ParticleEmitterComponent emitter, SpriteComponent sprite, Transform xform)
        {
            var dTime = _time.DeltaTime.TotalSeconds;

            //cout << "PtclDRaw:"<< &spriteBatch.getTexture() << endl;
            if (!emitter.IsContinuous)
            {
                emitter.TimeToFire += _time.DeltaTime;
                if (emitter.TimeToFire >= emitter.TimeToFire && emitter.CurrentParticleCount <= 0)
                    return;               
            }

            // generate new particles
            if (emitter.IsFiring && (emitter.TimeToFire > TimeSpan.Zero || emitter.IsContinuous))
            {
                var fParticlesNeeded = emitter.EmissionRate * dTime + emitter.ParticlesLeft;
                var nParticlesCreated = (int)fParticlesNeeded;
                emitter.ParticlesLeft = fParticlesNeeded - nParticlesCreated;

                for (var i = 0; i < nParticlesCreated; i++)
                {
                    if (_particles.Count >= MaxParticles) break;

                    EmitParticle(emitter, sprite, xform);
                }
            }
            emitter.PreviousLocation = xform.Position;
        }

        private void UpdateParticle(Particle particle)
        {
            var dTime = (float)_time.DeltaTime.TotalSeconds;

            particle.TimeToLive -= _time.DeltaTime;

            var vecAccel = particle.Position;
            var vecAccel2 = vecAccel = Vector2.Normalize(vecAccel);

            vecAccel = vecAccel * particle.RadialAcceleration;

            var angle = vecAccel2.X;
            vecAccel2.X = -vecAccel2.Y;
            vecAccel2.Y = angle;
            vecAccel2 *= particle.TangentialAcceleration;

            particle.Velocity += (vecAccel + vecAccel2) * dTime;

            particle.Velocity += particle.Emitter.Gravity;

            particle.Rotation += particle.AngularVelocity * dTime;
            particle.Size += particle.SizeDelta * dTime;
            particle.Position += particle.Velocity * dTime;

            particle.Color.R += (particle.ColorDelta.R * dTime);
            particle.Color.G += (particle.ColorDelta.G * dTime);
            particle.Color.B += (particle.ColorDelta.B * dTime);
            particle.Color.A += (particle.ColorDelta.A * dTime);

            particle.Scale = Vector2.One * particle.Size;
        }

        private Random _rng = new Random();

        private void EmitParticle(ParticleEmitterComponent emitter, SpriteComponent sprite, Transform transform)
        {
            const float pi2 = (float)(Math.PI * Math.PI);

            var angle = emitter.Direction - pi2 + _rng.NextFloat(0, emitter.Spread) - emitter.Spread / 2.0f;
            if (emitter.IsRelative) angle += (emitter.PreviousLocation - transform.Position).GetAngleRadians() + pi2;

            var sr = emitter.ColorStart.R / 255.0f;
            var sg = emitter.ColorStart.G / 255.0f;
            var sb = emitter.ColorStart.B / 255.0f;
            var sa = emitter.ColorStart.A / 255.0f;
            var er = emitter.ColorEnd.R / 255.0f;
            var eg = emitter.ColorEnd.G / 255.0f;
            var eb = emitter.ColorEnd.B / 255.0f;
            var ea = emitter.ColorEnd.A / 255.0f;

            var offset = new Vector2();

            if (emitter.PositionVariance > 0) offset = new Vector2(_rng.NextFloat(0, emitter.PositionVariance), _rng.NextFloat(0, emitter.PositionVariance)) * emitter.EffectScale;

            var p = new Particle
            {
                Sprite = sprite,
                TimeToLive = TimeSpan.FromSeconds(_rng.NextFloat((float)emitter.ParticleLifeMin.TotalSeconds, (float)emitter.ParticleLifeMax.TotalSeconds)),
                Position = (emitter.IsRelative ? Vector2.Zero : transform.Position) + offset,
                Velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * _rng.NextFloat(emitter.SpeedMin, emitter.SpeedMax) * emitter.EffectScale,
                RadialAcceleration = _rng.NextFloat(emitter.RadialAccelMin, emitter.RadialAccelMax),
                TangentialAcceleration = _rng.NextFloat(emitter.TangentialAccelMin, emitter.TangentialAccelMax),
                Size = _rng.NextFloat(emitter.SizeStart, emitter.SizeStart + (emitter.SizeEnd - emitter.SizeStart) * emitter.SizeVar) * emitter.EffectScale,
                Rotation = _rng.NextFloat(emitter.RotationStart, emitter.RotationStart + (emitter.RotationEnd - emitter.RotationStart) * emitter.RotationVar),
                Color = new FloatColor
                {
                    R = _rng.NextFloat(sr, sr + ((er - sr) * emitter.ColorVar)),
                    G = _rng.NextFloat(sg, sg + ((eg - sg) * emitter.ColorVar)),
                    B = _rng.NextFloat(sb, sb + ((eb - sb) * emitter.ColorVar)),
                    A = _rng.NextFloat(sa, sa + ((ea - sa) * emitter.ColorVar))
                },
            };

            p.SizeDelta = (float)((emitter.SizeEnd * emitter.EffectScale - p.Size) / p.TimeToLive.TotalSeconds);
            p.AngularVelocity = (float)(_rng.NextFloat(emitter.AngularAccelMin, emitter.AngularAccelMax) / p.TimeToLive.TotalSeconds);

            p.ColorDelta = new FloatColor
            {
                R = (float)((emitter.ColorEnd.R / 255.0f - p.Color.R) / p.TimeToLive.TotalSeconds),
                G = (float)((emitter.ColorEnd.G / 255.0f - p.Color.G) / p.TimeToLive.TotalSeconds),
                B = (float)((emitter.ColorEnd.B / 255.0f - p.Color.B) / p.TimeToLive.TotalSeconds),
                A = (float)((emitter.ColorEnd.A / 255.0f - p.Color.A) / p.TimeToLive.TotalSeconds)
            };
            p.Emitter = emitter;
            emitter.CurrentParticleCount++;
            _particles.Add(p);
        }
    }
}
