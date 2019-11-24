using System;
using System.Collections.Generic;
using System.Common;
using System.Data;
using System.Data.Entity;
using System.Data.Service;
using System.Linq;
using System.Text;

namespace System.Data.Common
{

    public interface IFileInfoService:ICURDAll<FileInfo,Guid>, ISequence<FileInfo,Guid>
    {

        FileInfo FindCatagoryItem(Guid fileCatagory, SearchCondition condition, OrderCondition orderitems);

        List<FileInfo> ListCatagoryAll(Guid fileCatagory, SearchCondition condition, OrderCondition orderitems);

        PageData<FileInfo> ListCatagoryPage(Guid fileCatagory, SearchCondition condition, OrderCondition orderitems, PageInfo pageinfo);

        int CountCatagory(Guid fileCatagory,SearchCondition conditions);
    }

}
