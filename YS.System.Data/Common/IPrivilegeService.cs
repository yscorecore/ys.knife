using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace System.Data.Common
{
    public interface IPrivilegeService
    {
        /// <summary>
        /// 获取授权的功能点
        /// </summary>
        /// <returns></returns>
        List<FunctionInfo> GetAuthorizedFunctions();
        /// <summary>
        /// 获取所有的功能点
        /// </summary>
        /// <returns></returns>
        List<FunctionInfo> GetAllFunctions();
        /// <summary>
        /// 注册功能点
        /// </summary>
        /// <param name="funcs"></param>
        /// <returns></returns>
        ResultInfo RegeditFunctions(List<FunctionInfo> funcs);


        
    }
}
