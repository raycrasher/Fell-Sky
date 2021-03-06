﻿using Artemis;
using FellSky.Components;
using FellSky.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Systems
{
    public class FrameAnimationSystem : Artemis.System.EntityComponentProcessingSystem<FrameAnimationComponent, SpriteComponent>
    {
        public override void Process(Entity entity, FrameAnimationComponent animation, SpriteComponent sprite)
        {
            if (animation.State == Framework.AnimationState.Playing)
            {
                animation.CurrentTime += EntityWorld.Delta;
                if (animation.CurrentFrame >= animation.Frames.Count)
                {
                    if (animation.Loop)
                    {
                        animation.CurrentTime = 0;
                    }else
                    {
                        animation.Stop();
                    }
                }                    
                entity.AddComponent(animation.Frames[animation.CurrentFrame]);
            }            
        }
    }
}
