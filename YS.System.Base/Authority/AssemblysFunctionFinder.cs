using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Authority
{
    public class AssemblysFunctionFinder : IFunctionFinder
    {
        public AssemblysFunctionFinder()
        {
            this.AssemblyFilePaths = new List<string>();
        }
        public List<string> AssemblyFilePaths { get; private set; }



        public IEnumerable<FunctionInfo> FindFunctions()
        {
            using (var remote = new System.NewDomainObject<FinderInternal>(Guid.NewGuid().ToString()))
            {
                return remote.RemoteObject.LoadFunctions(AssemblyFilePaths.ToArray());
            }
        }





         class FinderInternal : MarshalByRefObject
        {
            private void SetFunctionAndModuleParentIds(List<FunctionInfo> functions)
            {
                var moduleOrFunctions = functions.Where(p => (p.Level == FunctionLevel.Module || p.Level == FunctionLevel.Function)).OfType<FunctionInfo_Temp>();
                foreach (var v in moduleOrFunctions)
                {
                    v.ParentId = (from p in functions
                                  where (p.Level == FunctionLevel.Application || p.Level == FunctionLevel.Module)
                                  && p.Code == v.ParentCode
                                  select p.Id).FirstOrDefault();
                }
            }

            public List<FunctionInfo> LoadFunctions(string[] files)
            {
                List<FunctionInfo> res = new List<Authority.FunctionInfo>();
                foreach (var file in files)
                {
                    var ass = Assembly.LoadFrom(file);
                    res.AddRange(LoadFunctions(ass));
                }
                this.SetFunctionAndModuleParentIds(res);
                return res;
            }

            public List<FunctionInfo> LoadFunctions(Assembly[] assemblies)
            {
                List<FunctionInfo> res = new List<Authority.FunctionInfo>();
                foreach (var ass in assemblies)
                {
                    res.AddRange(LoadFunctions(ass));
                }
                this.SetFunctionAndModuleParentIds(res);
                return res;
            }

            private List<FunctionInfo> LoadFunctions(Assembly assembly)
            {
                List<FunctionInfo> functions = new List<FunctionInfo>();
                functions.AddRange(FindApplications(assembly));
                functions.AddRange(FindModules(assembly));
                functions.AddRange(FindFunctionsAndOprations(assembly));
                return functions;
            }
            private List<FunctionInfo> FindApplications(Assembly assembly)
            {
                List<FunctionInfo> lst = new List<FunctionInfo>();
                var ass = assembly.GetCustomAttribute<ApplicationAttribute>();
                if (ass != null)
                {
                    var guidattr = assembly.GetCustomAttribute<GuidAttribute>();
                    lst.Add(new Authority.FunctionInfo()
                    {
                        Code = ass.Code ?? string.Empty,
                        Description = ass.Description,
                        Icon = ass.Icon,
                        Level = FunctionLevel.Application,
                        Name = ass.Name,
                        Sequence = ass.Sequence,
                        Id = guidattr != null ? new Guid(guidattr.Value).ToString() : (ass.Code ?? string.Empty).ToGuid().ToString()
                    });
                }
                return lst;
            }

            private List<FunctionInfo> FindModules(Assembly assembly)
            {
                List<FunctionInfo> lst = new List<FunctionInfo>();
                var modules = assembly.GetCustomAttributes<ModuleAttribute>();
                var guidattr = assembly.GetCustomAttribute<GuidAttribute>();
                foreach (var module in modules)
                {
                    lst.Add(new FunctionInfo_Temp()
                    {
                        Code = module.Code ?? string.Empty,
                        Description = module.Description,
                        Icon = module.Icon,
                        Level = FunctionLevel.Module,
                        Name = module.Name,
                        Sequence = module.Sequence,
                        Id = guidattr != null ? (guidattr.Value + module.Code).ToGuid().ToString() : module.Code.ToGuid().ToString(),
                        ParentCode = module.ParentCode

                    });
                }
                return lst;
            }

            private List<FunctionInfo> FindFunctionsAndOprations(Assembly assembly)
            {
                List<FunctionInfo> lst = new List<FunctionInfo>();
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var function in type.GetCustomAttributes<FunctionAttribute>())
                    {
                        function.InitFunctionCode(type);
                        var typeid = (type.GUID.ToString()+"_opFun_"+function.Code).ToGuid().ToString();
                        lst.Add(new FunctionInfo_Temp()
                        {
                            Code = function.Code,
                            Id = typeid,
                            Description = function.Description,
                            Icon = function.Icon,
                            Level = FunctionLevel.Function,
                            Name = function.Name,
                            Sequence = function.Sequence,
                            ParentCode = function.ParentCode,
                        });
                        lst.AddRange(FindOperations(function.Code, typeid, type));
                    }

                   
                }
               
                return lst;
            }
            private List<FunctionInfo> FindOperations(string parentCode, string parentid, Type type)
            {
                List<FunctionInfo> lst = new List<FunctionInfo>();
                var methods = GetMethodFunctions(type);
                var tempdic = new Dictionary<string, FunctionInfo>();
                foreach (var v in methods)
                {
                    v.Value.InitFunctionCode(type,v.Key);
                    var groups = (v.Value.GroupCode ?? "").Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
                    string tempgroupkey = parentCode;
                    string tempparentid = parentid;
                    for (int i = 0; i < groups.Length; i++)
                    {
                        tempgroupkey = string.IsNullOrEmpty(tempgroupkey) ? groups[i] : tempgroupkey + "/" + groups[i];
                        if (!tempdic.ContainsKey(tempgroupkey))
                        {
                            tempdic.Add(tempgroupkey, new FunctionInfo()
                            {
                                Name = groups[i],
                                Icon = null,
                                Description = null,
                                Level = FunctionLevel.OperationGroup,
                                Sequence = default(int),
                                Code = tempgroupkey,
                                Id = (parentid + "_opGroup_" + tempgroupkey).ToGuid().ToString(),
                                ParentId = tempparentid
                            });
                            lst.Add(tempdic[tempgroupkey]);
                        }
                        tempparentid = tempdic[tempgroupkey].Id;
                    }//添加group结束
                    lst.Add(new FunctionInfo()
                    {
                        Code = v.Value.Code,
                        Id = (parentid + v.Key.Name).ToGuid().ToString(),
                        Icon = v.Value.Icon,
                        Sequence = v.Value.Sequence,
                        Name = v.Value.Name,
                        Level = FunctionLevel.Operation,
                        Description = v.Value.Description,
                        ParentId = tempparentid
                    });
                }
                return lst;
            }
            private Dictionary<MethodInfo, OperationAttribute> GetMethodFunctions(Type ty)
            {
                Dictionary<MethodInfo, OperationAttribute> dic = new Dictionary<MethodInfo, OperationAttribute>();
                foreach (var minfo in ty.GetMethods())
                {
                    var opattr = minfo.GetCustomAttribute<OperationAttribute>(false);
                    if (opattr != null) dic.Add(minfo, opattr);
                }
                return dic;
            }

        }

        [Serializable]
        class FunctionInfo_Temp : FunctionInfo
        {
            public string ParentCode { get; set; }
        }

    }
}
