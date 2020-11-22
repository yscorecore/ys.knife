using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace YS.Knife.EntityFrameworkCore
{
    public interface ICommitEFChangesContext
    {
        DbContext DbContext { get; }
    }
}
