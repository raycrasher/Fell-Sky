using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Components
{
    public class AnimationEditorComponent: IComponent
    {
        public Transform Transform = new Transform();
        public Color Color = Color.White;
        public float Alpha = 1;
    }
}
