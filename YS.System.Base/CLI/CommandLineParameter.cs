namespace System.CLI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    internal class CommandLineParameter
    {
        internal CommandLineParameter(PropertyInfo property, CommandLineParameterAttribute attribute)
        {
            this.Property = property;
            this.Attribute = attribute;
            this.Command = this.Attribute.Command;

            // Set the defaults
            if (property.PropertyType == typeof(bool))
            {
                // If no switch is specified for bool properties then the name is the switch
                if (string.IsNullOrWhiteSpace(this.Command))
                {
                    this.Command = property.Name;
                }
            }
        }

        internal string Command { get; set; }

        internal PropertyInfo Property { get; set; }

        internal CommandArgument Argument { get; set; }

        internal CommandLineParameterAttribute Attribute { get; private set; }

        internal bool ArgumentSupplied { get; set; }

        internal bool RequiredArgumentSupplied
        {
            get
            {
                return !this.Attribute.Required || this.ArgumentSupplied;
            }
        }

        public string Key
        {
            get
            {
                return this.IsCommand()
                           ? CommandLine.CaseSensitive
                                 ? this.Command
                                 : this.Command.ToLowerInvariant()
                           : CommandLineParameterAttribute.GetParameterKey(this.Attribute.ParameterIndex);
            }
        }

        internal bool IsParameter()
        {
            return !this.IsCommand();
        }

        internal bool IsCommand()
        {
            return !string.IsNullOrWhiteSpace(this.Command);
        }

        public void SetDefaultValue(object argument)
        {
            if (this.Attribute == null || this.Attribute.Default == null)
            {
                return;
            }

            var property = argument.GetType().GetProperty(this.Property.Name);
            // property.SetValue(argument, this.Attribute.Default, null);
            property.SetValue(argument, Convert.ChangeType(this.Attribute.Default, property.PropertyType), null);//20150717 ÑîÅô²©ÐÞ¸Ä
        }

        private object GetPropertyValue(CommandArgument cmd , Type conversiontype)
        {
            if (conversiontype == typeof(string))
            {
                return cmd.Value;
            }
            else if (conversiontype == typeof(bool))
            {
                return GetBoolValue(cmd);
            }
            else
            {
                if (string.IsNullOrEmpty(cmd.Value))
                {
                    try
                    {
                        return Convert.ChangeType(cmd.Value, conversiontype);
                    }
                    catch (Exception ex)
                    {
                        throw new CommandLineException(new CommandArgumentHelp(this.Argument.GetType(), string.Format("Unsupported property type {0}", conversiontype)), ex);
                    }
                }
                return null;
            }
        }

        public void SetValue(object argument, CommandArgument cmd)
        {
            // Argument already supplied
            if (!this.IsCollection() && this.ArgumentSupplied)
            {
                throw new CommandLineArgumentInvalidException(argument.GetType(), cmd);
            }

            this.Argument = cmd;

            #region 20150717 ÑîÅô²©ÐÞ¸Ä
            if (this.IsCollection())
            {
                var list = (IList)(this.Property.GetValue(argument, null) ?? Activator.CreateInstance(this.Property.PropertyType));
                var itemType = this.Property.PropertyType.GetGenericArguments()[0];
                var val = GetPropertyValue(cmd, itemType);
                if (val != null) { list.Add(val); }
                if (this.Property.CanWrite)
                    this.Property.SetValue(argument, list, null);
            }
            else
            {
                var val = GetPropertyValue(cmd, this.Property.PropertyType);
                if (val != null)
                {
                    this.Property.SetValue(argument, val, null);
                }
            }
            //if (this.Property.PropertyType == typeof(bool))
            //{
            //    this.Property.SetValue(argument, GetBoolValue(cmd), null);
            //}
            //else if (this.Property.PropertyType == typeof(int))
            //{
            //    this.Property.SetValue(argument, Convert.ToInt32(cmd.Value), null);
            //}
            //else if (this.Property.PropertyType == typeof(DateTime))
            //{
            //    this.Property.SetValue(argument, Convert.ToDateTime(cmd.Value), null);
            //}
            //else if (this.Property.PropertyType == typeof(string))
            //{
            //    this.Property.SetValue(argument, cmd.Value, null);
            //}
            //else if (this.Property.PropertyType == typeof(List<string>))
            //{
            //    var list = (List<string>)this.Property.GetValue(argument, null) ?? new List<string>();
            //    list.Add(cmd.Value);
            //    this.Property.SetValue(argument, list, null);
            //}
            //else
            //{
            //    throw new CommandLineException(new CommandArgumentHelp(argument.GetType(), string.Format("Unsupported property type {0}", this.Property.PropertyType)));
            //}
            #endregion
           

            this.ArgumentSupplied = true;
        }

        private bool IsCollection()
        {
            return typeof(List<>).IsAssignableFrom(this.Property.PropertyType);
        }

        /// <summary>
        ///   Returns a boolean value from a command switch
        /// </summary>
        /// <param name = "cmd">The command switch</param>
        /// <returns>A boolean value based on the switch and value</returns>
        private static bool GetBoolValue(CommandArgument cmd)
        {
            return string.IsNullOrWhiteSpace(cmd.SwitchOption) || cmd.SwitchOption.Trim() == "+";
        }
    }
}