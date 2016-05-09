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
        private Dictionary<Type, Func<object>> _serviceGetters = new Dictionary<Type, Func<object>>();

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

        public void RegisterService<T>() 
            where T : class, new()
        {
            _serviceGetters[typeof(T)] = () => new T();
        }

        public void RegisterService<T>(Func<T> getter)
            where T : class
        {
            _serviceGetters[typeof(T)] = getter;
        }

        public void RegisterService<TInterface, TObject>()
            where TInterface : class
            where TObject: class, TInterface, new()
        {
            _serviceGetters[typeof(TInterface)] = () => new TObject();
        }

        public void RemoveService<T>() => _services.RemoveService(typeof(T));
        public void RemoveService(Type type) => _services.RemoveService(type);
    }

    
}
