using Microsoft.EntityFrameworkCore;

namespace SqlBoTx.Net.Domain.BusinessObjectives
{
    public interface IBusinessObjectiveRepository
    {
        /// <summary>
        /// Get IQueryable
        /// </summary>
        /// <returns><see cref="IQueryable<BusinessObjective>"/></returns>
        IQueryable<BusinessObjective> IQueryable();

        /// <summary>
        /// Get BusinessObjective by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BusinessObjective?> GetByIdAsync(int id);

        /// <summary>
        /// Get List of BusinessObjective
        /// </summary>
        /// <param name="includeFunc"></param>
        /// <returns></returns>
        Task<List<BusinessObjective>> ListAsync(Func<IQueryable<BusinessObjective>, IQueryable<BusinessObjective>>? includeFunc = null);

        /// <summary>
        /// Get List of BusinessObjective
        /// </summary>
        /// <param name="includeFunc"></param>
        /// <returns></returns>
        Task<BusinessObjective> FindAsync(int id);
       

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task InsterAsync(BusinessObjective entity);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task UpdateAsync(BusinessObjective entity);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(int id);
    }
}
