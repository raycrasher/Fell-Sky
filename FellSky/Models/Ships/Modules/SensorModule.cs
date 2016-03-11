using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Models.Ships.Modules
{
    public class SensorModule: ShipModule<SensorModule.Upgrades>
    {
        public enum Upgrades
        {
            RangeLv1,
            RangeLv2,
            RangeLv3,

            EmissionReduction,
            StealthSensor,
            GravitySensor,
            WarpSensor,
            BioSensor
        }

        public int Range => CalculateUpgradeLevel(Upgrades.RangeLv1, Upgrades.RangeLv2, Upgrades.RangeLv3);

        protected override IEnumerable<Upgrades> GetAvailableUpgrades()
        {
            Upgrades? value;
            value = GetNextLevelledUpgrade(Range, Upgrades.RangeLv1, Upgrades.RangeLv2, Upgrades.RangeLv3);
            if (value != null) yield return (Upgrades)value;
            if (!InstalledUpgrades.Contains(Upgrades.EmissionReduction)) yield return Upgrades.EmissionReduction;
            if (!InstalledUpgrades.Contains(Upgrades.StealthSensor)) yield return Upgrades.StealthSensor;
            if (!InstalledUpgrades.Contains(Upgrades.GravitySensor)) yield return Upgrades.GravitySensor;
            if (!InstalledUpgrades.Contains(Upgrades.WarpSensor)) yield return Upgrades.WarpSensor;
            if (!InstalledUpgrades.Contains(Upgrades.BioSensor)) yield return Upgrades.BioSensor;
        }

    }
}
