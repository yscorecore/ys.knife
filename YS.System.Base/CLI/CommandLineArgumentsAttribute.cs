namespace System.CLI
{
    using System;
    using System.Reflection;
      using System.Linq;
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandLineArgumentsAttribute : Attribute
    {
        public string Title { get; set; }

        public virtual string Description { get; set; }

        public string Program { get; set; }

        /// <summary>
        ///   Returns a CommandLineArgumentsAttribute
        /// </summary>
        /// <param name = "member"></param>
        /// <returns></returns>
        public static CommandLineArgumentsAttribute Get(MemberInfo member)
        {
            return GetCustomAttributes(member, typeof(CommandLineArgumentsAttribute)).Cast<CommandLineArgumentsAttribute>().FirstOrDefault();
        }
    }
}