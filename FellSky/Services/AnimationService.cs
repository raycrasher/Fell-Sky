using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Services
{
    public interface IAnimationService
    {
        Coroutine Animate(Func<TimeSpan, bool> updater);
        Coroutine LerpTo<TSource>(TSource source, Expression<Func<TSource, Vector2>> propertyLambda, Vector2 value, TimeSpan duration);
        Coroutine LerpTo<TSource>(TSource source, Expression<Func<TSource, float>> propertyLambda, float value, TimeSpan duration);
    }

    public class AnimationService : IAnimationService
    {
        ICoroutineService _coroutineService;
        ITimerService _timerService;

        public AnimationService(ICoroutineService coroutine, ITimerService timer)
        {
            _coroutineService = coroutine;
            _timerService = timer;
        }
        
        public Coroutine LerpTo<TSource>(TSource source, Expression<Func<TSource, float>> propertyLambda, float value, TimeSpan duration)
        {
            var propertyInfo = GetPropertyInfo(source, propertyLambda);
            var getter = BuildGetAccessor<TSource, float>(propertyInfo.GetMethod);
            var setter = BuildSetAccessor<TSource, float>(propertyInfo.GetMethod);

            TimeSpan currentTime = TimeSpan.Zero;
            var startValue = getter(source);
            var currentValue = startValue;

            return Animate(t => 
            {
                double amount = currentTime.TotalSeconds / duration.TotalSeconds;
                setter(source, MathHelper.Lerp(startValue, value, (float) amount));
                currentTime += t;
                return 1.0 - amount < double.Epsilon;
            });
        }

        public Coroutine LerpTo<TSource>(TSource source, Expression<Func<TSource, Vector2>> propertyLambda, Vector2 value, TimeSpan duration)
        {
            var propertyInfo = GetPropertyInfo(source, propertyLambda);
            var getter = BuildGetAccessor<TSource, Vector2>(propertyInfo.GetMethod);
            var setter = BuildSetAccessor<TSource, Vector2>(propertyInfo.GetMethod);

            TimeSpan currentTime = TimeSpan.Zero;
            var startValue = getter(source);
            var currentValue = startValue;

            return Animate(t =>
            {
                double amount = currentTime.TotalSeconds / duration.TotalSeconds;
                setter(source, Vector2.Lerp(startValue, value, (float)amount));
                currentTime += t;
                return 1.0 - amount < double.Epsilon;
            });
        }


        public Coroutine Animate(Func<TimeSpan, bool> updater)
        {
            return _coroutineService.StartCoroutine(DoAnimation(updater));
        }



        private IEnumerable DoAnimation(Func<TimeSpan, bool> updater)
        {
            while (updater(_timerService.DeltaTime))
                yield return null;
        }

        static PropertyInfo GetPropertyInfo<TSource, TProperty>(
            TSource source,
            Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }

        static Action<TSource, TProperty> BuildSetAccessor<TSource, TProperty>(MethodInfo method)
        {
            var obj = Expression.Parameter(typeof(TSource), "o");
            var value = Expression.Parameter(typeof(TProperty));

            Expression<Action<TSource, TProperty>> expr =
                Expression.Lambda<Action<TSource, TProperty>>(
                    Expression.Call(
                        Expression.Convert(obj, method.DeclaringType),
                        method,
                        Expression.Convert(value, method.GetParameters()[0].ParameterType)),
                    obj,
                    value);

            return expr.Compile();
        }

        static Func<TSource, TProperty> BuildGetAccessor<TSource, TProperty>(MethodInfo method)
        {
            var obj = Expression.Parameter(typeof(TSource), "o");

            Expression<Func<TSource, TProperty>> expr =
                Expression.Lambda<Func<TSource, TProperty>>(
                    Expression.Convert(
                        Expression.Call(
                            Expression.Convert(obj, method.DeclaringType),
                            method),
                        typeof(object)),
                    obj);

            return expr.Compile();
        }
    }
}
