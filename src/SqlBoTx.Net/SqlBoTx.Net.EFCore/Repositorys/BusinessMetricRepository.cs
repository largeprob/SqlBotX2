using Microsoft.EntityFrameworkCore;
using SqlBoTx.Net.Domain.BusinessMetrics;
using SqlBoTx.Net.Domain.TableStructures;

namespace SqlBoTx.Net.EFCore.Repositorys
{
    /// <summary>
    /// BusinessMetric repository
    /// </summary>
    public class BusinessMetricRepository : IBusinessMetricRepository
    {
        private readonly SqlBotxDBContext _dbContext;

        public BusinessMetricRepository(SqlBotxDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get IQueryable
        /// </summary>
        /// <returns><see cref="IQueryable<BusinessMetric>"/></returns>
        public IQueryable<BusinessObjectiveMetric> IQueryable()
        {
            return _dbContext.Set<BusinessObjectiveMetric>();
        }

        /// <summary>
        /// Get BusinessMetric by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BusinessObjectiveMetric?> GetByIdAsync(int id)
        {
            return await _dbContext.Set<BusinessObjectiveMetric>()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Get List of BusinessMetric
        /// </summary>
        /// <param name="includeFunc"></param>
        /// <returns></returns>
        public async Task<List<BusinessObjectiveMetric>> ListAsync(Func<IQueryable<BusinessObjectiveMetric>, IQueryable<BusinessObjectiveMetric>>? includeFunc = null)
        {
            var query = _dbContext.Set<BusinessObjectiveMetric>().AsQueryable();
            if (includeFunc != null)
            {
                query = includeFunc(query);
            }
            return await query.ToListAsync();
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task InsterAsync(BusinessObjectiveMetric entity)
        {
            await _dbContext.Set<BusinessObjectiveMetric>().AddAsync(entity);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task UpdateAsync(BusinessObjectiveMetric entity)
        {
            var existingEntity = await _dbContext.Set<BusinessObjectiveMetric>()
                .Include(x => x.JoinPaths)
                .FirstAsync(x => x.Id == entity.Id);
            _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            await _dbContext.Set<BusinessObjectiveMetric>()
               .Where(x => x.Id == id)
               .Include(x=>x.JoinPaths)
               .ExecuteDeleteAsync();
        }
    }
}
