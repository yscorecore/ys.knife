using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace System.Authority
{
    public class FunctionInfoValidater
    {
        //public EntityValidateResult Validate(IEnumerable<FunctionInfo> functions)
        //{
        //    var result = new EntityValidateResult();
        //    var emptyCodeFunctions = functions.Where(p => string.IsNullOrWhiteSpace(p.Code));
        //    foreach (var emptyCodeFunction in emptyCodeFunctions)
        //    {
        //        result.Add(new EntityValidateError()
        //        {
        //            Entity = emptyCodeFunction,
        //            Error = string.Format("存在Code为空的项。Name={0},Level={1}", emptyCodeFunction.Name, emptyCodeFunction.Level)
        //        });
        //    }
        //    var res = (from p in functions
        //               group p by new {  p.Code } into t
        //               where t.Count() > 1
        //               select new
        //               {
        //                   Key = t.Key,
        //                   Items = t.ToList()
        //               }).ToList();
        //    foreach (var v in res)
        //    {
        //        foreach (var r in v.Items)
        //        {
        //            result.Add(new EntityValidateError()
        //            {
        //                Entity = r,
        //                Error = string.Format("存在多个相同Code的节点,Code:{0},ParentId:{1},Name={2},Id={3}", r.Code, r.ParentId, r.Name, r.Id)
        //            });
        //        }
        //    }
        //    return result;

        //}

    }
}
