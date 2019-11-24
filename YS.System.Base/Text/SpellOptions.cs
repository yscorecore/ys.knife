using System;
namespace System.Text
{
    /// <summary>
    /// 提供用于设置汉字转换拼音选项的枚举值。
    /// </summary>
    [Flags]
    public enum SpellOptions
    {
        /// <summary>
        /// 保留非字母、非数字字符，默认不保留
        /// </summary>
        EnableUnicodeLetter = 4,
        /// <summary>
        /// 只转换拼音首字母，默认转换全部
        /// </summary>
        FirstLetterOnly = 1,
        /// <summary>
        /// 转换未知汉字为问号，默认不转换
        /// </summary>
        TranslateUnknowWordToInterrogation = 2
    }
}




