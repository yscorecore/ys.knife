using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace System.Data.Common
{
    public class ResourceInfo: BaseEntity<Guid>,ISequence,IDomainData<Guid>,ISelfTree<Guid>
    {
        /// <summary>
        /// 表示资源的ID
        /// </summary>
        public Guid ResourceID { get; set; }
        /// <summary>
        /// 资源的名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 表示资源是否可见
        /// </summary>
        public bool Visible { get; set; }
        /// <summary>
        /// 表示资源的代码
        /// </summary>
        public string ResourceCode { get; set; }
        /// <summary>
        /// 父资源的ID
        /// </summary>
        public Guid ParentId { get; set; }
        ///// <summary>
        ///// 表示资源的拓展属性
        ///// </summary>
        //public virtual List<ResourcePropertyValue> ExtentionValues { get; set; }
        /// <summary>
        /// 表示资源是否存在关联的内容
        /// </summary>
        public bool HasContent { get; set; }
        /// <summary>
        /// 表明是否是集合资源
        /// </summary>
        public bool IsCollection { get; set; }


        public Guid DomainId
        {
            get;set;
        }

        public int Sequence
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// 表示链接
    /// </summary>
    public class LinkInfo:ResourceInfo
    {
        
        public string LinkUrl { get; set; }
        public string Title { get; set; }
        public string Target { get; set; }
        public string ImageUrl { get; set; }

    }
    /// <summary>
    /// 表示目录信息
    /// </summary>
    public class CatagoryInfo : ResourceInfo
    {
        public CatagoryKind CatagoryKind { get; set; }
        /// <summary>
        /// 表示关联的内容
        /// </summary>
        public ContentInfo ContentInfo { get; set; }
        /// <summary>
        /// 表示跳转的链接
        /// </summary>
        public LinkInfo LinkInfo { get; set; }
    }

    /// <summary>
    /// 表示内容项(网站新闻资讯文章等)
    /// </summary>
    public class ContentInfo : ResourceInfo
    {
        public ContentInfo()
        {
            this.Images = new List<ContentImage>();
            this.TitleImages = new List<ContentImage>();
        }
        /// <summary>
        /// 表示ContentID
        /// </summary>
        public Guid ContentID { get; set; }
        /// <summary>
        /// 表示配置的ID
        /// </summary>
        public Guid ConfigID { get; set; }
        /// <summary>
        /// 表示标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 表示副标题
        /// </summary>
        public string Title2 { get; set; }
        /// <summary>
        /// 表示关键字信息
        /// </summary>
        public string KeyWords { get; set; }
        /// <summary>
        /// 表示摘要信息
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 表示作者
        /// </summary>
        public string Author { get; set; }

        #region 来源
        /// <summary>
        /// 来源名称
        /// </summary>
        public string FromName { get; set; }
        /// <summary>
        /// 来源Url
        /// </summary>
        public string FromUrl { get; set; }
        #endregion

        #region 数据统计
        /// <summary>
        /// 收藏次数
        /// </summary>
        public int FavoriteCount { get; set; }

        /// <summary>
        /// 基础收藏次数(初始化用，造假用）
        /// </summary>
        public int BaseFavoriteCount { get; set; }
        /// <summary>
        /// 浏览次数
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// 基础浏览次数(初始化用，造假用）
        /// </summary>
        public int BaseViewCount { get; set; }
        /// <summary>
        /// 点赞次数
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 基础点赞次数(初始化用，造假用)
        /// </summary>
        public int BaseLikeCount { get; set; }
        /// <summary>
        /// 不喜欢次数
        /// </summary>
        public int UnLikeCount { get; set; }

        /// <summary>
        /// 基础不喜欢次数(初始化用，造假用)
        /// </summary>
        public int BaseUnLikeCount { get; set; }
        /// <summary>
        /// 分享的次数
        /// </summary>
        public int ShareCount { get; set; }
        /// <summary>
        /// 基础分享次数(初始化用，造假用)
        /// </summary>
        public int BaseShareCount { get; set; }
        /// <summary>
        /// 回复的次数
        /// </summary>
        public int ReplyCount { get; set; }
        /// <summary>
        /// 基础回复的次数(造假用)
        /// </summary>
        public int BaseReplyCount { get; set; }
        #endregion

        #region 复杂属性
        public virtual List<ContentImage> TitleImages { get; set; }
        /// <summary>
        /// 关联图片信息
        /// </summary>
        public virtual List<ContentImage> Images { get; set; }

        /// <summary>
        /// 表示关联的附件
        /// </summary>
        public virtual List<ContentAttachment> Attachments { get; set; }
        /// <summary>
        /// 表示关联的视频信息
        /// </summary>
        public virtual List<ContentVideo> Video { get; set; }

        /// <summary>
        /// 表示关联的文章内容 
        /// </summary>
        public virtual List<ContentArticle> Articles { get; set; }
        /// <summary>
        /// 表示消息答复的
        /// </summary>
        public virtual List<ContentReply> Replys { get; set; }

        #endregion

        #region 跟踪属性
        public string DeleteUser
        {
            get; set;
        }

        public DateTime? DeleteTime
        {
            get; set;
        }

        public bool IsDeleted
        {
            get; set;
        }

        /// <summary>
        /// 表示发布时间
        /// </summary>
        public DateTime? PublishTime { get; set; }

        public string PublishUser
        {
            get; set;
        }

        public string ResumeUser
        {
            get; set;
        }

        public DateTime? ResumeTime
        {
            get; set;
        }

        public DateTime? UpdateTime
        {
            get; set;
        }

        public string UpdateUser
        {
            get; set;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsFixedTop { get; set; }
        /// <summary>
        /// 是否属于热点
        /// </summary>
        public bool IsHot { get; set; }

        /// <summary>
        /// 是否属于原创
        /// </summary>
        public bool IsOriginal { get; set; }
        /// <summary>
        /// 是否属于最新
        /// </summary>
        public bool IsNew { get; set; }

        #endregion

    }

    public class ContentImage : BaseEntity, ISequence
    {
        public Guid ImageID { get; set; }

        public string ImageName { get; set; }
        public string ImageDescription { get; set; }
        public string ImageUrl { get; set; }

        public int Sequence
        {
            get; set;
        }
    }

    public class ContentAttachment : BaseEntity, ISequence
    {
        public Guid AttachmentID { get; set; }

        public string AttachmentName { get; set; }

        public string AttachmentDescription { get; set; }
        public Guid ContentID { get; set; }
        public string AttachmentUrl { get; set; }

        public int Sequence
        {
            get; set;
        }
    }

    public class ContentVideo : BaseEntity, ISequence
    {
        public Guid VideoID { get; set; }
        public Guid ContentID { get; set; }
        public string VedioName { get; set; }
        public string VedioDescription { get; set; }
        public int VideoSeconds { get; set; }

        /// <summary>
        /// 视频的截图
        /// </summary>
        public string VideoImageUrl { get; set; }

        /// <summary>
        /// 表示视频的地址
        /// </summary>
        public string VideoUrl { get; set; }
        public int Sequence
        {
            get; set;
        }
    }

    public class ContentArticle : BaseEntity, ISequence
    {
        public Guid ArticleID { get; set; }
        public Guid ContentID { get; set; }
        public string ArticleContent { get; set; }
        public int Sequence
        {
            get; set;
        }
    }

    /// <summary>
    /// 表示回复信息
    /// </summary>
    public class ContentReply : BaseEntity//, ISelfTree<Guid?>
    {
        public Guid ReplyID { get; set; }

        public Guid ContentID { get; set; }
        /// <summary>
        /// 表示答复的内容
        /// </summary>
        public string ReplyContent { get; set; }

        public Guid? ParentID
        {
            get; set;
        }

        public string ReplyUser { get; set; }
    }

    public class ContentConfig
    {
        public Guid ConfigID { get; set; }

        public string ConfigName { get; set; }

        public string ConfigDescription { get; set; }

        public string HasKeyWorks { get; set; }

        public string HasTitle2 { get; set; }

        public string HasTitle { get; set; }

        public List<ContentConfigProperty> Properties { get; set; }
    }
    public class ContentConfigProperty
    {
        public Guid PropertyID { get; set; }
        public Guid ConfigID { get; set; }

        public string PropertyName { get; set; }

        public string PropertyDescription { get; set; }

        public string PropertyType { get; set; }//String，Number，Date，Tel，Url，Email,Bool等

    }

    public enum CatagoryKind
    {
        /// <summary>
        /// 目录
        /// </summary>
        Folder = 0,
        /// <summary>
        /// 站群
        /// </summary>
        WebSiteGroup = 1,
        /// <summary>
        /// 网站
        /// </summary>
        WebSite = 11,
        /// <summary>
        /// 专题
        /// </summary>
        Subject = 12,
        /// <summary>
        /// 网站数据源
        /// </summary>
        DataSource = 4,
        /// <summary>
        /// 静态数据源
        /// </summary>
        StaticDataSource = 5,
        /// <summary>
        /// 动态数据源
        /// </summary>
        DynamicDataSource = 6,
        /// <summary>
        /// 表示界面
        /// </summary>
        UI = 7,
    }
}
