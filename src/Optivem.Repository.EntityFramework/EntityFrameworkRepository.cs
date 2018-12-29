using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Optivem.Repository.EntityFramework
{
    // TODO: VC: Bring back

    /*
     * 
    public class EntityFrameworkRepository<TEntity, TId, TContext> : IRepository<TEntity, TId> 
        where TEntity : class, IEntity<TId> 
        where TContext : DbContext
     * 
     */


    public class EntityFrameworkRepository<TEntity, TId, TContext> : IRepository<TEntity, TId> 
        where TEntity : class 
        where TContext : DbContext
    {
        private readonly TContext context;
        private readonly DbSet<TEntity> set;

        public EntityFrameworkRepository(TContext context)
        {
            this.context = context;
            this.set = context.Set<TEntity>();
        }

        #region Read

        // TODO: VC: Optimize with AsNoTracking and check any other optimizations

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

        // TODO: VC: When adding, setting CreatedBy and CreatedDate

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
            // TODO: VC: Setting ModifiedDate and ModifiedBy

            // TODO: VC: Check this
            // set.Update(entity);

            // TODO: VC: If this does not work, then use set.Attach(entity), then set.Entry(entity).State = EntityState.Modified;

            // TODO: VC: This is from template
            context.Entry(entity).State = EntityState.Modified;
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            // TODO: VC: Check this
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
            // TODO: VC: should we return bool indicating if delete was done?

            set.Remove(entity);

            // TODO: VC: If this does not work, firstly check if the entry state is detached in that case attach the entity, and adter all this, call remove
        }

        public bool Delete(object[] id)
        {
            var entity = GetSingleOrDefault(id);

            if(entity == null)
            {
                return false;
            }

            Delete(entity);

            return true;
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            set.RemoveRange(entities);
        }

        public List<KeyValuePair<object[], bool>> DeleteRange(IEnumerable<object[]> ids)
        {
            // TODO: VC: Optimize
            // TODO: VC: Handle duplicate

            var results = new List<KeyValuePair<object[], bool>>();

            var entities = new List<TEntity>();

            foreach(object[] id in ids)
            {
                // var entity = GetSingleOrDefault(id);
                var deleted = Delete(id);
                results.Add(new KeyValuePair<object[], bool>(id, deleted));
            }

            return results;
        }

        public void DeleteRange(params TEntity[] entities)
        {
            set.RemoveRange(entities);
        }

        public List<KeyValuePair<object[], bool>> DeleteRange(params object[][] ids)
        {
            // TODO: VC: Optimize
            // TODO: VC: Handle duplicate

            var results = new List<KeyValuePair<object[], bool>>();

            var entities = new List<TEntity>();

            foreach (object[] id in ids)
            {
                var deleted = Delete(id);
                results.Add(new KeyValuePair<object[], bool>(id, deleted));
            }

            return results;
        }

        #endregion

        #region Additional

        public TEntity GetSingleOrDefault(TId id)
        {
            // TODO: VC: Improve
            return GetSingleOrDefault((object)id);
        }

        public Task<TEntity> GetSingleOrDefaultAsync(TId id)
        {
            // TODO: VC: Improve
            return GetSingleOrDefaultAsync((object)id);
        }

        public void DeleteRange(IEnumerable<TId> ids)
        {
            // TODO: VC: Improve
            DeleteRange((IEnumerable<object[]>) ids);
        }

        public void DeleteRange(params TId[] ids)
        {
            // TODO: VC: Improve
            // DeleteRange((object[][])ids);

            throw new NotImplementedException();
        }

        public bool GetExists(TId id)
        {
            // TODO: VC: Passing in expression for matching id, e.g. Expression<Func<TEntity, bool>> idFilter in the constructor
            // so that we could call something like return context.Suppliers.Any(e => e.SupplierId == id);
            var entity = GetSingleOrDefault(id);
            return entity != null;
        }

        public async Task<bool> GetExistsAsync(TId id)
        {
            // TODO: VC: Passing in expression for matching id, e.g. Expression<Func<TEntity, bool>> idFilter in the constructor
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
        
        #endregion
    }
}
