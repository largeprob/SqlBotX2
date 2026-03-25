using SqlBoTx.Net.Domain.TableStructures;

namespace SqlBoTx.Net.Domain.BusinessMetrics
{
    public interface IBusinessMetricRepository
    {
        /// <summary>
        /// Get IQueryable
        /// </summary>
        /// <returns><see cref="IQueryable<BusinessMetric>"/></returns>
        IQueryable<DomainMetric> IQueryable();

        /// <summary>
        /// Get BusinessMetric by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DomainMetric?> GetByIdAsync(int id);

        /// <summary>
        /// Get List of BusinessMetric
        /// </summary>
        /// <param name="includeFunc"></param>
        /// <returns></returns>
        Task<List<DomainMetric>> ListAsync(Func<IQueryable<DomainMetric>, IQueryable<DomainMetric>>? includeFunc = null);

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task InsterAsync(DomainMetric entity);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task UpdateAsync(DomainMetric entity);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(int id);
    }
}
