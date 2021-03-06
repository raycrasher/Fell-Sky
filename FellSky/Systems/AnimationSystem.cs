﻿using Artemis;
using FellSky.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems
{
    public class AnimationSystem: Artemis.System.EntitySystem
    {
        public AnimationSystem()
            : base(Aspect.All(typeof(AnimationComponent)))
        {
        }

        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            foreach(var entity in entities.Values)
            {
                var animation = entity.GetComponent<AnimationComponent>();
                animation.PositionAnimator?.MoveNext();
                animation.RotationAnimator?.MoveNext();
                animation.ScaleAnimator?.MoveNext();
                animation.ColorAnimator?.MoveNext();
                animation.AlphaAnimator?.MoveNext();
            }
        }
    }
}
