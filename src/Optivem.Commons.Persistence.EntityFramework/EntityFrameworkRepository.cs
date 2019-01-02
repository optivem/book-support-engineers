using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Optivem.Commons.Persistence.EntityFramework
{
    public class EntityFrameworkRepository<TContext, TEntity, TId> : IRepository<TEntity, TId>
        where TContext : DbContext
        where TEntity : class
    {
        private readonly TContext context;
        private readonly DbSet<TEntity> set;

        public EntityFrameworkRepository(TContext context)
        {
            this.context = context;
            this.set = context.Set<TEntity>();
        }

        #region Read

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null, int? take = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var query = GetQuery(filter, orderBy, skip, take, includes);
            return query.ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null, int? take = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var query = GetQuery(filter, orderBy, skip, take, includes);
            return await query.ToListAsync();
        }

        public TEntity GetSingle(Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = GetQuery(filter, includes);
            return query.Single();
        }

        public Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = GetQuery(filter, includes);
            return query.SingleAsync();
        }

        public TEntity GetSingleOrDefault(Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = GetQuery(filter, includes);
            return query.SingleOrDefault();
        }

        public Task<TEntity> GetSingleOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = GetQuery(filter, includes);
            return query.SingleOrDefaultAsync();
        }

        public TEntity GetFirst(Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = GetQuery(filter, includes);
            return query.First();
        }

        public Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = GetQuery(filter, includes);
            return query.FirstAsync();
        }

        public TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = GetQuery(filter, includes);
            return query.FirstOrDefault();
        }

        public Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = GetQuery(filter, includes);
            return query.FirstOrDefaultAsync();
        }

        public TEntity GetSingleOrDefault(params object[] id)
        {
            return set.Find(id);
        }

        public Task<TEntity> GetSingleOrDefaultAsync(params object[] id)
        {
            return set.FindAsync(id);
        }

        public long GetCount(Expression<Func<TEntity, bool>> filter = null)
        {
            var query = GetQuery(filter);
            return query.LongCount();
        }

        public Task<long> GetCountAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            var query = GetQuery(filter);
            return query.LongCountAsync();
        }

        public bool GetExists(Expression<Func<TEntity, bool>> filter = null)
        {
            var query = GetQuery(filter);
            return query.Any();
        }
        
        public Task<bool> GetExistsAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            var query = GetQuery(filter);
            return query.AnyAsync();
        }

        #endregion

        #region Create
        
        public virtual void Add(TEntity entity)
        {
            set.Add(entity);
        }

        public Task AddAsync(TEntity entity)
        {
            return set.AddAsync(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            set.AddRange(entities);
        }

        public Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            return set.AddRangeAsync(entities);
        }

        public void AddRange(params TEntity[] entities)
        {
            set.AddRange(entities);
        }

        public Task AddRangeAsync(params TEntity[] entities)
        {
            return set.AddRangeAsync(entities);
        }

        #endregion

        #region Update

        public void Update(TEntity entity)
        {
            set.Update(entity);
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            set.UpdateRange(entities);
        }

        public void UpdateRange(params TEntity[] entities)
        {
            set.UpdateRange(entities);
        }

        #endregion

        #region Delete

        public void Delete(TEntity entity)
        {
            set.Remove(entity);
        }

        public void Delete(object[] id)
        {
            var entity = GetSingleOrDefault(id);

            if(entity == null)
            {
                return;
            }

            Delete(entity);
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            set.RemoveRange(entities);
        }

        public void DeleteRange(IEnumerable<object[]> ids)
        {
            var entities = GetEntities(ids);

            DeleteRange(entities);
        }


        public void DeleteRange(params TEntity[] entities)
        {
            set.RemoveRange(entities);
        }

        public void DeleteRange(params object[][] ids)
        {
            var entities = GetEntities(ids);

            DeleteRange(entities);
        }

        #endregion

        #region Additional

        public TEntity GetSingleOrDefault(TId id)
        {
            return set.Find(id);
        }

        public Task<TEntity> GetSingleOrDefaultAsync(TId id)
        {
            return set.FindAsync(id);
        }

        public void DeleteRange(IEnumerable<TId> ids)
        {
            var entities = GetEntities(ids);
            set.RemoveRange(entities);
        }

        public void DeleteRange(params TId[] ids)
        {
            var entities = GetEntities(ids);
            set.RemoveRange(entities);
        }

        public bool GetExists(TId id)
        {
            var entity = GetSingleOrDefault(id);
            return entity != null;
        }

        public async Task<bool> GetExistsAsync(TId id)
        {
            var entity = await GetSingleOrDefaultAsync(id);
            return entity != null;
        }

        #endregion

        #region Helper

        protected virtual IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var query = set.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip != null)
            {
                query = query.Skip(skip.Value);
            }

            if (take != null)
            {
                query = query.Take(take.Value);
            }

            return query;
        }

        protected virtual IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> filter = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return GetQuery(filter, null, null, null, includes);
        }


        protected IEnumerable<TEntity> GetEntities<T>(IEnumerable<T> ids)
        {
            var entities = new List<TEntity>();

            foreach (var id in ids)
            {
                var entity = GetSingleOrDefault(id);

                if (entity != null)
                {
                    entities.Add(entity);
                }
            }

            return entities;
        }

        #endregion
    }
}
