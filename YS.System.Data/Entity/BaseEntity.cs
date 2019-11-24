using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace System.Data.Entity
{
    public class BaseEntity : ICreateTrack, IRemark//,ISequence
    {
        public BaseEntity()
        {
            
        }
        public DateTimeOffset CreateTime
        {
            get; set;
        }

        public string CreateUser
        {
            get; set;
        }

        public string Remark
        {
            get; set;
        }

        //public int Sequence
        //{
        //    get;set;
        //}
    }
    public class BaseEntity<T> : BaseEntity, IId<T>
    {
        public T Id
        {
            get;set;
        }
    }
    public class BaseNamedEntity<T> : BaseEntity<T>, IName
    {
        public string Name { get; set; }
    }
    public class LoopRecordBase :  BaseEntity, ILoopRecord
    {
        /// <summary>
        /// 表示要检索发送的时间
        /// </summary>
        public DateTime NextSendTime { get; set; }
        /// <summary>
        /// 表示完成时间
        /// </summary>
        public DateTime? SuccessTime { get; set; }
        /// <summary>
        /// 表示上次失败的时间
        /// </summary>
        public DateTime? LastFailureTime { get; set; }
        /// <summary>
        /// 表示上次失败的消息
        /// </summary>
        public string LastFailureMessage { get; set; }
        /// <summary>
        /// 表示发送失败的次数
        /// </summary>
        public int FailureCount { get; set; }

        /// <summary>
        /// 表示最晚发送时间，超过此时间后不再发送
        /// </summary>
        public DateTime? MaxSendTime { get; set; }
        /// <summary>
        /// 表示是否成功处理
        /// </summary>
        public bool IsSuccess
        {
            get; set;
        }
    }

    public interface ILoopRecord
    {
        /// <summary>
        /// 表示要检索发送的时间
        /// </summary>
        DateTime NextSendTime { get; set; }
        /// <summary>
        /// 表示完成时间
        /// </summary>
        DateTime? SuccessTime { get; set; }
        /// <summary>
        /// 表示是否发送成功
        /// </summary>
        bool IsSuccess { get; set; }
        /// <summary>
        /// 表示上次失败的时间
        /// </summary>
        DateTime? LastFailureTime { get; set; }
        /// <summary>
        /// 表示上次失败的消息
        /// </summary>
        string LastFailureMessage { get; set; }
        /// <summary>
        /// 表示发送失败的次数
        /// </summary>
        int FailureCount { get; set; }
        /// <summary>
        /// 表示最晚执行的时间
        /// </summary>
        DateTime? MaxSendTime { get; set; }
    }

    public class DefaultSendTimeBuilder : ISendTimeBuilder
    {

        private static DefaultSendTimeBuilder instance;

        private static object instancelock = new object();

        public DefaultSendTimeBuilder()
        {
        }

        public static DefaultSendTimeBuilder Instance
        {
            get
            {
                if (object.ReferenceEquals(instance, null))
                {
                    lock (instancelock)
                    {
                        if (object.ReferenceEquals(instance, null))
                        {
                            instance = new DefaultSendTimeBuilder();
                        }
                    }
                }
                return instance;
            }
        }

        public DateTime GetNextSendTime(int failureCount)
        {
            if (failureCount <= 0) return DateTime.Now;
            var spanminutes = Math.Pow(2, failureCount - 1);
            spanminutes = Math.Min(spanminutes, 60 * 24 * 365 * 10);//最多10年
            return DateTime.Now.AddMinutes(spanminutes);
        }
    }
    public interface ISendTimeBuilder
    {
        DateTime GetNextSendTime(int failureCount);
    }
    /// <summary>
    /// 表示域（组织，相互隔离的组织，机构）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Domain<T> : BaseEntity, ISelfTree<T>
    {
        public T ParentId
        {
            get;set;
        }
    }

    public class TagInfo
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
    public interface ITag
    {
        List<TagInfo> Tags { get; set; }
    }

}
