using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using SqlBoTx.Net.Application.Contracts.BusinessObjectives;
using SqlBoTx.Net.Application.Contracts.BusinessObjectives.Dtos;
using SqlBoTx.Net.Application.Contracts.TableStructures.Dtos;
using SqlBoTx.Net.Domain;
using SqlBoTx.Net.Domain.BusinessObjectives;
using SqlBoTx.Net.Domain.BusinessObjectives.Events;
using SqlBoTx.Net.Domain.TableRelationships;
using Weasel.SqlServer.Tables;
using Wolverine;
using Wolverine.EntityFrameworkCore;

namespace SqlBoTx.Net.Application.BusinessObjectives
{
    /// <summary>
    /// Service 用来处理 BusinessObjective 相关的应用逻辑
    /// </summary>
    public class BusinessObjectiveService : IBusinessObjectiveService
    {
        private readonly IBusinessObjectiveRepository _businessObjectiveRepository;
        private readonly ITableStructureRepository _tableStructureRepository;
        private readonly BusinessObjectiveManager _businessObjectiveManager;
        private readonly IUnitOfWork  _unitOfWork;
        private readonly Kernel _kernel;
        private readonly IMessageBus _bus;
        private readonly IDbContextOutbox _outbox;

        public BusinessObjectiveService(IBusinessObjectiveRepository businessObjectiveRepository, ITableStructureRepository tableStructureRepository, BusinessObjectiveManager businessObjectiveManager, IUnitOfWork unitOfWork, Kernel kernel, IMessageBus bus, IDbContextOutbox outbox)
        {
            _businessObjectiveRepository = businessObjectiveRepository;
            _tableStructureRepository = tableStructureRepository;
            _businessObjectiveManager = businessObjectiveManager;
            _unitOfWork = unitOfWork;
            _kernel = kernel;
            _bus = bus;
            _outbox = outbox;
        }

        /// <summary>
        /// List all business objectives
        /// </summary>
        /// <returns></returns>
        public async Task<List<ListBusinessObjectiveDto>> ListAsync()
        {
            var list = await _businessObjectiveRepository.ListAsync();
            var result = list.Adapt<List<ListBusinessObjectiveDto>>();

            var tableIds = list.SelectMany(x => x.DependencyTables!).Select(x => x.TableId).Distinct().ToList();
            var tableList = await _tableStructureRepository.ListAsync((p) => p.Where(x => tableIds.Contains(x.TableId)));
         
            foreach (var item in result)
            {
                var thisTableIds = item.DependencyTables!.Select(x => x.TableId).Distinct();
                var thisTableList = tableList.Where(x => thisTableIds.Contains(x.TableId));
                item.DependencyTables = thisTableList.Adapt<List<ListTableStructureDto>>();
            }
            return result;
        }

        /// <summary>
        /// List all table structures with fields
        /// </summary>
        /// <returns></returns>
        public async Task<List<ListBusinessObjectiveDto>> ListByIdAsync(int[] ids)
        {
            var list = await _businessObjectiveRepository.ListAsync(q => q.Where(x => ids.Contains(x.Id)).Include(x => x.DependencyTables));

            //表
            var allTableIds = list.SelectMany(x => x.DependencyTables!).Select(x => x.TableId).Distinct().ToList();
            var tableList = (await _tableStructureRepository.ListAsync((p) => p.Where(x => allTableIds.Contains(x.TableId))))
            .Adapt<List<ListTableStructureDto>>().ToDictionary(x => x.TableId);

           
            return list.Select(x =>
            {
                //var dto = new ListBusinessObjectiveDto { Id = x.Id };
                var dto = x.Adapt<ListBusinessObjectiveDto>();
                if (x.DependencyTables != null && x.DependencyTables.Count > 0) 
                {
                    dto.DependencyTables = dto.DependencyTables
                    .Where(t => tableList.ContainsKey(t.TableId))
                    .Select(t => tableList[t.TableId]).ToList();
                }
                return dto;
            }).ToList();
        }

        /// <summary>
        /// Add a new business objective
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task AddAsync(AddBusinessObjectiveDto input)
        {
            var entity = input.Adapt<BusinessObjective>();
            entity = await _businessObjectiveManager.CreateAsync(entity);

            await using (var uow = await _unitOfWork.BeginTransactionAsync())
            {
                await _businessObjectiveRepository.InsterAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                await _bus.PublishAsync(new Upsert(entity.Id, entity.BusinessName, entity.Synonyms, entity.Description));
                await uow.CommitAsync();
            }
        }

        /// <summary>
        /// Update an existing business objective
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateAsync(UpdateBusinessObjectiveDto input)
        {
            var entity = input.Adapt<BusinessObjective>();
            entity = await _businessObjectiveManager.UpdateAsync(entity);
          
            await using (var uow = await _unitOfWork.BeginTransactionAsync())
            {
                await _businessObjectiveRepository.UpdateAsync(entity);
                await _bus.PublishAsync(new Upsert(entity.Id, entity.BusinessName, entity.Synonyms, entity.Description));
                await uow.CommitAsync();
            }
        }

        /// <summary>
        /// Delete a business objective
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            await _businessObjectiveRepository.DeleteAsync(id);
            await _bus.PublishAsync(new Delete(id));
        }

        /// <summary>
        /// Delete a business objective
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ListBusinessObjectiveDto> FindAsync(int id)
        {
            var data = 
            await _businessObjectiveRepository.FindAsync(id);

            return data.Adapt<ListBusinessObjectiveDto>();
        }
    }
}
