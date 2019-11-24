using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
namespace System.Data.Meta
{

    public class EntityInfo
    {
        public EntityInfo()
        {
            this.Props = new List<PropInfo>();
        }
        public List<PropInfo> Props { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

    }
    public enum QueryLevel
    {
        No = 0,
        Simple = 1,
        Advance = 2,
    }
    [Flags()]
    public enum DisplayLevel
    {
        NoShow = 0,
        View = 1,
        List = 2,
        Create = 4,
        Edit = 8,
        NoCreate = View | List | Edit,
        NoEdit = View | List | Create,
        All = View | List | Create | Edit,
    }
    public class DisplayInfo
    {
        //
        // 摘要:
        //     Gets or sets a value that will be used to set the watermark for prompts in the
        //     UI.
        //
        // 返回结果:
        //     A value that will be used to display a watermark in the UI.
        public string Prompt { get; set; }
        //
        // 摘要:
        //     Gets or sets the order weight of the column.
        //
        // 返回结果:
        //     The order weight of the column.
        public int Order { get; set; }
        //
        // 摘要:
        //     Gets or sets a value that is used for display in the UI.
        //
        // 返回结果:
        //     A value that is used for display in the UI.
        public string Name { get; set; }
        //
        // 摘要:
        //     Gets or sets a value that is used to group fields in the UI.
        //
        // 返回结果:
        //     A value that is used to group fields in the UI.
        public string GroupName { get; set; }
        //
        // 摘要:
        //     Gets or sets a value that is used to display a description in the UI.
        //
        // 返回结果:
        //     The value that is used to display a description in the UI.
        public string Description { get; set; }



        //
        // 摘要:
        //     Gets or sets a value that is used for the grid column label.
        //
        // 返回结果:
        //     A value that is for the grid column label.
        public string ShortName { get; set; }

        /// <summary>
        /// 列宽度,20%,20等
        /// </summary>
        public string ColumnWidth { get; set; }


        /// <summary>
        /// 支持查询的等级
        /// </summary>
        public QueryLevel QueryLevel { get; set; }


        public DisplayLevel DisplayLevel { get; set; }

    }
    public class DisplayFormatInfo
    {
        //
        // 摘要:
        //     Gets or sets a value that indicates whether the formatting string that is specified
        //     by the System.ComponentModel.DataAnnotations.DisplayFormatAttribute.DataFormatString
        //     property is applied to the field value when the data field is in edit mode.
        //
        // 返回结果:
        //     true if the formatting string applies to the field value in edit mode; otherwise,
        //     false. The default is false.
        public bool ApplyFormatInEditMode { get; set; }
        //
        // 摘要:
        //     Gets or sets a value that indicates whether empty string values (&quot;&quot;)
        //     are automatically converted to null when the data field is updated in the data
        //     source.
        //
        // 返回结果:
        //     true if empty string values are automatically converted to null; otherwise, false.
        //     The default is true.
        public bool ConvertEmptyStringToNull { get; set; }
        //
        // 摘要:
        //     Gets or sets the display format for the field value.
        //
        // 返回结果:
        //     A formatting string that specifies the display format for the value of the data
        //     field. The default is an empty string (&quot;&quot;), which indicates that no
        //     special formatting is applied to the field value.
        public string DataFormatString { get; set; }
        //
        // 摘要:
        //     Gets or sets a value that indicates whether the field should be HTML-encoded.
        //
        // 返回结果:
        //     true if the field should be HTML-encoded; otherwise, false.
        public bool HtmlEncode { get; set; }
        //
        // 摘要:
        //     Gets or sets the text that is displayed for a field when the field&#39;s value
        //     is null.
        //
        // 返回结果:
        //     The text that is displayed for a field when the field&#39;s value is null. The
        //     default is an empty string (&quot;&quot;), which indicates that this property
        //     is not set.
        public string NullDisplayText { get; set; }

    }
    public class DataInfo
    {
        /// <summary>
        /// 表示数据的单位
        /// </summary>
        public string DataUnit { get; set; }
        /// <summary>
        /// 表示数据的类型
        /// </summary>
        public DataType DataType { get; set; }
        /// <summary>
        /// 表示支持的枚举项列表
        /// </summary>
        public List<SelectInfo> SelectItems { get; set; }
        /// <summary>
        /// 表示数据源
        /// </summary>
        public string DataSource { get; set; }
        /// <summary>
        /// 表示是否必须
        /// </summary>
        public bool IsRequired { get; set; }
        /// <summary>
        /// 是否是主键
        /// </summary>
        public bool IsKey { get; set; }
        /// <summary>
        /// 是否是名称列
        /// </summary>
        public bool IsName { get; set; }

    }
    public class SelectInfo
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }

    public class PropInfo
    {
        public string Name { get; set; }
        /// <summary>
        /// 表示存储字段的类型 string,int,double,float,bool,enum,object
        /// </summary>
        public string FieldTypeCode { get; set; }

        public DisplayInfo DisplayInfo { get; set; }

        public DisplayFormatInfo DisplayFormatInfo { get; set; }

        public DataInfo DataInfo { get; set; }

        public EntityInfo EntityInfo { get; set; }



    }








}
