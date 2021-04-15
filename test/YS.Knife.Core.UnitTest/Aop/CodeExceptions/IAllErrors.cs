using System;

namespace YS.Knife.Aop.CodeExceptions
{
    [CodeExceptions(100000)]
    public interface IAllErrors
    {
        [Ce(1, "no argument return exception.")]
        Exception NoArgumentReturnException();

        [Ce(2, "no argument return application exception.")]
        ApplicationException NoArgumentReturnApplicationException();
        [Ce(3, "no argument return code exception.")]
        CodeException NoArgumentReturnCodeException();

        [Ce(4, "argument exception not supported.")]
        ArgumentException ReturnArgumentException();

        [Ce(5, "the argument value is {arg}.")]
        Exception WithNameArgument(string arg);

        [Ce(6, "the argument value is {0}.")]
        Exception WithIndexArgument(string arg);

        [Ce(7, "the argument value is {arg}.")]
        Exception WithNameArgumentAndHasDefaultValue(string arg = "abc");

        [Ce(8, "the argument value is {arg:d3}.")]
        Exception WithNameArgumentAndHasFormat(int arg);

        [Ce(9, "the argument value is {arg,5:d3}.")]
        Exception WithNameArgumentAndHasFormatAndWidth(int arg);

        [Ce(10, "the argument value is {arg,-5:d3}.")]
        Exception WithNameArgumentAndHasFormatAndNegativeWidth(int arg);

        [Ce(11, "first value is {arg1:d3}, second value is {1:d3}.")]
        Exception MixedNameArgumentAndIndexArgument(int arg1, int arg2);

        [Ce(12, "value is {arg}.")]
        Exception MissingNameArgument();

        [Ce(13, "value is {0}.")]
        Exception MissingIndexArgument();
        [Ce(14, "Value '{arg}' Error '{ex}'.")]
        Exception WithInnerExceptionAndHasNameArgument(Exception ex, string arg);

        [Ce(15, "Value '{1}' Error '{0}'.")]
        Exception WithInnerExceptionAndHasIndexArgument(Exception ex, string arg);


        [Ce(16, null)]
        Exception NullTemplate(string arg);
        Exception NoDefineCeAttributeWillReturnNull();
    }
}
