using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Store;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace System.Data.Service
{
    public class TrackStoreProxy<T> : IEntityStore<T>
             where T : class
    {
        public TrackStoreProxy(IEntityStore<T> store)
        {
            this.innnerStore = store;
        }
        IEntityStore<T> innnerStore;
        public void Add(T entity)
        {
            if (entity is ICreateTrack)
            {
                (entity as ICreateTrack).FillCreateInfo();
            }
            this.innnerStore.Add(entity);
        }

        public void Delete(T entity)
        {
            if (entity is IDeleteTrack)
            {
                (entity as IDeleteTrack).IsDeleted = true;
                (entity as IDeleteTrack).FillDeleteInfo();

                var names = System.Data.Exp<IDeleteTrack>.FieldNames(p => p.IsDeleted, p => p.DeleteTime, p => p.DeleteUser);
                this.innnerStore.Update(entity, names);
            }
            else if (entity is IFalseDelete)
            {
                (entity as IDeleteTrack).IsDeleted = true;
                var names = System.Data.Exp<IDeleteTrack>.FieldNames(p => p.IsDeleted);
                this.innnerStore.Update(entity, names);
            }
            else
            {
                this.innnerStore.Delete(entity);
            }
        }

        public T FindByKey(params object[] keyValues)
        {
            return this.innnerStore.FindByKey(keyValues);
        }

        public IQueryable<T> Query(Expression<Func<T, bool>> conditions)
        {
            if (typeof(IFalseDelete).IsAssignableFrom(typeof(T)))
            {
                var filterDeleted = System.Data.Exp<IFalseDelete>.CreateSearch(p => p.IsDeleted == false).CreatePredicate<T>();
                if (conditions == null)
                {
                    conditions = filterDeleted;
                }
                else
                {
                    conditions = conditions.AndAlso(filterDeleted);
                }
            }
            return this.innnerStore.Query(conditions);
        }

        public int SaveChanges()
        {
            return this.innnerStore.SaveChanges();
        }

        public void Update(T entity)
        {
            if (entity is IUpdateTrack)
            {
                (entity as IUpdateTrack).FillUpdateInfo();
            }
            this.innnerStore.Update(entity);
        }

        public void Update(T entity, params string[] fields)
        {
            if (entity is IUpdateTrack)
            {
                (entity as IUpdateTrack).FillUpdateInfo();
                var names = System.Data.Exp<IUpdateTrack>.FieldNames(p => p.UpdateTime, p => p.UpdateUser);
                fields = fields.Union(fields).ToArray();
            }
            this.innnerStore.Update(entity, fields);
        }

        public Func<int> GetSaveChangesMethod()
        {
            return this.innnerStore.GetSaveChangesMethod();
        }
    }
}
