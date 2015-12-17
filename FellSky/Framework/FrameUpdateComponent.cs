using Artemis;
using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;

namespace FellSky.Framework
{
    public class FrameUpdateComponent: IComponent
    {
        public Action<GameTime, Entity> UpdateFunction { get; set; }
    }
}
