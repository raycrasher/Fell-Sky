using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Framework
{
    public static class Animation
    {
        public static Coroutine Rotate(Transform xform, float speed) => Game.Coroutines.StartCoroutine(DoRotate(xform, speed));
        public static Coroutine RotateTo(Transform xform, float speed, float deltaAngle, bool smooth = true) => Game.Coroutines.StartCoroutine(DoRotateTo(xform, speed, deltaAngle, smooth));
        //public static Coroutine RotateTo(Transform xform, TimeSpan speed, float deltaAngle, bool smooth = true) => Game.Coroutines.StartCoroutine(DoRotateTo(xform, speed, deltaAngle, smooth));
        public static Coroutine RotateBy(Transform xform, float speed, float deltaAngle, bool smooth = true) => Game.Coroutines.StartCoroutine(DoRotateTo(xform, speed, deltaAngle, smooth));

        public static Coroutine Move(Transform xform, Vector2 delta) => Game.Coroutines.StartCoroutine(DoMove(xform, delta));
        //public static Coroutine MoveTo(Transform xform, float speed, Vector2 delta, bool smooth = true) => Game.Coroutines.StartCoroutine(DoMoveTo(xform, speed, delta, smooth));

        private static IEnumerable DoMove(Transform xform, Vector2 delta)
        {
            while (true)
            {
                xform.Position += delta * (float)(Game.CurrentUpdateTime.ElapsedGameTime.TotalSeconds * 2 * Math.PI);
                yield return null;
            }
        }

        private static IEnumerable DoRotateTo(Transform xform, float speed, float delta, bool smooth)
        {
            float theta = 0;
            float origRotation = xform.Rotation;
            var thetaLimit = Math.Abs(delta);
            var absSpeed = Math.Abs(speed);
            while (theta < thetaLimit)
            {
                xform.Rotation = smooth ? MathHelper.SmoothStep(origRotation, origRotation + delta, theta / delta)
                                        : MathHelper.Lerp(origRotation, origRotation + delta, theta / delta);
                theta += (float)(Game.CurrentUpdateTime.ElapsedGameTime.TotalSeconds * 2 * Math.PI * absSpeed);
                yield return null;
            }

            if (theta > thetaLimit) xform.Rotation = origRotation + delta;
        }

        private static IEnumerable DoRotate(Transform xform, float speed)
        {
            while (true)
            {
                xform.Rotation += (float)(Game.CurrentUpdateTime.ElapsedGameTime.TotalSeconds * 2 * Math.PI * speed);
                yield return null;
            }
        }
    }
}
