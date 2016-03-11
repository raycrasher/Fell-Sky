using System;
using System.Collections.Generic;
using System.Linq;

namespace FellSky.Models.Ships.Modules
{
    public interface IShipModule
    {
        IEnumerable<string> AvailableUpgrades { get; }
        IEnumerable<string> InstalledUpgrades { get; }

        string ModuleName { get; }

        bool InstallUpgrade(string upgrade);
    }

    public abstract class ShipModule<TUpgrade> : ShipModule<TUpgrade, HashSet<TUpgrade>>
        where TUpgrade : struct
    {
    }

    public abstract class ShipModule<TUpgrade, TUpgradeCollectionType> : IShipModule
        where TUpgrade : struct
        where TUpgradeCollectionType : ICollection<TUpgrade>, new()
    {
        private TUpgradeCollectionType _upgrades = new TUpgradeCollectionType();
        public virtual IEnumerable<TUpgrade> AvailableUpgrades => GetAvailableUpgrades();
        public virtual TUpgradeCollectionType InstalledUpgrades => _upgrades;
        public string ModuleName { get; set; }

        IEnumerable<string> IShipModule.InstalledUpgrades => InstalledUpgrades.Select(s => s.ToString());
        IEnumerable<string> IShipModule.AvailableUpgrades => AvailableUpgrades.Select(s => s.ToString());


        public static int CalculateUpgradeLevel(ICollection<TUpgrade> upgradeCollection, params TUpgrade[] upgrades)
        {
            int level;
            for (level = upgrades.Length; level > 0; level--)
            {
                if (upgradeCollection.Contains(upgrades[level - 1])) return level;
            }
            return level;
        }

        public static TUpgrade? GetNextLevelledUpgrade(ICollection<TUpgrade> upgradeCollection, int level, params TUpgrade[] upgrades)
        {
            if (upgrades.Length <= 0) return null;
            if (level >= upgrades.Length) return null;
            if (level <= 0) return upgrades[0];
            return upgrades[level];
        }

        public virtual bool InstallUpgrade(TUpgrade upgrade)
        {
            if (!AvailableUpgrades.Contains(upgrade)) return false;
            InstalledUpgrades.Add(upgrade);
            return true;
        }

        bool IShipModule.InstallUpgrade(string upgrade) => InstallUpgrade(AvailableUpgrades, upgrade, InstallUpgrade);

        protected int CalculateUpgradeLevel(params TUpgrade[] upgrades) => CalculateUpgradeLevel(InstalledUpgrades, upgrades);

        protected virtual IEnumerable<TUpgrade> GetAvailableUpgrades()
        {
            yield break;
        }
        protected TUpgrade? GetNextLevelledUpgrade(int level, params TUpgrade[] upgrades) => GetNextLevelledUpgrade(InstalledUpgrades, level, upgrades);

        protected bool InstallUpgrade(IEnumerable<TUpgrade> availableUpgrades, string upgrade, Func<TUpgrade, bool> upgradeFunc)
        {
            TUpgrade value;
            if (Enum.TryParse(upgrade, out value))
            {
                return upgradeFunc(value);
            }
            return false;
        }
    }
}