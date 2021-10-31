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
        string CreatedBy { get; set; }
        /// <summary>
        /// 表示创建时间
        /// </summary>
        DateTimeOffset CreatedTime { get; set; }
    }
    public interface IUpdateTrack
    {
        /// <summary>
        /// 表示更新时间
        /// </summary>
        DateTimeOffset? UpdatedTime { get; set; }
        /// <summary>
        /// 表示更新用户
        /// </summary>
        string UpdatedBy { get; set; }
    }
    /// <summary>
    /// 表示审核跟踪
    /// </summary>
    public interface IAuditTrack
    {
        /// <summary>
        /// 表示审核时间
        /// </summary>
        DateTimeOffset? AuditTime { get; set; }
        /// <summary>
        /// 表示审核用户
        /// </summary>
        string AuditUser { get; set; }
    }

    /// <summary>
    /// 表示是否删除
    /// </summary>
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
    public interface IDeleteTrack : ISoftDelete
    {
        /// <summary>
        /// 表示删除的用户
        /// </summary>
        string DeletedBy { get; set; }
        /// <summary>
        /// 表示删除时间
        /// </summary>
        DateTimeOffset? DeletedTime { get; set; }

    }
    /// <summary>
    /// 表示恢复删除的记录
    /// </summary>
    public interface IResumeTrack : IDeleteTrack, ISoftDelete
    {
        /// <summary>
        /// 表示恢复删除的用户
        /// </summary>
        string ResumedBy { get; set; }
        /// <summary>
        /// 表示恢复删除的时间
        /// </summary>
        DateTimeOffset? ResumedTime { get; set; }
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

    public interface ITenantEntity<T>
    {
        T TenantId { get; set; }
    }
    public abstract class BaseEntity<TKey> : IId<TKey>, ICreateTrack, IUpdateTrack
    {
        public virtual TKey Id { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTimeOffset CreatedTime { get; set; }
        public virtual DateTimeOffset? UpdatedTime { get; set; }
        public virtual string UpdatedBy { get; set; }
    }
    public abstract class BaseTenantEntity<TKey> : BaseEntity<TKey>, ITenantEntity<TKey>
    {
        public TKey TenantId { get; set; }
    }
}
