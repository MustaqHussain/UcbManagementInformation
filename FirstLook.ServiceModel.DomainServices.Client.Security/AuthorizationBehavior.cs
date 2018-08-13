namespace FirstLook.ServiceModel.DomainServices.Client.Security
{
    public abstract class AuthorizationBehavior
    {
        public abstract void AddBehavior(object target, AuthorizationSource source);

        public abstract void RemoveBehavior(object target);
    }
}
