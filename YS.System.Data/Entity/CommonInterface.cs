using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Entity
{
    /// <summary>
    /// 表示排序字段
    /// </summary>
    public interface ISequence
    {
        int Sequence { get; set; }
    }
    /// <summary>
    /// 表示备注字段
    /// </summary>
    public interface IRemark
    { 
         string Remark { get; set; }
    }
    public interface ICreateTrack
    {
        /// <summary>
        /// 表示创建用户
        /// </summary>
        string CreateUser { get; set; }
        /// <summary>
        /// 表示创建时间
        /// </summary>
        DateTimeOffset CreateTime { get; set; }
    }
    public interface IUpdateTrack
    {
        /// <summary>
        /// 表示更新时间
        /// </summary>
        DateTime? UpdateTime { get; set; }
        /// <summary>
        /// 表示更新用户
        /// </summary>
        string UpdateUser { get; set; }
    }

    /// <summary>
    /// 表示审核跟踪
    /// </summary>
    public interface IAuditTrack
    {
        /// <summary>
        /// 表示审核时间
        /// </summary>
        DateTime? AuditTime { get; set; }
        /// <summary>
        /// 表示审核用户
        /// </summary>
        string AuditUser { get; set; }
    }
    /// <summary>
    /// 表示发布跟踪
    /// </summary>
    public interface IPublishTrack
    {
        /// <summary>
        /// 表示发布时间
        /// </summary>
        DateTime? PublishTime { get; set; }
        /// <summary>
        /// 表示发布用户
        /// </summary>
        string PublishUser { get; set; }
    }
    /// <summary>
    /// 表示是否删除
    /// </summary>
    public interface IFalseDelete
    {
        bool IsDeleted { get; set; }
    }
    public interface IDeleteTrack: IFalseDelete
    {
        /// <summary>
        /// 表示删除的用户
        /// </summary>
        string DeleteUser { get; set; }
        /// <summary>
        /// 表示删除时间
        /// </summary>
        DateTime? DeleteTime { get; set; }

    }
    /// <summary>
    /// 表示恢复删除的记录
    /// </summary>
    public interface IResumeTrack : IDeleteTrack, IFalseDelete
    {
        /// <summary>
        /// 表示恢复删除的用户
        /// </summary>
        string ResumeUser { get; set; }
        /// <summary>
        /// 表示恢复删除的时间
        /// </summary>
        DateTime? ResumeTime { get; set; }
    }
    /// <summary>
    /// 表示RowVersion
    /// </summary>
    public interface IRowVersion
    {
        byte[] RowVersion { get; set; }
    }

    /// <summary>
    /// 表示树状结构的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISelfTree<T>
    {
       T ParentId{ get; set; }
    }
    public interface IId<T> {
        T Id { get; set; }
    }
    public interface IName
    {
        [System.ComponentModel.DataAnnotations.StringLength(32)]
        string Name { get; set; }
    }

    /// <summary>
    /// 表示域数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDomainData<T>
    {
        T DomainId { get; set; }
    }




    /// <summary>
    /// 表示属性描述
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityProperty<T>: BaseNamedEntity<T>
    {
        public string Description { get; set; }

        public string Type { get; set; }//String，Number，Date，Tel，Url，Email，Bool，single，mutil

        public T SchameId { get; set; }

        public string DefaultValue { get; set; }

    }
    /// <summary>
    /// 表示实体属性的验证器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityPropertyValidation<T>: BaseEntity<T>
    {
        public T PropertyId { get; set; }
        public string ValidateType { get; set; }
        public string ErrorMessage { get; set; }
        public string Arguments { get; set; }
    }

    /// <summary>
    /// 表示实体的架构信息，可继承
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntitySchame<T> : BaseEntity<T>, ISelfTree<T>
    {
        public T ParentId
        {
            get;set;
        }

        public virtual List<EntityProperty<T>> Properties { get; set; }
    }


    public interface IExtentionObject<T>
    {
        List<PropertyValue<T>> Properties { get; set; }

        T SchemaId { get; set; }
    }

    /// <summary>
    /// 表示实体的值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityValue<T> : BaseEntity<T>, IExtentionObject<T>
    {
        public List<PropertyValue<T>> Properties
        {
            get;set;
        }

        public T SchemaId
        {
            get;set;
        }
    }

    /// <summary>
    /// 表示实体的属性值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyValue<T>:IId<T>
    {
        public T Id
        {
            get; set;
        }
        public T EntityId { get; set; }
        public T PropertyId { get; set; }
        public string Value { get; set; }
    }

    
}
