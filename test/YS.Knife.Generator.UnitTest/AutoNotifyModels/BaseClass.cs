using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Generator.UnitTest.AutoNotifyModels
{
    public class BaseClass : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;


    }

}
