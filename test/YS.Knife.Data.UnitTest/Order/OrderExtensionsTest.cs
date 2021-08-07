using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Data.Mappers;
using YS.Knife.Data.Query;

namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class OrderExtensionsTest
    {
        [TestMethod]
        public void ShouldOrderByWhenGiveIdAsc()
        {
            var query = CreateTestUsers().DoOrderBy(OrderInfo.Parse("Id"));
            JoinIds(query).Should().Be("001,002,003,004");
        }

        [TestMethod]
        public void ShouldOrderByWhenGiveIdDesc()
        {
            var query = CreateTestUsers().DoOrderBy(OrderInfo.Parse("Id.desc()"));
            JoinIds(query).Should().Be("004,003,002,001");
        }

        [TestMethod]
        public void ShouldOrderByGiveAgeDescAndNameAsc()
        {
            var orderInfo = OrderInfo.Parse("Age.desc(),Name.asc()");
            var query = CreateTestUsers().DoOrderBy(orderInfo);
            JoinIds(query).Should().Be("002,003,004,001");
        }

        [TestMethod]
        public void ShouldOrderByGiveAgeAscAndNameDesc()
        {
            var orderInfo = OrderInfo.Parse("Age.Asc(),Name.desc()");
            var query = CreateTestUsers().DoOrderBy(orderInfo);
            JoinIds(query).Should().Be("001,004,003,002");
        }

        [TestMethod]
        public void ShouldWhenOrderByWithMapperAndGiveAgeAscAndNameDesc()
        {
            var mapper = new ObjectMapper<User, UserDto>();
            mapper.Append(p => p.TAge, p => p.Age);
            mapper.Append(p => p.TId, p => p.Id);
            mapper.Append(p => p.TName, p => p.Name);
            mapper.Append(p => p.TNameLength, p => p.Name.Length);
            var orderInfo = OrderInfo.Parse("TAge.Asc(),TName.desc()");
            var query = CreateTestUsers().DoOrderBy(orderInfo, mapper);
            JoinIds(query).Should().Be("001,004,003,002");
        }

        [TestMethod]
        public void ShouldWhenOrderByWithMapperAndGiveNameLengthDescAndNameAsc()
        {
            var mapper = new ObjectMapper<User, UserDto>();
            mapper.Append(p => p.TAge, p => p.Age);
            mapper.Append(p => p.TId, p => p.Id);
            mapper.Append(p => p.TName, p => p.Name);
            mapper.Append(p => p.TNameLength, p => p.Name.Length);
            var orderInfo = OrderInfo.Parse("TNameLength.desc(),TName");
            var query = CreateTestUsers().DoOrderBy(orderInfo, mapper);
            JoinIds(query).Should().Be("004,001,003,002");
        }

        private IQueryable<User> CreateTestUsers()
        {
            return new List<User>
            {
                new User {Id = "001", Name = "ZhangSan", Age = 19},
                new User {Id = "002", Name = "lisi", Age = 20},
                new User {Id = "003", Name = "wangWu", Age = 20},
                new User {Id = "004", Name = "WangMaZi", Age = 19}
            }.AsQueryable();
        }

        private string JoinIds(IQueryable<User> sources)
        {
            return string.Join(",", sources.Select(p => p.Id));
        }

        public class User
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }

        }

        public class UserDto
        {
            public string TId { get; set; }
            public string TName { get; set; }
            public int TAge { get; set; }
            public int TNameLength { get; set; }
        }

        [TestMethod]
        public void ShouldDoOrderWhenOrderByMultipleLambdas()
        {
            var query = CreateTestUsers().DoOrderBy(
                (Lambda<User>.Select(p => p.Age), OrderType.Desc),
                (Lambda<User>.Select(p => p.Name), OrderType.Asc));

            JoinIds(query).Should().Be("002,003,004,001");
        }

        [TestMethod]
        public void ShouldDoOrderWhenOrderByMultipleLambdasAndWithFunctions()
        {
            var query = CreateTestUsers().DoOrderBy(
                (Lambda<User>.Select(p => p.Name.ToUpper()), OrderType.Asc));

            JoinIds(query).Should().Be("002,004,003,001");
        }

        [TestMethod]
        public void ShouldDoNothingWhenOrderByEmptyLambdas()
        {
            var query = CreateTestUsers().DoOrderBy();

            JoinIds(query).Should().Be("001,002,003,004");
        }

        public class Lambda<TSource>
        {
            [DebuggerHidden]
            public static Expression<Func<TSource, TKey>> Select<TKey>(Expression<Func<TSource, TKey>> select)
            {
                return select;
            }
        }
    }
}
