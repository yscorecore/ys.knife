using YS.Knife.Aop;

namespace YS.Knife.Aop.StringResources
{
    [StringResources]
    public interface I18NWithResx
    {
        [Sr("A001", "code template first")]
        string NotConfigKeyInI18NResx();

        [Sr("A002", "code template will ignore")]
        string ConfigEmptyTemplateInI18NResx();

        [Sr("A003", "config some value in i18n resx")]
        string ConfigSomeValueInI18NResx();

        [Sr("A004", "config name template value in i18n resx")]
        string ConfigNameTemplateValueInI18NResx(int val);

        [Sr("A005", "config index template value in i18n resx")]
        string ConfigIndexTemplateValueInI18NResx(int val);
    }
}
