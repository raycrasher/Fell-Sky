using Artemis;
using FellSky.Components;
using FellSky.Services;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Framework
{
    public static class GuiElementUtilities
    {
        static IAnimationService _animationService => ServiceLocator.Instance.GetService<IAnimationService>();

        public static Vector4 ToVector4(this LibRocketNet.Color color) =>new Vector4(color.R / 255f,color.G / 255f,color.B / 255f,color.A / 255f);
        public static LibRocketNet.Color ToLibRocketColor(this Vector4 v) => new LibRocketNet.Color
        {
            R = (byte)MathHelper.Clamp((v.X * 255), 0, 255),
            G = (byte)MathHelper.Clamp((v.Y * 255), 0, 255),
            B = (byte)MathHelper.Clamp((v.Z * 255), 0, 255),
            A = (byte)MathHelper.Clamp((v.W * 255), 0, 255)
        };

        public static Coroutine FadeGuiElement(this Entity entity, TimeSpan duration, byte alphaFrom, byte alphaTo)
        {
            var component = entity.GetComponent<GuiComponent>();
            if (component == null) return null;
            var bg = component.Element.GetPropertyColor("background-color");
            var fg = component.Element.GetPropertyColor("color");

            bg.A = alphaFrom;
            fg.A = alphaFrom;

            component.Element.SetProperty("background-color", bg.ToString());
            component.Element.SetProperty("color", fg.ToString());

            return entity.FadeGuiElement(duration, alphaTo);
        }

        public static Coroutine FadeGuiElement(this Entity entity, TimeSpan duration, byte alpha)
        {
            var component = entity.GetComponent<GuiComponent>();
            if (component == null) return null;
            var bg = component.Element.GetPropertyColor("background-color");
            var fg = component.Element.GetPropertyColor("color");

            TimeSpan currentTime = TimeSpan.Zero;
            return _animationService.Animate(t =>
            {
                float amount = (float)(currentTime.TotalSeconds / duration.TotalSeconds);
                LibRocketNet.Color c = bg;
                c.A = (byte) MathHelper.Lerp(bg.A, alpha, amount);
                component.Element.SetProperty("background-color", c.ToString());
                c = fg;
                c.A = (byte)MathHelper.Lerp(fg.A, alpha, amount);
                component.Element.SetProperty("color", c.ToString());
                currentTime += t;
                return 1.0 - amount > float.Epsilon;
            });
        }

        public static void RemoveAllChildren(this LibRocketNet.Element element)
        {
            element.InnerRml = "";
        }
    }
}
