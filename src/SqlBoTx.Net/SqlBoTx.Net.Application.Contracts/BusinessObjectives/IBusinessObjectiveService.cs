using SqlBoTx.Net.Application.Contracts.BusinessObjectives.Dtos;

namespace SqlBoTx.Net.Application.Contracts.BusinessObjectives
{
    public interface IBusinessObjectiveService
    {
        Task<List<ListBusinessObjectiveDto>> ListAsync();
        Task AddAsync(AddBusinessObjectiveDto input);
        Task UpdateAsync(UpdateBusinessObjectiveDto input);
        Task DeleteAsync(int objectiveId);
        Task<ListBusinessObjectiveDto> FindAsync(int id);
       
    }
}
