using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky.Services
{
    public class ServiceLocator: IServiceProvider
    {
        private GameServiceContainer _services;

        public static ServiceLocator Instance { get; private set; }

        public static void Initialize() => Instance = new ServiceLocator();
        public static void Initialize(GameServiceContainer gameServiceContainer) => Instance = new ServiceLocator(gameServiceContainer);

        public ServiceLocator(GameServiceContainer gameServiceContainer)
        {
            _services = gameServiceContainer;
        }

        public ServiceLocator()
        {
            _services = new GameServiceContainer();
        }

        public object GetService(Type serviceType) => _services.GetService(serviceType);

        public T GetService<T>() where T : class => _services.GetService<T>();

        public void AddService<T>(T service) where T : class => _services.AddService(service);

        public void AddService<TInterface, TInstance>()
            where TInstance : class, TInterface, new() 
            => _services.AddService<TInterface>(new TInstance());

        public void AddService<TInterface, TInstance>(TInstance instance)
            where TInstance : class, TInterface 
            => _services.AddService<TInterface>(instance);

        public void RemoveService<T>() => _services.RemoveService(typeof(T));
        public void RemoveService(Type type) => _services.RemoveService(type);
    }

    
}
