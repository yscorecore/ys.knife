using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace System.Reflection
{
    public static class PathDirection
    {
        private static ITypeService tyservice = SmartTypeService.Instance;
        
        static Dictionary<Type,TypeMemberCache> dic = new Dictionary<Type,TypeMemberCache>();

        public static object GetObject(object obj,string path)
        {
            if(string.IsNullOrEmpty(path)) return obj;
            if(obj == null) return obj;
            string[] members = path.Split(new char[] { '.', },StringSplitOptions.RemoveEmptyEntries);
            if(members.Length == 0) return obj;
            lock(dic)
            {
                return GetDirectObject(obj,members,0);
            }
        }

        public static object GetObject<T>(string path)
        {
            if(string.IsNullOrEmpty(path)) return null;
            string[] members = path.Split(new char[] { '.', },StringSplitOptions.RemoveEmptyEntries);
            if(members.Length == 0) return null;
            lock(dic)
            {
                if(!dic.ContainsKey(typeof(T)))
                    dic.Add(typeof(T),new TypeMemberCache(typeof(T)));
                object obj = dic[typeof(T)].GetStaticMemberValue(members[0].Trim());
                return GetDirectObject(obj,members,1);
            }
        }

        public static object GetObject(Type ty,string path)
        {
            string[] members = path.Split(new char[] { '.', },StringSplitOptions.RemoveEmptyEntries);
            if(members.Length == 0) return null;
            lock(dic)
            {
                if(!dic.ContainsKey(ty))
                    dic.Add(ty,new TypeMemberCache(ty));
                object obj = dic[ty].GetStaticMemberValue(members[0].Trim());
                return GetDirectObject(obj,members,1);
            }
        }

        private static object GetDirectObject(object obj,string[] paths,int index)
        {
            for(int i = index;i < paths.Length;i++)
            {
                if(obj == null) return null;
                Type objType = obj.GetType();
                if(!dic.ContainsKey(objType))
                {
                    TypeMemberCache tmc = new TypeMemberCache(objType);
                    dic.Add(objType,tmc);
                    obj = tmc.GetMemberValue(obj,paths[i].Trim());
                }
                else
                {
                    obj = dic[objType].GetMemberValue(obj,paths[i].Trim());
                }
            }
            return obj;
        }

        class TypeMemberCache
        {
            private readonly Type ty;
            public TypeMemberCache(Type ty)
            {
                this.ty = ty;
            }
            Dictionary<string,MemberInfo> Props = new Dictionary<string,MemberInfo>();
            public object GetStaticMemberValue(string memberName)
            {
                if(string.IsNullOrEmpty(memberName)) return null;

                int indexch = memberName.IndexOf('(');
                if(indexch > 0) { memberName = memberName.Substring(0,indexch); }
                memberName = memberName.Trim();
                lock(Props)
                {
                    if(Props.ContainsKey(memberName))
                    {
                        return GetStaticMemberValue(Props[memberName]);
                    }
                    else
                    {
                        var p = ty.GetMember(memberName,BindingFlags.Static | BindingFlags.Public);
                        if(p != null && p.Length >0)
                        {
                            Props.Add(memberName,p[0]);
                            return GetStaticMemberValue(p[0]);
                        }
                        else
                        {
                            throw new MemberNotFoundException(ty,memberName);
                        }
                    }
                }
            }
            public object GetMemberValue(object obj,string memberName)
            {
                if(obj == null) return null;
                if(string.IsNullOrEmpty(memberName)) return null;
                int indexch = memberName.IndexOf('(');
                if(indexch > 0) { memberName = memberName.Substring(0,indexch); }
                memberName = memberName.Trim();
                lock(Props)
                {
                    if(Props.ContainsKey(memberName))
                    {
                        return GetMemberValue(obj,Props[memberName]);
                    }
                    else
                    {
                        var p = ty.GetMember(memberName,BindingFlags.Instance | BindingFlags.Public);
                        if(p != null && p.Length > 0)
                        {
                            Props.Add(memberName,p[0]);
                            return GetMemberValue(obj,p[0]);
                        }
                        else
                        {
                            throw new MemberNotFoundException(ty,memberName);
                        }
                    }
                }

            }
            private object GetStaticMemberValue(MemberInfo minfo)
            {
                if(minfo is PropertyInfo)
                {
                    return (minfo as PropertyInfo).GetValue(null,null);
                }
                else if(minfo is MethodInfo)
                {
                    return (minfo as MethodInfo).Invoke(null,null);
                }
                else if(minfo is FieldInfo)
                {
                    return (minfo as FieldInfo).GetValue(null);
                }
                else
                {
                    throw new NotSupportedException(string.Format("can not handle the member: {0}",minfo.Name));
                }
            }
            private object GetMemberValue(object obj,MemberInfo minfo)
            {
                if(minfo is PropertyInfo)
                {
                    return (minfo as PropertyInfo).GetValue(obj,null);
                }
                else if(minfo is MethodInfo)
                {
                    return (minfo as MethodInfo).Invoke(obj,null);
                }
                else if(minfo is FieldInfo)
                {
                    return (minfo as FieldInfo).GetValue(obj);
                }
                else
                {
                    throw new NotSupportedException(string.Format("can not handle the member: {0}",minfo.Name));
                }
            }
        }
    }


}
