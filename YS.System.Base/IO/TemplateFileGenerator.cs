using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO
{
    /// <summary>
    /// 表示基于模板的文件生成器的基类
    /// </summary>
    public abstract class TemplateFileGenerator : FileGenerator
    {
        /// <summary>
        /// 获取文件的模板位置
        /// </summary>
        public virtual string TemplateFile
        {
            get
            {
                return this.GetType().GetFileResourcePath();
            }
        }
    }
}
