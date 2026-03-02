using Microsoft.Extensions.VectorData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SqlBoTx.Net.Application.Contracts.BusinessObjectives.Embeddings
{
    public class BusinessObjectiveFieldEmbeddingModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        [VectorStoreKey]
        public ulong Id { get; set; }

        /// <summary>
        /// 元数据
        /// </summary>
        [VectorStoreData]
        public BusinessObjectiveFieldMataData? MataData { get; set; }

        /// <summary>
        /// 元数据
        /// </summary>
        [VectorStoreData]
        public BusinessObjectiveMataData? ObjectiveMataData { get; set; }

        /// <summary>
        /// 字段名向量数据
        /// </summary>
        [VectorStoreVector(Dimensions: 1536, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Hnsw)]
        public ReadOnlyMemory<float>? Embedding { get; set; }
    }


    public class BusinessObjectiveFieldMataData
    {
        /// <summary>
        /// 主键
        /// </summary>
        [VectorStoreData(IsIndexed = true)]
        public int Id { get; set; }

        /// <summary>
        /// 类型：field:普通字段、calculated:指标
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 字段说明
        /// </summary>
        public string? Description { get; set; }
    }
}
