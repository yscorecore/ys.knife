﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Data.Query;

namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class OrderExtensionsTest
    {
        //[TestMethod]
        //public void ShouldOrderByWhenGiveIdAsc()
        //{
        //    var data = CreateTestUsers().Order(new OrderItem("Id", OrderType.Asc))
        //         .DoSelect(p => p.Id).ToList();
        //    Assert.AreEqual("001,002,003,004", string.Join(",", data));
        //}
        //[TestMethod]
        //public void ShouldOrderByWhenGiveIdDesc()
        //{
        //    var data = CreateTestUsers().Order(new OrderItem("Id", OrderType.Desc))
        //         .DoSelect(p => p.Id).ToList();
        //    Assert.AreEqual("004,003,002,001", string.Join(",", data));
        //}

        //[TestMethod]
        //public void ShouldOrderByGiveAgeDescAndNameAsc()
        //{
        //    var orderInfo = new OrderInfo()
        //        .Add("Age", OrderType.Desc)
        //        .Add("Name", OrderType.Asc);
        //    var data = CreateTestUsers().Order(orderInfo)
        //         .DoSelect(p => p.Id).ToList();
        //    Assert.AreEqual("002,003,004,001", string.Join(",", data));
        //}

        //[TestMethod]
        //public void ShouldOrderByGiveAgeAscAndNameDesc()
        //{
        //    var orderInfo = new OrderInfo()
        //        .Add("Age", OrderType.Asc)
        //        .Add("Name", OrderType.Desc);
        //    var data = CreateTestUsers().Order(orderInfo)
        //         .DoSelect(p => p.Id).ToList();
        //    Assert.AreEqual("001,004,003,002", string.Join(",", data));
        //}

        private IQueryable<User> CreateTestUsers()
        {
            return new List<User>
            {
                new User{ Id="001",Name="ZhangSan",Age=19 },
                new User{ Id="002",Name="lisi",Age=20 },
                new User{ Id="003",Name="wangWu",Age=20 },
                new User{ Id="004",Name="WangMaZi",Age=19 }
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
