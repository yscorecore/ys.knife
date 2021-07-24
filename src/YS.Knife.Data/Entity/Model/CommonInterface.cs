using System;

namespace YS.Knife.Entity.Model
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
        DateTimeOffset? UpdateTime { get; set; }
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
    /// 表示是否删除
    /// </summary>
    public interface IFalseDelete
    {
        bool IsDeleted { get; set; }
    }
    public interface IDeleteTrack : IFalseDelete
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
    public interface ISelfTree<T> : IId<T>
    {
        T ParentId { get; set; }
        string NodePath { get; set; }
        int PathValue { get; set; }
        int Depth { get; set; }
    }
    public interface IId<T>
    {
        T Id { get; set; }
    }
    public interface IName
    {

        string Name { get; set; }
    }

    /// <summary>
    /// 表示域数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITenantData<T>
    {
        T TenantId { get; set; }
    }
    public class BaseEntity<TKey> : IId<TKey>, ICreateTrack, IUpdateTrack
    {
        public TKey Id { get; set; }
        public string CreateUser { get; set; }
        public DateTimeOffset CreateTime { get; set; }
        public DateTimeOffset? UpdateTime { get; set; }
        public string UpdateUser { get; set; }
    }
    public class BaseTenantEntity<TKey> : BaseEntity<TKey>, ITenantData<TKey>
    {
        public TKey TenantId { get; set; }
    }
}
