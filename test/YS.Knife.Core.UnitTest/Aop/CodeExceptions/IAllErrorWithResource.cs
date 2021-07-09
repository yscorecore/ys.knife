using System;

namespace YS.Knife.Aop.CodeExceptions
{
    [CodeExceptions("20000")]
    public interface IAllErrorWithResource
    {
        [Ce("1", "code template first")]
        Exception NotConfigKeyInI18NResx();

        [Ce("2", "code template will ignore")]
        Exception ConfigEmptyTemplateInI18NResx();

        [Ce("3", "config some value in i18n resx")]
        Exception ConfigSomeValueInI18NResx();

        [Ce("4", "config name template value in i18n resx")]
        Exception ConfigNameTemplateValueInI18NResx(int val);

        [Ce("5", "config index template value in i18n resx")]
        Exception ConfigIndexTemplateValueInI18NResx(int val);
    }
}
