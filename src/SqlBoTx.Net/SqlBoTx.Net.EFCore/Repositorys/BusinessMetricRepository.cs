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
        public IQueryable<DomainMetric> IQueryable()
        {
            return _dbContext.Set<DomainMetric>();
        }

        /// <summary>
        /// Get BusinessMetric by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<DomainMetric?> GetByIdAsync(int id)
        {
            return await _dbContext.Set<DomainMetric>()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Get List of BusinessMetric
        /// </summary>
        /// <param name="includeFunc"></param>
        /// <returns></returns>
        public async Task<List<DomainMetric>> ListAsync(Func<IQueryable<DomainMetric>, IQueryable<DomainMetric>>? includeFunc = null)
        {
            var query = _dbContext.Set<DomainMetric>().AsQueryable();
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
        public async Task InsterAsync(DomainMetric entity)
        {
            await _dbContext.Set<DomainMetric>().AddAsync(entity);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task UpdateAsync(DomainMetric entity)
        {
            var existingEntity = await _dbContext.Set<DomainMetric>()
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
            await _dbContext.Set<DomainMetric>()
               .Where(x => x.Id == id)
               .ExecuteDeleteAsync();
        }
    }
}
