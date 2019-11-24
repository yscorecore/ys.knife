using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Entity
{
    public static class EntityExtentions
    {
        public static void FillCreateInfo<EntityType>(this EntityType entity)
            where EntityType : class, ICreateTrack
        {
            if (entity != null)
            {
                entity.CreateTime = DateTime.Now;
                entity.CreateUser = EnvironmentEx.CurrentIdentityName;
            }
        }
        public static void FillUpdateInfo<EntityType>(this EntityType entity)
            where EntityType : class, IUpdateTrack
        {
            if (entity != null)
            {
                entity.UpdateTime = DateTime.Now;
                entity.UpdateUser = EnvironmentEx.CurrentIdentityName;
            }
        }
        public static void FillAuditInfo<EntityType>(this EntityType entity)
           where EntityType : class, IAuditTrack
        {
            if (entity != null)
            {
                entity.AuditTime = DateTime.Now;
                entity.AuditUser = EnvironmentEx.CurrentIdentityName;
            }
        }
        public static void FillPublishInfo<EntityType>(this EntityType entity)
          where EntityType : class, IPublishTrack
        {
            if (entity != null)
            {
                entity.PublishTime = DateTime.Now;
                entity.PublishUser = EnvironmentEx.CurrentIdentityName;
            }
        }
        public static void FillDeleteInfo<EntityType>(this EntityType entity)
               where EntityType : class, IDeleteTrack
        {
            if (entity != null)
            {
                entity.DeleteTime = DateTime.Now;
                entity.DeleteUser = EnvironmentEx.CurrentIdentityName;
            }
        }
        public static void FillResumeInfo<EntityType>(this EntityType entity)
        where EntityType : class, IResumeTrack
        {
            if (entity != null)
            {
                entity.ResumeTime = DateTime.Now;
                entity.ResumeUser = EnvironmentEx.CurrentIdentityName;
            }
        }

        public static void SetNextSendTime<EntityType>(this EntityType entity, ISendTimeBuilder func)
            where EntityType : ILoopRecord
        {
            if (entity == null) return;
            if (func == null)
                throw new ArgumentNullException("func");

            entity.NextSendTime = func.GetNextSendTime(entity.FailureCount);
        }
    }
}
