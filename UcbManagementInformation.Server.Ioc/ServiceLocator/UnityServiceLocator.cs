using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;
using System.Collections.Generic;


namespace UcbManagementInformation.Server.IoC.ServiceLocation
{
    public class UnityServiceLocator : ISimpleServiceLocator
    {
        private readonly UnityContainer unityContainer;

        public UnityServiceLocator()
        {
            this.unityContainer = new UnityContainer();

            // Prep for moving BootStrapper info to config.

            //UnityConfigurationSection config = // Where be this UnityConfigurationSection thingy?
            //    (UnityConfigurationSection)ConfigurationManager.GetSection("unity");

            //IUnityContainer container = new UnityContainer();

            //config.Containers.Default.Configure(container); 
        }

        public T Get<T>()
        {
            return this.unityContainer.Resolve<T>();
        }
        public Type Get(Type type)
        {
            return (Type)this.unityContainer.Resolve(type);
        }
        public Type Get(Type type, Dictionary<string, object> parameterOverrides)
        {
            return (Type)this.unityContainer.Resolve(type, UnpackParameters(parameterOverrides));
        }
        public T Get<T>(string key)
        {
            return this.unityContainer.Resolve<T>(key);
        }
        public T Get<T>(string key, Dictionary<string, object> parameterOverrides)
        {
            return this.unityContainer.Resolve<T>(key, UnpackParameters(parameterOverrides));
        }
        public T Get<T>(Dictionary<string,object> parameterOverrides)
        {
            return this.unityContainer.Resolve<T>(UnpackParameters(parameterOverrides));
        }

        private static ParameterOverride[] UnpackParameters(Dictionary<string,object> parameterOverrides)
        {
            List<ParameterOverride> parameterOverrideList = new List<ParameterOverride>(); 
            foreach(string paramKey in parameterOverrides.Keys)
            {
                parameterOverrideList.Add(new ParameterOverride(paramKey,parameterOverrides[paramKey]));
            }
            ParameterOverride[] ParameterOverrideArray = parameterOverrideList.ToArray();
            return ParameterOverrideArray;
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
        public void RegisterWithConstructorParameters(Type interfaceType,Type implementorType,params object[] parameters)
        {
            this.unityContainer.RegisterType(interfaceType, implementorType,new InjectionConstructor(parameters));
        }
        public void Register(Type interfaceType,Type implementorType)
        {
            this.unityContainer.RegisterType(interfaceType, implementorType);
        }

        public void Register<TInterface, TImplementor>(string key) where TImplementor : TInterface
        {
            this.unityContainer.RegisterType<TInterface, TImplementor>(key);
        }
        public void RegisterWithConstructorParameters<TInterface, TImplementor>(string key, params object[] parameters) where TImplementor : TInterface
        {
            this.unityContainer.RegisterType<TInterface, TImplementor>(key,new InjectionConstructor(parameters));
        }
        public void RegisterWithConstructorParameters<TInterface, TImplementor>(params object[] parameters) where TImplementor : TInterface
        {
            this.unityContainer.RegisterType<TInterface, TImplementor>(new InjectionConstructor(parameters));
        }

        public void RegisterAsSingleton<TInterface, TImplementor>() where TImplementor : TInterface
        {
            this.unityContainer.RegisterType<TInterface, TImplementor>(new ContainerControlledLifetimeManager());
        }

    }
}
