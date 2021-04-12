using System;
using YS.Knife.Aop;

namespace YS.Knife.CodeException
{
    [CodeExceptions(100000)]
    public interface IAllErrors
    {
        [Ce(100, "User {userid} not found.")]
        Exception UserNotFound(string userId);

        [Ce(101, "Permission denied.")]
        Exception PermissionDenied();
    }
}
