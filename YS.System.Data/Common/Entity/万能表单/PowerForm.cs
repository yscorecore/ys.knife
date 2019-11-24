using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace System.Common.Forms
{
    /// <summary>
    /// 表示万能表单(投票，调查问卷，收集数据等)
    /// </summary>
    public class PowerForm : BaseEntity<Guid>
    {
        public string FormName { get; set; }
        public string FormDescription { get; set; }

    }

    public class FormItem : BaseEntity, ISequence
    {

        public Guid ItemID { get; set; }
        public Guid FormID { get; set; }
        public string ItemName { get; set; }
        public string ItemDisplayName { get; set; }
        public ItemType ItemType { get; set; }
        public ItemKind ItemKind { get; set; }
        public Guid? ValueGroupID { get; set; }
        public int Sequence
        {
            get; set;
        }
    }

    public class FormValueGroupItem : ISequence
    {
        public Guid ValueItemID { get; set; }
        public Guid GroupID { get; set; }

        public string Value { get; set; }

        public string DisplayValue { get; set; }

        public int Sequence { get; set; }
    }

    public class FormResult : BaseEntity
    {
        public Guid ResultID { get; set; }
        public Guid FormID { get; set; }


    }

    public class FormResultDetails
    {
        public Guid ResultID { get; set; }
        public Guid ItemID { get; set; }
        public string Value { get; set; }
        public Guid? ValueItemID { get; set; }

    }


    public enum ItemType
    {
        String,
        Int,
        DateTime,
        Date,
        Email,
        Tel,
        Decimal,
        ImageUrl,
    }
    public enum ItemKind
    {
        Text,
        Select,
        RadioList,
        CheckBoxList
    }

}
