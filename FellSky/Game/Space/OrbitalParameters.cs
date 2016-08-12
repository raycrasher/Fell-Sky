using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Game.Space
{
    /// <summary>
    /// Highly simplified 2d model of orbit
    /// </summary>
    public class OrbitalParameters
    {
        public double SemiMajorAxis { get; set; }
        public double Eccentricity { get; set; }
        public double MeanAnomalyAtEpoch { get; set; }
        public double LongitudeOfPeriapsis { get; set; }

        public OrbitalParameters(double semiMajorAxis, double eccentricity, double longitudeOfPeriapsis, double meanAnomalyAtEpoch)
        {
            SemiMajorAxis = semiMajorAxis;
            Eccentricity = eccentricity;
            MeanAnomalyAtEpoch = meanAnomalyAtEpoch;
            LongitudeOfPeriapsis = longitudeOfPeriapsis;
        }

        public static double GetGravitationalParameter(float mass1, float mass2)
            => Constants.GravitationalConstant * (mass1 + mass2);
        
        public static double GetMeanMotion(double gravParameter, double semiMajorAxis)
            => Math.Sqrt(gravParameter / (semiMajorAxis * semiMajorAxis * semiMajorAxis));

        public static double GetTrueAnomaly(double time, double meanMotion)
            => time * meanMotion;

        public Vector2 GetPositionAtTime(DateTime time)
        {
            var deltaTime = (time - Constants.EpochJ2000).Seconds;

            var apoapsis = (1 + Eccentricity) * SemiMajorAxis;
            var periapsis = (1 - Eccentricity) * SemiMajorAxis;
            var semiMinorAxis = Math.Sqrt(apoapsis * periapsis);

            var focus = Math.Sqrt((SemiMajorAxis * SemiMajorAxis) - (apoapsis * periapsis));

            var cosLongP = Math.Cos(LongitudeOfPeriapsis);
            var sinLongP = Math.Sin(LongitudeOfPeriapsis);

            var cosTime = Math.Cos(deltaTime);
            var sinTime = Math.Sin(deltaTime);

            var xc = focus * cosLongP;
            var yc = focus * sinLongP;

            var x = xc + SemiMajorAxis * cosTime * cosLongP - semiMinorAxis * sinTime * sinLongP;
            var y = yc + SemiMajorAxis * cosTime * sinLongP + semiMinorAxis * sinTime * cosLongP;

            return new Vector2((float)x, (float)y);
        }

        public OrbitalParameters() { }
    }
}
