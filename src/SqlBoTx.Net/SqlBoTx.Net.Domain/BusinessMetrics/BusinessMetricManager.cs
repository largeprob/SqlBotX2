using SqlBoTx.Net.Share.Exceptions;

namespace SqlBoTx.Net.Domain.BusinessMetrics
{
    /// <summary>
    /// Manager 用来执行 BusinessMetric 相关的业务逻辑
    /// </summary>
    public class BusinessMetricManager
    {
        private readonly IBusinessMetricRepository _businessMetricRepository;

        public BusinessMetricManager(IBusinessMetricRepository businessMetricRepository)
        {
            _businessMetricRepository = businessMetricRepository;
        }

        /// <summary>
        /// 创建业务指标
        /// </summary>
        public async Task<DomainMetric> CreateAsync(DomainMetric input)
        {

            // 验证指标编码是否已存在
            var existingMetric = await _businessMetricRepository.ListAsync(q => q.Where(x => x.MetricCode == input.MetricCode));
            if (existingMetric.Count > 0)
            {
                throw new BusinessException("BusinessMetric006", $"指标编码 {input.MetricCode} 已存在");
            }

            // 设置创建时间
            input.CreatedDate = DateTime.Now;

            return input;
        }

        /// <summary>
        /// 更新业务指标
        /// </summary>
        public async Task<DomainMetric> UpdateAsync(DomainMetric input)
        {
            // 验证是否存在
            var existing = await _businessMetricRepository.GetByIdAsync(input.Id);
            if (existing == null)
            {
                throw new BusinessException("BusinessMetric012", "业务指标不存在");
            }

            // 验证指标编码是否已存在（排除自己）
            var existingMetric = await _businessMetricRepository.ListAsync(q => q.Where(x => x.MetricCode == input.MetricCode && x.Id != input.Id));
            if (existingMetric.Count > 0)
            {
                throw new BusinessException("BusinessMetric013", $"指标编码 {input.MetricCode} 已存在");
            }

            // 设置更新时间
            input.UpdatedDate = DateTime.Now;
 

            return input;
        }
    }
}
