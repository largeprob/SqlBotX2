using SqlBoTx.Net.Domain.TableStructures;

namespace SqlBoTx.Net.Domain.BusinessMetrics
{
    public interface IBusinessMetricRepository
    {
        /// <summary>
        /// Get IQueryable
        /// </summary>
        /// <returns><see cref="IQueryable<BusinessMetric>"/></returns>
        IQueryable<BusinessObjectiveMetric> IQueryable();

        /// <summary>
        /// Get BusinessMetric by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BusinessObjectiveMetric?> GetByIdAsync(int id);

        /// <summary>
        /// Get List of BusinessMetric
        /// </summary>
        /// <param name="includeFunc"></param>
        /// <returns></returns>
        Task<List<BusinessObjectiveMetric>> ListAsync(Func<IQueryable<BusinessObjectiveMetric>, IQueryable<BusinessObjectiveMetric>>? includeFunc = null);

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task InsterAsync(BusinessObjectiveMetric entity);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task UpdateAsync(BusinessObjectiveMetric entity);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(int id);
    }
}
