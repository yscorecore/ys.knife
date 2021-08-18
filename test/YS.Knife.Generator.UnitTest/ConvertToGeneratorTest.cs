﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace YS.Knife.Generator.UnitTest
{
    public class ConvertToGeneratorTest : BaseGeneratorTest
    {
        [Theory]
        [InlineData("ConvertToCases/HappyCase.xml")]
        public void ShouldGenerateExpectSingletonPaitalClass(string testCaseFileName)
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
