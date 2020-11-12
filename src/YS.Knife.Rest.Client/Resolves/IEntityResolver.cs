namespace YS.Knife.Rest.Client.Resolves
{
    public interface IEntityResolver
    {
        T Resolve<T>(string content);
    }
}
