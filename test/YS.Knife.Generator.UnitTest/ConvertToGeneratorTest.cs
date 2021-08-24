using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace YS.Knife.Generator.UnitTest
{
    public class ConvertToGeneratorTest : BaseGeneratorTest
    {
        [Theory]
        [InlineData("ConvertToCases/HappyCase.xml")]
        [InlineData("ConvertToCases/ClassifyConversion.xml")]
        [InlineData("ConvertToCases/ClassToStruct.xml")]
        [InlineData("ConvertToCases/StructToClass.xml")]
        [InlineData("ConvertToCases/StructToStruct.xml")]
        [InlineData("ConvertToCases/IgnoreTargetProperty.xml")]
        [InlineData("ConvertToCases/IgnoreNullTargetProperty.xml")]
        [InlineData("ConvertToCases/IgnoreEmptyTargetProperty.xml")]
        [InlineData("ConvertToCases/IgnoreNotExistingTargetProperty.xml")]
        [InlineData("ConvertToCases/CustomerMappings.xml")]
        [InlineData("ConvertToCases/SubObjectType.xml")]
        public void ShouldGenerateConverterClass(string testCaseFileName)
        {
            var assemblies = new[]
            {
                typeof(Binder).GetTypeInfo().Assembly,
                typeof(ConvertToAttribute).GetTypeInfo().Assembly,
                Assembly.GetExecutingAssembly()
            };
            base.ShouldGenerateExpectCodeFile(new ConvertToGenerator(), testCaseFileName, assemblies);
        }
    }

   
  
}
namespace ConsoleApp2
{
    public class UserInfo
    {
        public string Name { get; set; }
        public AddressInfo Address { get; set; }
    }
    public class AddressInfo
    {
        public string City { get; set; }
    }

    public partial class TUser
    {
        public string Name { get; set; }
        public AddressDto Address { get; set; }
    }
    public class AddressDto
    {
        public string City { get; set; }
    }
    static partial class Convertors
    {
        public static global::ConsoleApp2.UserInfo ToUserInfo(this global::ConsoleApp2.TUser source)
        {
            if (source == null) return default;
            return new global::ConsoleApp2.UserInfo
            {
                Name = source.Name,
                Address = source.Address == null ? default(global::ConsoleApp2.AddressInfo) : new global::ConsoleApp2.AddressInfo
                {
                    City = source.Address.City,
                },
            };
        }
        public static void ToUserInfo(this global::ConsoleApp2.TUser source, global::ConsoleApp2.UserInfo target)
        {
            if (source == null) return;
            if (target == null) return;
            target.Name = source.Name;
            target.Address = source.Address == null ? default(global::ConsoleApp2.AddressInfo) : new global::ConsoleApp2.AddressInfo
            {
                City = source.Address.City,
            };
        }
        public static IEnumerable<global::ConsoleApp2.UserInfo> ToUserInfo(this IEnumerable<global::ConsoleApp2.TUser> source)
        {
            return source?.Select(p => new global::ConsoleApp2.UserInfo
            {
                Name = p.Name,
                Address = p.Address == null ? default(global::ConsoleApp2.AddressInfo) : new global::ConsoleApp2.AddressInfo
                {
                    City = p.Address.City,
                },
            });
        }
        public static IQueryable<global::ConsoleApp2.UserInfo> ToUserInfo(this IQueryable<global::ConsoleApp2.TUser> source)
        {
            return source?.Select(p => new global::ConsoleApp2.UserInfo
            {
                Name = p.Name,
                Address = p.Address == null ? default(global::ConsoleApp2.AddressInfo) : new global::ConsoleApp2.AddressInfo
                {
                    City = p.Address.City,
                },
            });
        }
    }
}
