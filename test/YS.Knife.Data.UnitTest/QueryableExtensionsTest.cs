using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Data.Query;

namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class QueryableExtensionsTest
    {
        [TestMethod, TestCategory("ListAll")]
        public void ShouldGetListWhenListAllWithOutQueryInfo()
        {
            var users = CreateUsersWithAddress();
            var actual = users.ListAll(null);
            actual.Should().BeEquivalentTo(users.ToList());
        }

        [TestMethod, TestCategory("ListAll")]
        public void ShouldGetListWhenListAllWithFilterInfoAndNoOrderInfo()
        {
            var users = CreateUsersWithAddress();
            var actual = users.ListAll(FilterInfo.CreateItem("Name", Operator.Contains, "a"), null, null);
            actual.Should().BeEquivalentTo(users.Where(p => (p.Name != null && p.Name.Contains("a"))).ToList());
        }

        [TestMethod, TestCategory("ListAll")]
        public void ShouldGetOriginListWhenListAllWithNoFilterInfoAndOrderInfo()
        {
            var users = CreateUsersWithAddress();
            var actual = users.ListAll(null, OrderInfo.Parse("Name.desc()"), null);
            actual.Should().BeEquivalentTo(users.OrderByDescending(p => p.Name).ToList());
        }

        [TestMethod, TestCategory("ListAll")]
        public void ShouldGetListWhenListAllWithFilterInfoAndOrderInfo()
        {
            var users = CreateUsersWithAddress();
            var actual = users.ListAll(FilterInfo.CreateItem("Name", Operator.Contains, "a"),
                OrderInfo.Parse("Name.Desc()"), null);
            actual.Should().BeEquivalentTo(users.Where(p => (p.Name != null && p.Name.Contains("a")))
                .OrderByDescending(p => p.Name).ToList());
        }


        // [TestMethod, TestCategory("ListAll2")]
        // public void ShouldGetOriginListWhenListAll2WithOutQueryInfo()
        // {
        //     var users = CreateUsersWithAddress();
        //     var actual = users.ListAll(null,null, UserMapper);
        //     actual.Should().BeEquivalentTo( UserMapper(users).ToList());
        // }

        // private IQueryable<User2> UserMapper(IQueryable<User> users)
        // {
        //     return users.DoSelect(p => new User2()
        //     {
        //          Id = p.Id,
        //          Name = p.Name,
        //          Age = p.Age,
        //          FirstAddressCity = p.Addresses.DoSelect(c=>c.City).FirstOrDefault()
        //     });
        // }

        private IQueryable<User> CreateUsersWithAddress()
        {
            return (new List<User>()
            {
                new User()
                {
                    Id = "001",
                    Name = "ZhangSan",
                    Age = 19,
                    Score = 61,
                    Addresses = new List<Address>() {new Address() {City = "xian"}}
                },
                new User()
                {
                    Id = "002",
                    Name = "LiSi",
                    Age = 20,
                    Score = 81,
                    Addresses = new List<Address>() {new Address() {City = "beijing"}, new Address() {City = "xian"}}
                },
                new User()
                {
                    Id = "003",
                    Name = "WangWu",
                    Age = 20,
                    Score = 70,
                    Addresses = new List<Address>() {new Address() {City = "beijing"}}
                },
                new User()
                {
                    Id = "004",
                    Name = "WangMaZi",
                    Age = 19,
                    Addresses = new List<Address>() {new Address() {City = "xian"}, new Address() {City = "xian"}}
                },
                new User()
                {
                    Id = "005",
                    Name = null,
                    Age = 21,
                    Addresses = new List<Address>() {new Address() {City = "beijing"}, new Address() {City = "beijing"}}
                }
            }).AsQueryable();
        }

        class User
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }

            public int? Score { get; set; }

            public List<Address> Addresses { get; set; } = new List<Address>();
        }

        class Address
        {
            public string City { get; set; }
        }

        class User2
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public string FirstAddressCity { get; set; }
        }
    }
}
