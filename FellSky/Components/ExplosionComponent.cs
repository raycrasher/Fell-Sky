using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class ExplosionComponent: IComponent
    {
        /// <summary>
        /// Add blast wave effect
        /// </summary>
        public bool BlastWave { get; set; } 
        public float BlastWaveSize { get; set; }
        public int BlastWaveDurationMs { get; set; }
        public Color BlastWaveColor { get; set; }
        public SpriteComponent BlastWaveSprite { get; set; }

        public bool Fireball { get; set; }
        public float FireballStartSize { get; set; }
        public float FireballEndSize { get; set; }
        public int FireballDurationMs { get; set; }
        public Color FireballColor { get; set; }
        public SpriteComponent FireballSprite { get; set; }

        public bool Smoke { get; set; }
        public int SmokeNumParticles { get; set; }
        public int SmokeDuration { get; set; }
        public SpriteComponent SmokeSprite { get; set; }

        public float ScreenShake { get; set; }
    }
}
