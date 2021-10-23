using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using AutoMapper;
using BenchmarkDotNet.Attributes;
using YS.Knife;

namespace Mapper.Benchmark
{
    public class MapperTest
    {
        private IMapper autoMapper;
        private MapsterMapper.Mapper mapster;

        private UserInfo user = new UserInfo
        {
            Name = Guid.NewGuid().ToString(),
            Age = 30,
            Birthday = DateTime.Now
        };
        public MapperTest()
        {
            InitAutoMapper();
            InitMapster();
            void InitAutoMapper()
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<UserInfo, UserDto>());

                autoMapper = config.CreateMapper();
            }
            void InitMapster()
            {
                var config = new Mapster.TypeAdapterConfig();
                config.ForType<UserInfo, UserDto>();
                mapster = new MapsterMapper.Mapper(config);
            }
        }


        [Params(1, 100, 10000)]
        public int LoopCount { get; set; }

        [Benchmark]
        public void AutoMapper()
        {
            for (int i = 0; i < LoopCount; i++)
            {
                var dto = autoMapper.Map<UserDto>(user);
            }

        }
        [Benchmark]
        public void Mapster()
        {
            for (int i = 0; i < LoopCount; i++)
            {
                var dto = mapster.Map<UserDto>(user);
            }
        }

        [Benchmark]
        public void KnifeMap()
        {
            for (int i = 0; i < LoopCount; i++)
            {
                var dto = user.Map<dynamic, UserDto>();
            }
        }

        [Benchmark]
        public void KnifeMapTo()
        {
            for (int i = 0; i < LoopCount; i++)
            {
                var dto = user.MapTo<UserDto>();
            }
        }

        [Benchmark]
        public void KnifeMapToWithEmit()
        {
            for (int i = 0; i < LoopCount; i++)
            {
                var dto = user.MyMapTo<UserDto>();
            }
        }
        [Benchmark]
        public void ConvertTo()
        {
            for (int i = 0; i < LoopCount; i++)
            {
                // var dto = user.ToUserDto();
            }
        }


    }
    [YS.Knife.ConvertTo(typeof(UserInfo), typeof(UserDto))]
    partial class Converters
    {

    }

    public class UserInfo
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime Birthday { get; set; }

    }
    public class UserDto
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime Birthday { get; set; }
    }

    public static class Extensions
    {
        static readonly Dictionary<string, MethodInfo> AllMethods = typeof(MapperExtensions)
           .GetMethods(BindingFlags.Public | BindingFlags.Static)
           .Where(m => Attribute.IsDefined(m, typeof(DescriptionAttribute)))
           .ToDictionary(p => p.GetCustomAttribute<DescriptionAttribute>().Description);
        static readonly LocalCache<int, DynamicMethod> MethodCache = new LocalCache<int, DynamicMethod>();
        static readonly LocalCache<int, Delegate> DelegeteCache = new LocalCache<int, Delegate>();
        static DynamicMethod methodInfo;
        public static T2 MyMapTo<T2>(this object t)
        {
            if (t == null) return default(T2);
            var type = t.GetType();

            var key = type.GetHashCode() ^ typeof(T2).GetHashCode();
            if (methodInfo == null)
            {
                DynamicMethod dynamicMethod = new DynamicMethod($"__{key}", typeof(T2), new[] { type });
                var generator = dynamicMethod.GetILGenerator();
                generator.Emit(OpCodes.Ldarg_0);
                var method = AllMethods["map-single"].MakeGenericMethod(type, typeof(T2));
                generator.Emit(OpCodes.Call, method);

                generator.Emit(OpCodes.Ret);
                methodInfo = dynamicMethod;
            }

            return (T2)methodInfo.Invoke(null, new object[] { t });



        }

        public static T2 MyMap<T, T2>(this T t)
        {
            return default(T2);
        }

    }
}
