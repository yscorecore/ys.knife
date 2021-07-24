using Microsoft.EntityFrameworkCore;

namespace YS.Knife.EntityFrameworkCore
{
    internal interface IDbContextConsumer
    {
        DbContext DbContext { get; }
    }
    internal interface IDbContextConsumer<T> : IDbContextConsumer
        where T : DbContext
    {

    }
}
