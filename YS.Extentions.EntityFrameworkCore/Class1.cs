using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace YS.Extentions.EntityFrameworkCore
{
    internal class MyCoreConventionSetBuilder : CoreConventionSetBuilder
    {
        public MyCoreConventionSetBuilder(CoreConventionSetBuilderDependencies dependencies):base(dependencies)
        {

        }
        public override ConventionSet CreateConventionSet()
        {
            //var items = CustomAttributeExtensions.GetCustomAttributes<StringLengthAttribute>();
            var conSet= base.CreateConventionSet();

            return conSet;
        }
    }
    public class StringDefaultLengthConvention : IPropertyAddedConvention
    {
        public InternalPropertyBuilder Apply(InternalPropertyBuilder propertyBuilder)
        {
          
            if (propertyBuilder.Metadata.ClrType == typeof(string))
                propertyBuilder.HasMaxLength(32, ConfigurationSource.Convention);
            return propertyBuilder;
        }
    }

  
}
