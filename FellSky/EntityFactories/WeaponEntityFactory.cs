using Artemis;
using Artemis.Interface;
using FellSky.Components;
using FellSky.Game.Combat.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.EntityFactories
{
    public static class WeaponEntityFactory
    {

    }

    class MachineGunWeaponFactory
    {
        class MachineGunComponent : IComponent
        {
            public Entity[][] BarrelEntities;
        };

        public Entity CreateWeapon(EntityWorld world, Entity[] parts, WeaponData data)
        {
            var weaponEntity = world.CreateEntity();
            var machineGunComponent = new MachineGunComponent();

            weaponEntity.AddComponent(machineGunComponent);
            machineGunComponent.BarrelEntities = Enumerable.Range(0,data.NumBarrels)
                .Select(i=>parts.Where(p => p.Components.OfType<IShipPartComponent>().First().Part.Name == $"Barrel{i}").ToArray())
                .ToArray();

            weaponEntity.AddComponent(new WeaponComponent
            {
                Data = data,
                Fire = () => FireMachineGunProjectile(world, weaponEntity)
            });

            return weaponEntity;
        }

        Entity FireMachineGunProjectile(EntityWorld world, Entity weapon)
        {
            var projectileEntity = world.CreateEntity();
            var weaponComponent = weapon.GetComponent<WeaponComponent>();



            return projectileEntity;
        }
    }
}
