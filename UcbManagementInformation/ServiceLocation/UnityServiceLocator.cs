using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;


namespace UcbManagementInformation.ServiceLocation
{
    public class UnityServiceLocator : ISimpleServiceLocator
    {
        private readonly UnityContainer unityContainer;

        public UnityServiceLocator()
        {
            this.unityContainer = new UnityContainer();
        }

        public T Get<T>()
        {
            return this.unityContainer.Resolve<T>();
        }

        public Type Get(Type type)
        {
            return (Type)this.unityContainer.Resolve(type);
        }

        public T Get<T>(string key)
        {
            return this.unityContainer.Resolve<T>(key);
        }

        public void Inject<T>(T instance)
        {
            this.unityContainer.RegisterInstance(instance, new ExternallyControlledLifetimeManager());
        }

        public void InjectAsSingleton<T>(T instance)
        {
            this.unityContainer.RegisterInstance(instance);
        }

        public void Register<TInterface, TImplementor>() where TImplementor : TInterface
        {
            this.unityContainer.RegisterType<TInterface, TImplementor>();
        }

        public void Register<TInterface, TImplementor>(string key) where TImplementor : TInterface
        {
            this.unityContainer.RegisterType<TInterface, TImplementor>(key);
        }

        public void RegisterAsSingleton<TInterface, TImplementor>() where TImplementor : TInterface
        {
            this.unityContainer.RegisterType<TInterface, TImplementor>(new ContainerControlledLifetimeManager());
        }

        public void RegisterWithConstructorParameters(Type interfaceType, Type implementorType, params object[] parameters)
        {
            this.unityContainer.RegisterType(interfaceType, implementorType, new InjectionConstructor(parameters));
        }

        public void RegisterWithConstructorParameters<TInterface, TImplementor>(string key, params object[] parameters) where TImplementor : TInterface
        {
            this.unityContainer.RegisterType<TInterface, TImplementor>(key, new InjectionConstructor(parameters));
        }

        public void RegisterWithConstructorParameters<TInterface, TImplementor>(params object[] parameters) where TImplementor : TInterface
        {
            this.unityContainer.RegisterType<TInterface, TImplementor>(new InjectionConstructor(parameters));
        }
    }
}
