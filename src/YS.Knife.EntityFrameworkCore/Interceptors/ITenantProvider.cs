namespace YS.Knife.EntityFrameworkCore.Interceptors
{
    public interface ITenantProvider
    {
        string GetTenantId();
    }
}
