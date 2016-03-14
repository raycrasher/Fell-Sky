using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Models.Ships.Modules
{
    public class WarpCoreModule: ShipModule<WarpCoreModule.Upgrades>, IShipModule
    {
        public enum Upgrades
        {
            ContainmentLv1,
            ContainmentLv2,
            ContainmentLv3,

            SizeLv1,
            SizeLv2,
            SizeLv3,

            StrengthLv1,
            StrengthLv2,
            StrengthLv3,

            ControlLv1,
            ControlLv2,
            ControlLv3,
        };

        public int Strength =>      CalculateUpgradeLevel(Upgrades.StrengthLv1,      Upgrades.StrengthLv2,       Upgrades.StrengthLv3);
        public int Size =>          CalculateUpgradeLevel(Upgrades.SizeLv1,          Upgrades.SizeLv2,           Upgrades.SizeLv2);
        public int Control =>       CalculateUpgradeLevel(Upgrades.ControlLv1,       Upgrades.ControlLv2,        Upgrades.ControlLv3);
        public int Containment =>   CalculateUpgradeLevel(Upgrades.ContainmentLv1,   Upgrades.ContainmentLv2,    Upgrades.ContainmentLv3);

        public WarpCoreModule()
        {
            ModuleName = "Warp Core";
        }

        protected override IEnumerable<Upgrades> GetAvailableUpgrades() => (new[] {
            GetNextLevelledUpgrade(Strength, Upgrades.StrengthLv1, Upgrades.StrengthLv2, Upgrades.StrengthLv3),
            GetNextLevelledUpgrade(Size, Upgrades.SizeLv1, Upgrades.SizeLv2, Upgrades.SizeLv3),
            GetNextLevelledUpgrade(Control, Upgrades.ControlLv1, Upgrades.ControlLv2, Upgrades.ControlLv3),
            GetNextLevelledUpgrade(Containment, Upgrades.ContainmentLv1, Upgrades.ContainmentLv2, Upgrades.ContainmentLv3)
        }).Where(u => u != null).Cast<Upgrades>();
    }
}
