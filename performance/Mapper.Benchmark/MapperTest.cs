using System;
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
        public void KnifeMapper()
        {
            for (int i = 0; i < LoopCount; i++)
            {
                var dto = user.Map<UserInfo, UserDto>();
            }
        }
        [Benchmark]
        public void ConvertTo()
        {
            for (int i = 0; i < LoopCount; i++)
            {
                var dto = user.ToUserDto();
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
}
