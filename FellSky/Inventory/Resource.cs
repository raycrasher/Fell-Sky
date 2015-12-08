using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Inventory
{
    public class Resource
    {
        public static List<Resource> Resources { get; } = new List<Resource>();

        public string IconSpriteId { get; set; }
        public string Name { get; set; }
        public string Id { get; private set; }
        public static Resource FissionFuel { get; private set; }

        public static Resource Get(string id)
        {
            return Resources.First(r => r.Id == id);
        }

        static Resource()
        {
            Resources.Add(ElectricCharge);
            Resources.Add(Heat);
            Resources.Add(Air);
            Resources.Add(Food);
            Resources.Add(Water);

            Resources.Add(new Resource { Id = "medsupplies", Name = "medical supplies" });
            Resources.Add(new Resource { Id = "repairsupplies", Name = "repair supplies" });
            Resources.Add(new Resource { Id = "constructionsupplies", Name = "construction supplies" });
            Resources.Add(new Resource { Id = "metalore", Name = "metal ore" });
            Resources.Add(new Resource { Id = "fissionableore", Name = "fissionable ore" });
            Resources.Add(new Resource { Id = "hydrocarbons", Name = "hydrocarbons" });
            Resources.Add(new Resource { Id = "biomass", Name = "biomass" });
            Resources.Add(new Resource { Id = "antimatter", Name = "antimatter" });
            Resources.Add(new Resource { Id = "fusionfuel", Name = "fusion fuel" });
            Resources.Add(new Resource { Id = "exoticmatter", Name = "exotic matter" });
            
            //Goods
        }


        //built in resources
        public static readonly Resource ElectricCharge = new Resource { Id = "charge", Name = "electric charge" };
        public static readonly Resource Heat = new Resource { Id = "heat", Name = "heat" };
        public static readonly Resource Air = new Resource { Id = "air", Name = "air" };
        public static readonly Resource Food = new Resource { Id = "food", Name = "food" };
        public static readonly Resource Water = new Resource { Id = "water", Name = "water" };
        public static readonly Resource ChargedParticles = new Resource { Id = "chargedparticles", Name = "charged particles" };
        public static readonly Resource Biomass = new Resource { Id = "biomass", Name = "biomass" };
    }
}
