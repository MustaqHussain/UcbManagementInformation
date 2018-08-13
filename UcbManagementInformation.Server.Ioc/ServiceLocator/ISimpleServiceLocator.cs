using System;
using System.Collections.Generic;

namespace UcbManagementInformation.Server.IoC.ServiceLocation
{
    public interface ISimpleServiceLocator
    {
        T Get<T>();
        Type Get(Type type);
        T Get<T>(string key);
        T Get<T>(Dictionary<string,object> parameterOverrides);
        Type Get(Type type,Dictionary<string,object> parameterOverrides);
        T Get<T>(string key,Dictionary<string,object> parameterOverrides);
        void Inject<T>(T instance);
        void InjectAsSingleton<T>(T instance);
        void Register(Type interfaceType, Type implementorType);
        void Register<TInterface, TImplementor>() where TImplementor : TInterface;
        void RegisterWithConstructorParameters<TInterface, TImplementor>(params object[] parameters) where TImplementor : TInterface;
        void RegisterWithConstructorParameters<TInterface, TImplementor>(string key, params object[] parameters) where TImplementor : TInterface;
        void RegisterWithConstructorParameters(Type interfaceType, Type implementorType, params object[] parameters);
        void Register<TInterface, TImplementor>(string key) where TImplementor : TInterface;
        
        void RegisterAsSingleton<TInterface, TImplementor>() where TImplementor : TInterface;
    }
}
