﻿using Artemis.Interface;
using FellSky.Framework;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class FrameAnimationComponent: IComponent
    {
        public int CurrentFrame => MathHelper.Clamp((int)(CurrentTime / Fps), 0, Frames?.Count ?? 0);

        public float Fps;
        public float CurrentTime;
        public List<SpriteComponent> Frames = new List<SpriteComponent>();
        public AnimationState State = AnimationState.Stopped;

        public void Play() => State = AnimationState.Playing;
        public void Stop()
        {
            State = AnimationState.Stopped;
            CurrentTime = 0;
        }
        public void Pause() => State = AnimationState.Paused;
    }
}
