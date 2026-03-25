using SqlBoTx.Net.Share.Exceptions;

namespace SqlBoTx.Net.Domain.BusinessObjectives
{
    /// <summary>
    /// Manager 用来执行 BusinessObjective 相关的业务逻辑
    /// </summary>
    public class BusinessObjectiveManager
    {
        private readonly IBusinessObjectiveRepository _businessObjectiveRepository;

        public BusinessObjectiveManager(IBusinessObjectiveRepository businessObjectiveRepository)
        {
            _businessObjectiveRepository = businessObjectiveRepository;
        }

        /// <summary>
        /// 创建业务目标
        /// </summary>
        public async Task<BusinessObjective> CreateAsync(BusinessObjective input)
        {
            // 业务规则验证
            if (string.IsNullOrWhiteSpace(input.BusinessName))
            {
                throw new BusinessException("BusinessObjective001", "业务域名称不能为空");
            }

         


            input.CreatedDate = DateTime.Now;
            return input;
        }

        /// <summary>
        /// 更新业务目标
        /// </summary>
        public async Task<BusinessObjective> UpdateAsync(BusinessObjective input)
        {
            // 业务规则验证
            if (string.IsNullOrWhiteSpace(input.BusinessName))
            {
                throw new BusinessException("BusinessObjective003", "业务名称不能为空");
            }

      

            // 验证是否存在
            var existing = await _businessObjectiveRepository.GetByIdAsync(input.Id);
            if (existing == null)
            {
                throw new BusinessException("BusinessObjective005", "业务目标不存在");
            }

            input.UpdatedDate = DateTime.Now;
            return input;
        }
    }
}
