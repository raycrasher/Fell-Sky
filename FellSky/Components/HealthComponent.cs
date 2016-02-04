using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class HealthComponent: IComponent
    {
        public float Health { get; set; } = 100;
        public float MaxHealth { get; set; } = 100;

        public bool IsAlive => Health > 0;
        public bool IsFullHealth => Health >= MaxHealth;

        public HealthComponent(float maxHealth, float currentHealth)
        {
            MaxHealth = maxHealth;
            Health = currentHealth;
        }

        public HealthComponent(float maxHealth)
        {
            MaxHealth = maxHealth;
            Health = MaxHealth;
        }
    }
}
