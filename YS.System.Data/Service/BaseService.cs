using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Store;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Service
{
    public partial class BaseService<EntityType,T>:ICURDAll<EntityType,T>
         where EntityType : class,IId<T>,new()
    {
        public BaseService(IEntityStore<EntityType> entityStore)
        {
            this.Store = ProcessStore(entityStore);
        }
        protected IEntityStore<EntityType> Store { get; set; }

        protected virtual IEntityStore<EntityType> ProcessStore(IEntityStore<EntityType> entityStore)
        {
            return new TrackStoreProxy<EntityType>(entityStore);
        }
        protected virtual Task<R> FromResult<R>(R res)
        {
            return Task.FromResult(res);
        }
        protected ResultData<TData> Success<TData>(TData data)
        {
            return ResultInfos.Success.WrapData(data);
        }
        protected Task<ResultData<TData>> TaskSuccess<TData>(TData data)
        {
            return Task.FromResult(Success(data));
        }
        protected ResultData<TData> Failure<TData>(ResultInfo resultInfo, TData data = default(TData))
        {
            return resultInfo.WrapData(data);
        }
        protected Task<ResultData<TData>> TaskFailure<TData>(ResultInfo resultInfo, TData data = default(TData))
        {
            return Task.FromResult(Failure(resultInfo, data));
        }
        #region CreateNew
        public virtual Task<ResultData<EntityType>> Create()
        {
            var instance = new EntityType();
            return this.TaskSuccess(instance);
        }
        #endregion
        #region Add
        public async virtual Task<ResultData<EntityType>> Add(EntityType entity)
        {
            var res = await this.OnBeforeAddEntity(entity);
            if (!res)
            {
                return res.WrapData<EntityType>(null);//添加失败不返回数据
            }
            await this.OnFillAddEntityPropertys(entity);//填充必要的属性
            this.Store.Add(entity);
            if (this.Store.SaveChanges() > 0)
            {
                await this.OnAfterAddEntity(entity);
                return this.Success(entity);
            }
            else
            {
                return ResultInfos.OperaterFailure.WrapData<EntityType>(null);//添加失败不返回数据，此处是为了安全性考虑，FillAddEntityProperty 后如果返回则会暴漏服务器的一些规则
            }


        }

        protected virtual Task<ResultInfo> OnBeforeAddEntity(EntityType entity)
        {
            return Task.FromResult(ResultInfos.True);
        }

        protected virtual Task OnAfterAddEntity(EntityType entity)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnFillAddEntityPropertys(EntityType entity)
        {
            return Task.CompletedTask;
        }
        #endregion
        #region Delete
        public virtual async Task<ResultInfo> DeleteById(T id)
        {
            return await DeleteEntity(id);
        }
        private async Task<ResultInfo> DeleteEntity(T id)
        {
            var entity = this.Store.FindByKey(id);
            if (entity == null) return ResultInfos.NotFound;
            var res = await this.OnBeforeDeleteEntity(entity);
            if (res)
            {
                await this.OnDeleteRelationEntities(entity);
                this.Store.Delete(entity);
                if (this.Store.SaveChanges() > 0)
                {
                    await this.OnAfterDeleteEntity(entity);
                    return ResultInfos.Success;
                }
                else
                {
                    return ResultInfos.OperaterFailure;
                }
            }
            return res;
        }
        protected virtual Task OnAfterDeleteEntity(EntityType entity)
        {
            return Task.CompletedTask;
        }
        protected virtual Task<ResultInfo> OnBeforeDeleteEntity(EntityType entity)
        {
            return Task.FromResult(ResultInfos.True);
        }
        /// <summary>
        /// 删除关联的实体
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entity"></param>
        protected virtual Task OnDeleteRelationEntities(EntityType entity)
        {
            return Task.CompletedTask;
        }
        #endregion
        #region Batch
        public  virtual Task<ResultData<int>> BatchOperation(BatchData<EntityType> batchInfos)
        {
            batchInfos = batchInfos ?? new BatchData<EntityType>();
            this.Store.BatchOperation(batchInfos.Inserts, batchInfos.Updates, batchInfos.Deletes);
            var changeCount = this.Store.SaveChanges();
            return this.TaskSuccess(changeCount);
        }
        #endregion
        #region Count

        public virtual Task<int> Count(SearchCondition conditions)
        {
            var exp = conditions == null ? null :
               conditions.CreatePredicate<EntityType>();
            return this.FromResult(this.Store.Count(exp));
        }
        #endregion
        #region SelectById
        public virtual Task<EntityType> FindById(T id)
        {
            return this.FromResult(this.Store.FindByKey(id));
        }
        #endregion
        #region FindItem
        public virtual Task<EntityType> FindItem(SearchCondition condition)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            var item= this.Store.FindSingleOrDefault(condition.CreatePredicate<EntityType>());
            return this.FromResult(item);
        }
        #endregion
        #region ListLimit
        public virtual Task<LimitData<EntityType>> ListLimit(SearchCondition condition, OrderCondition orderCondition, int offset,int limit)
        {
            var res = this.Store.ListLimit(condition == null ? null : condition.CreatePredicate<EntityType>(),
               GetInternalOrderItems(orderCondition), offset, limit);
            return this.FromResult(res);

        }
        #endregion
        #region ListPage
        public virtual Task<PageData<EntityType>> ListPage(SearchCondition condition, OrderCondition orderCondition,int pageIndex,int pageSize)
        {
            var res = this.Store.ListPage(condition == null ? null : condition.CreatePredicate<EntityType>(),
                GetInternalOrderItems(orderCondition), pageIndex, pageSize);
            return this.FromResult(res);
        }
        #endregion
        #region ListAll
        public virtual Task<List<EntityType>> ListAll(SearchCondition condition, OrderCondition orderCondition)
        {
            var res = this.Store.ListAll(condition == null ? null : condition.CreatePredicate<EntityType>(),
                orderCondition==null?null:orderCondition.Items);
            return this.FromResult(res);
        }
        #endregion

        #region DefaultOrder
        protected virtual IEnumerable<OrderItem> OnGetDefaultOrderItems()
        {
            yield return new OrderItem()
            {
                FieldName = Exp<IId<string>>.FieldName(p => p.Id),
                OrderType = OrderType.ASC
            };
        }

        private List<OrderItem> GetInternalOrderItems(OrderCondition orderCondition)
        {
            if (orderCondition!=null && orderCondition.HasItems())
            {
                return orderCondition.Items;
            }
            List<OrderItem> resultList = new List<OrderItem>();
            var defaultItems = this.OnGetDefaultOrderItems();
            if (defaultItems != null)
            {
                resultList.AddRange(defaultItems);
            }
            if (resultList.Count > 0)
            {
                return resultList;
            }
            resultList.Add(new OrderItem()
            {
                FieldName = Exp<IId<string>>.FieldName(p => p.Id),
                OrderType = OrderType.ASC
            });
            return resultList;
        }
        #endregion

        #region Update
        public async virtual Task<ResultData<EntityType>> Update(EntityType entity)
        {
            var result = await this.OnBeforeUpdateEntity(entity);
            if (!result)
            {
                return result.WrapData<EntityType>(null);
            }
            await this.OnFillUpdateEntityPropertys(entity);//填充更新需要填充的字段，比如lastupdatetime，lastupdateuser等
            this.Store.Update(entity);
            if (this.Store.SaveChanges() > 0)
            {
                await this.OnAfterUpdateEntity(entity);
                return this.Success(entity);
            }
            else
            {
                return ResultInfos.OperaterFailure.WrapData<EntityType>(null);//更新失败不返回数据，为了程序安全性考虑
            }
        }

        protected virtual Task<ResultInfo> OnBeforeUpdateEntity(EntityType entity)
        {
            return Task.FromResult(ResultInfos.True);
        }
        protected virtual Task OnAfterUpdateEntity(EntityType entity)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnFillUpdateEntityPropertys(EntityType entity)
        {
            return Task.CompletedTask;
        }
    



        Task<ResultData<EntityType>> IUpdate<EntityType>.Update(EntityType entity)
        {
            throw new NotImplementedException();
        }




        #endregion

        #region Patch
        public async virtual Task<ResultData<EntityType>> Patch(EntityType entity, params string[] fields)
        {
            var result = await this.OnBeforeUpdateEntity(entity);
            if (!result)
            {
                return result.WrapData<EntityType>(null);
            }
            await this.OnFillUpdateEntityPropertys(entity);//填充更新需要填充的字段，比如lastupdatetime，lastupdateuser等
            this.Store.Update(entity, fields);
            if (this.Store.SaveChanges() > 0)
            {
                await this.OnAfterUpdateEntity(entity);
                return this.Success(entity);
            }
            else
            {
                return this.Failure<EntityType>(ResultInfos.OperaterFailure);//更新失败不返回数据，为了程序安全性考虑
            }
        }
        #endregion

    }
}
