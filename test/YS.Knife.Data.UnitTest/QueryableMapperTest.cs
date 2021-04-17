using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class QueryableMapperTest
    {
        [TestMethod]
        public void ShouldMapClassesWithCustomMapper()
        {
            IQueryableMapper.Instance = new UserMapper();
            var user1List = new List<User1> { new User1 { Name = "zhangsan" }, new User1 { Name = "lisi" } };
            var user2List = user1List.AsQueryable().MapType<User1,User2>().ToList();
            var expected = new List<User2> { new User2 { Nm = "zhangsan" }, new User2 { Nm = "lisi" } };
            user2List.Should().BeEquivalentTo(expected);
        }
    }
    class User1
    {
        public string Name { get; set; }
    }
    class User2
    {
        public string Nm { get; set; }
    }
    class UserMapper : IQueryableMapper
    {
        public IQueryable<R> MapType<T, R>(IQueryable<T> from)
        {
            if (from is IQueryable<User1> users && typeof(R) == typeof(User2))
            {
                return (IQueryable<R>)users.Select(p => new User2 { Nm = p.Name });
            }
            throw new NotImplementedException();
        }
    }
}
