using Microsoft.EntityFrameworkCore;

namespace YS.Knife.EntityFrameworkCore
{
    internal interface IDbContextConsumer
    {
        DbContext DbContext { get; }
    }
}
