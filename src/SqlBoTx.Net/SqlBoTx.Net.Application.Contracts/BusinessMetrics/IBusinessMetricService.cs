using SqlBoTx.Net.Application.Contracts.BusinessMetrics.Dtos;

namespace SqlBoTx.Net.Application.Contracts.BusinessMetrics
{
    public interface IBusinessMetricService
    {
        Task<List<ListBusinessMetricDto>> ListAsync();
        Task AddAsync(AddBusinessMetricDto input);
        Task UpdateAsync(UpdateBusinessMetricDto input);
        Task DeleteAsync(int metricId);
    }
}
