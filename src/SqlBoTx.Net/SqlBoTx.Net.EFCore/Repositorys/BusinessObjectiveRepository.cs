using Microsoft.EntityFrameworkCore;
using SqlBoTx.Net.Domain.BusinessObjectives;
using SqlBoTx.Net.Domain.TableStructures;

namespace SqlBoTx.Net.EFCore.Repositorys
{
    /// <summary>
    /// BusinessObjective repository
    /// </summary>
    public class BusinessObjectiveRepository : IBusinessObjectiveRepository
    {
        private readonly SqlBotxDBContext _dbContext;

        public BusinessObjectiveRepository(SqlBotxDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get IQueryable
        /// </summary>
        /// <returns><see cref="IQueryable<BusinessObjective>"/></returns>
        public IQueryable<BusinessObjective> IQueryable()
        {
            return _dbContext.Set<BusinessObjective>();
        }

        /// <summary>
        /// Get BusinessObjective by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BusinessObjective?> GetByIdAsync(int id)
        {
            return await _dbContext.Set<BusinessObjective>()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Get List of BusinessObjective
        /// </summary>
        /// <param name="includeFunc"></param>
        /// <returns></returns>
        public async Task<List<BusinessObjective>> ListAsync(Func<IQueryable<BusinessObjective>, IQueryable<BusinessObjective>>? includeFunc = null)
        {
            var query = _dbContext.Set<BusinessObjective>().AsQueryable();
            if (includeFunc != null)
            {
                query = includeFunc(query);
            }
            return await query.ToListAsync();
        }


        /// <summary>
        /// Get List of BusinessObjective
        /// </summary>
        /// <param name="includeFunc"></param>
        /// <returns></returns>
        public async Task<BusinessObjective> FindAsync(int id)
        {
            var data = await _dbContext.Set<BusinessObjective>().FindAsync(id);
            return data;
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task InsterAsync(BusinessObjective entity)
        {
            await _dbContext.Set<BusinessObjective>().AddAsync(entity);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task UpdateAsync(BusinessObjective entity)
        {
            var existingEntity = await _dbContext.Set<BusinessObjective>().FirstAsync(x => x.Id == entity.Id);
            _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            await _dbContext.Set<BusinessObjective>()
               .Where(x => x.Id == id)
               .ExecuteDeleteAsync();
        }
    }
}
