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

namespace UcbManagementInformation.ServiceLocation
{
    public interface ISimpleServiceLocator
    {
        T Get<T>();
        Type Get(Type type);
        T Get<T>(string key);
        void Inject<T>(T instance);
        void InjectAsSingleton<T>(T instance);
        void Register<TInterface, TImplementor>() where TImplementor : TInterface;
        void Register<TInterface, TImplementor>(string key) where TImplementor : TInterface;
        void RegisterAsSingleton<TInterface, TImplementor>() where TImplementor : TInterface;
        void RegisterWithConstructorParameters<TInterface, TImplementor>(params object[] parameters) where TImplementor : TInterface;
        void RegisterWithConstructorParameters<TInterface, TImplementor>(string key, params object[] parameters) where TImplementor : TInterface;
        void RegisterWithConstructorParameters(Type interfaceType, Type implementorType, params object[] parameters);
    }
}
