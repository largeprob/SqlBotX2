using Microsoft.EntityFrameworkCore;
using Mapster;
using SqlBoTx.Net.Application.Contracts.BusinessMetrics;
using SqlBoTx.Net.Application.Contracts.BusinessMetrics.Dtos;
using SqlBoTx.Net.Application.Contracts.TableStructures.Dtos;
using SqlBoTx.Net.Domain;
using SqlBoTx.Net.Domain.BusinessMetrics;
using SqlBoTx.Net.Domain.TableRelationships;

namespace SqlBoTx.Net.Application.BusinessMetrics
{
    /// <summary>
    /// Service 用来处理 BusinessMetric 相关的应用逻辑
    /// </summary>
    public class BusinessMetricService : IBusinessMetricService
    {
        private readonly IBusinessMetricRepository _businessMetricRepository;
        private readonly ITableStructureRepository _tableStructureRepository;
        private readonly BusinessMetricManager _businessMetricManager;
        private readonly IUnitOfWork _unitOfWork;

        public BusinessMetricService(
            IBusinessMetricRepository businessMetricRepository,
            ITableStructureRepository tableStructureRepository,
            BusinessMetricManager businessMetricManager,
            IUnitOfWork unitOfWork)
        {
            _businessMetricRepository = businessMetricRepository;
            _tableStructureRepository = tableStructureRepository;
            _businessMetricManager = businessMetricManager;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// List all business metrics
        /// </summary>
        /// <returns></returns>
        public async Task<List<ListBusinessMetricDto>> ListAsync()
        {
            var list = await _businessMetricRepository.ListAsync(q => q
                .Include(x => x.JoinPaths)
                .Include(x => x.MainTable)
                .Include(x => x.BusinessObjective));

            var result = list.Adapt<List<ListBusinessMetricDto>>();

            // 获取所有涉及的表ID
            var tableIds = new List<int>();
            foreach (var item in list)
            {
                if (item.MainTableId > 0)
                {
                    tableIds.Add(item.MainTableId);
                }
                if (item.JoinPaths != null)
                {
                    tableIds.AddRange(item.JoinPaths.Select(x => x.TableId));
                }
            }
            tableIds = tableIds.Distinct().ToList();

            if (tableIds.Count > 0)
            {
                var tableList = await _tableStructureRepository.ListAsync((p) => p.Where(x => tableIds.Contains(x.TableId)));

                // 填充 ListBusinessMetricJoinPathDto
                foreach (var item in result)
                {
                    if (item.JoinPaths != null)
                    {
                        foreach (var joinPath in item.JoinPaths)
                        {
                            var table = tableList.FirstOrDefault(x => x.TableId == joinPath.TableId);
                            if (table != null)
                            {
                                joinPath.Alias = joinPath.Alias;
                                joinPath.JoinType = joinPath.JoinType;
                                joinPath.OnCondition = joinPath.OnCondition;
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Add a new business metric
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task AddAsync(AddBusinessMetricDto input)
        {
            var entity = input.Adapt<BusinessObjectiveMetric>();
            entity = await _businessMetricManager.CreateAsync(entity);

            await using (var uow = await _unitOfWork.BeginTransactionAsync())
            {
                await _businessMetricRepository.InsterAsync(entity);
                await uow.CommitAsync();
            }
        }

        /// <summary>
        /// Update an existing business metric
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateAsync(UpdateBusinessMetricDto input)
        {
            var entity = input.Adapt<BusinessObjectiveMetric>();
            entity = await _businessMetricManager.UpdateAsync(entity);

            await using (var uow = await _unitOfWork.BeginTransactionAsync())
            {
                await _businessMetricRepository.UpdateAsync(entity);
                await uow.CommitAsync();
            }
        }

        /// <summary>
        /// Delete a business metric
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            await _businessMetricRepository.DeleteAsync(id);
        }
    }
}
