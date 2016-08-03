using Artemis;
using FellSky.Components;
using FellSky.Game.Ships.Parts;
using FellSky.Services;
using Microsoft.Xna.Framework;
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

        public virtual Entity Install(EntityWorld world, Entity shipEntity, Entity hardpointEntity)
        {
            var slot = hardpointEntity.GetComponent<HardpointComponent>();
            if (!hardpointEntity.IsChildOf(shipEntity)) throw new InvalidOperationException("Cannot install, ship entity does not own hardpoint entity.");
            if (slot == null) throw new InvalidOperationException("Cannot install on non-hardpoint entity.");
            var shipComponent = shipEntity.GetComponent<ShipComponent>();
            var moduleEntity = world.CreateEntity();

            var hardpointTransform = hardpointEntity.GetComponent<Transform>();
            var moduleTransform = new Transform(Vector2.Zero, 0, Vector2.One, -hardpointTransform.Origin);

            var scale = hardpointTransform.Scale;
            var origin = -hardpointTransform.Origin;
            if (scale.X < 0)
            {
                scale.X = Math.Abs(scale.X);
                origin.X *= -1;
            }
            if (scale.Y < 0)
            {
                scale.Y *= Math.Abs(scale.Y);
                origin.Y *= -1;
            }
            moduleTransform.Scale = scale;
            moduleTransform.Origin = origin;

            moduleEntity.AddComponent(moduleTransform);

            hardpointEntity.AddChild(moduleEntity);
            var previousInstalled = slot.InstalledEntity;
            if (previousInstalled != null)
            {
                previousInstalled.GetComponent<ModuleComponent>().Module.Uninstall(shipEntity, previousInstalled);
            }

            if (!string.IsNullOrWhiteSpace(PartGroupId))
            {
                EntityFactories.ShipEntityFactory.CreateShipModel(PartGroupId).CreateChildEntities(world, moduleEntity);
            }

            var moduleComponent = new ModuleComponent
            {
                HardpointEntity = hardpointEntity,
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