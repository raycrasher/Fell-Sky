using Artemis;
using FellSky.Components;
using FellSky.Game.Ships.Parts;
using FellSky.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FellSky.Game.Ships.Modules
{
    public interface IModuleUpgrade
    {
        string Name { get; }
        string Description { get; }
    }

    public abstract class Module
    {
        public string Id { get; set; }
        public string ModuleName { get; set; }

        public HardpointSize Size { get; set; }
        public string PartGroupId { get; set; }

        public virtual bool CanInstall(Entity shipEntity, Hardpoint slot)
        {
            return Size == slot.Size && slot.Type == HardpointType.Module;
        }

        public virtual bool CanInstallUpgrade(Entity shipEntity, Entity moduleEntity, IModuleUpgrade upgrade) => false;

        public virtual bool CanMountToHardpoint(HardpointType type) => false;

        public virtual Entity Install(EntityWorld world, Entity shipEntity, Hardpoint slot)
        {
            var shipComponent = shipEntity.GetComponent<ShipComponent>();
            var moduleEntity = world.CreateEntity();
            moduleEntity.AddComponent(new Transform());
            var hpEntity = shipComponent.Hardpoints[slot];
            hpEntity.AddChild(moduleEntity);
            var previousInstalled = shipComponent.GetInstalledEntityForHardpoint(slot);
            if (previousInstalled != null)
            {
                previousInstalled.GetComponent<ModuleComponent>().Module.Uninstall(shipEntity, previousInstalled);
            }

            if (!string.IsNullOrWhiteSpace(PartGroupId))
            {
                EntityFactories.ShipEntityFactory.CreatePartGroup(PartGroupId).CreateEntities(world, moduleEntity);
            }

            var moduleComponent = new ModuleComponent
            {
                HardpointEntity = shipComponent.Hardpoints[slot],
                Module = this
            };
            moduleEntity.AddComponent(moduleComponent);
            return moduleEntity;
        }

        public virtual void InstallMod(Entity moduleEntity, IModuleUpgrade upgrade)
        {
            var component = moduleEntity.GetComponent<ModuleComponent>();
            component.InstalledMods.Add(upgrade);
        }

        public virtual void Uninstall(Entity shipEntity, Entity moduleEntity)
        {
            var component = moduleEntity.GetComponent<ModuleComponent>();
            component.HardpointEntity.GetComponent<HardpointComponent>().InstalledEntity = null;
            component.HardpointEntity.RemoveChild(moduleEntity);
        }

        public virtual void UninstallMod(Entity moduleEntity, IModuleUpgrade upgrade)
        {
            var component = moduleEntity.GetComponent<ModuleComponent>();
            component.InstalledMods.Remove(upgrade);
        }
    }
}