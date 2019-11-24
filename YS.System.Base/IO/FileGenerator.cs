using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO
{
    /// <summary>
    /// 表示文件的生成器的基类
    /// </summary>
    public abstract class FileGenerator : System.ComponentModel.Component
    {
        private FileGeneratorContext FileMakerContext { get; set; }
        public string OutputFile { get; set; }
        public void GenerateFile()
        {
            FileMakerContext = new FileGeneratorContext();
            this.InitContext(FileMakerContext);
            this.GenerateFile(FileMakerContext);
        }
        protected virtual void InitContext(FileGeneratorContext context)
        {

        }
        protected abstract void GenerateFile(FileGeneratorContext context);
    }
}
