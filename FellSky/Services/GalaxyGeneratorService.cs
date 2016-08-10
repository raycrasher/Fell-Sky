using FellSky.Game.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Services
{
    public class GalaxyGeneratorService
    {
        Random _rng = new Random();
        public virtual Galaxy CreateGalaxy(int initialStars = 50)
        {            
            var galaxy = new Galaxy();

            galaxy.StarSystems.Add(CreateSolSystem());

            return galaxy;
        }

        public virtual StarSystem CreateSolSystem()
        {
            
        }
    }
}
