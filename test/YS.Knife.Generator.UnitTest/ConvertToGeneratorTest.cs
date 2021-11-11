using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace YS.Knife.Generator.UnitTest
{
    public class ConvertToGeneratorTest : BaseGeneratorTest
    {
        [Theory]
        //[InlineData("ConvertToCases/HappyCase.xml")]
        //[InlineData("ConvertToCases/ClassifyConversion.xml")]
        //[InlineData("ConvertToCases/ClassToStruct.xml")]
        //[InlineData("ConvertToCases/StructToClass.xml")]
        //[InlineData("ConvertToCases/StructToStruct.xml")]
        //[InlineData("ConvertToCases/IgnoreTargetProperty.xml")]
        //[InlineData("ConvertToCases/IgnoreNullTargetProperty.xml")]
        //[InlineData("ConvertToCases/IgnoreEmptyTargetProperty.xml")]
        //[InlineData("ConvertToCases/IgnoreNotExistingTargetProperty.xml")]
        //[InlineData("ConvertToCases/CustomerMappings.xml")]
        //[InlineData("ConvertToCases/SubClassToClass.xml")]
        //[InlineData("ConvertToCases/SubStructToStruct.xml")]
        //[InlineData("ConvertToCases/SubStructToClass.xml")]
        //[InlineData("ConvertToCases/SubClassToStruct.xml")]
        //[InlineData("ConvertToCases/CircleRefrence.xml")]
        //[InlineData("ConvertToCases/ListToList.xml")]
        //[InlineData("ConvertToCases/ListToArray.xml")]
        //[InlineData("ConvertToCases/ListToIEnumerable.xml")]
        //[InlineData("ConvertToCases/ListToIQueryable.xml")]
        //[InlineData("ConvertToCases/ListToIList.xml")]
        //[InlineData("ConvertToCases/ListToICollection.xml")]

        //[InlineData("ConvertToCases/ArrayToList.xml")]
        //[InlineData("ConvertToCases/ArrayToArray.xml")]
        //[InlineData("ConvertToCases/ArrayToIEnumerable.xml")]
        //[InlineData("ConvertToCases/ArrayToIQueryable.xml")]
        //[InlineData("ConvertToCases/ArrayToIList.xml")]
        //[InlineData("ConvertToCases/ArrayToICollection.xml")]

        //[InlineData("ConvertToCases/ArrayClassToListStruct.xml")]
        //[InlineData("ConvertToCases/ArrayClassToArrayStruct.xml")]
        //[InlineData("ConvertToCases/ArrayClassToIEnumerableStruct.xml")]
        //[InlineData("ConvertToCases/ArrayClassToIQueryableStruct.xml")]
        //[InlineData("ConvertToCases/ArrayClassToIListStruct.xml")]
        //[InlineData("ConvertToCases/ArrayClassToICollectionStruct.xml")]


        //[InlineData("ConvertToCases/ArrayStructToListClass.xml")]
        //[InlineData("ConvertToCases/ArrayStructToArrayClass.xml")]
        //[InlineData("ConvertToCases/ArrayStructToIEnumerableClass.xml")]
        //[InlineData("ConvertToCases/ArrayStructToIQueryableClass.xml")]
        //[InlineData("ConvertToCases/ArrayStructToIListClass.xml")]
        //[InlineData("ConvertToCases/ArrayStructToICollectionClass.xml")]
        
        [InlineData("ConvertToCases/SourceParentClassProperty.xml")]
        [InlineData("ConvertToCases/TargetParentClassProperty.xml")]
        [InlineData("ConvertToCases/OverwriteParentClassProperty.xml")]
        
        //[InlineData("ConvertToCases/AllInOne.xml")]

        public void ShouldGenerateConverterClass(string testCaseFileName)
        {

            var assemblies = new[]
            {
                typeof(Binder).GetTypeInfo().Assembly,
                typeof(ConvertToAttribute).GetTypeInfo().Assembly,
                typeof(IQueryable<>).GetTypeInfo().Assembly,
                Assembly.GetExecutingAssembly()
            };
            base.ShouldGenerateExpectCodeFile(new ConvertToGenerator(), testCaseFileName, assemblies);
        }
    }
   
}


