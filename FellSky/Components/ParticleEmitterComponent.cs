using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FellSky.Components
{
    public class ParticleEmitterComponent: IComponent
    {
        public bool IsFiring { get; set; } = true;
        public int EmissionRate { get; set; } = 0;
        public bool IsContinuous { get; set; } = true;

        public float PositionVariance { get; set; }

        public TimeSpan ParticleLifeMin { get; set; } = TimeSpan.FromSeconds(2f);
        public TimeSpan ParticleLifeMax { get; set; } = TimeSpan.FromSeconds(3f);
        public Vector2 Gravity { get; set; }

        public float Direction { get; set; }
        public float DirectionVar { get; set; }
        public float Spread { get; set; } = 2 * MathHelper.Pi;

        public float SpeedMin { get; set; } = 20;
        public float SpeedMax { get; set; } = 30;

        public float AngularAccelMin { get; set; }
        public float AngularAccelMax { get; set; }

        public float RadialAccelMin { get; set; }
        public float RadialAccelMax { get; set; }

        public float TangentialAccelMin { get; set; }
        public float TangentialAccelMax { get; set; }

        public float SizeStart { get; set; } = 1;
        public float SizeEnd { get; set; } = 1.5f;
        public float SizeVar { get; set; }

        public float RotationStart { get; set; }
        public float RotationEnd { get; set; }
        public float RotationVar { get; set; }

        public Color ColorStart { get; set; } = Color.White;
        public Color ColorEnd { get; set; } = new Color(255, 255, 255, 0);
        public float ColorVar { get; set; }
        public float AlphaVar { get; set; }
        public float EffectScale { get; set; }
        public bool IsRelative { get; set; }


        public Vector2 PreviousLocation { get; set; }
        public TimeSpan TimeToFire { get; set; }
        public int CurrentParticleCount { get; set; }
        public double ParticlesLeft { get; set; }
    }
}
