using Microsoft.Extensions.VectorData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SqlBoTx.Net.Application.Contracts.BusinessObjectives.Embeddings
{
    /// <summary>
    /// 业务目标向量模型
    /// </summary>
    public class BusinessObjectiveEmbeddingModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        [VectorStoreKey]
        public ulong Id { get; set; }

        /// <summary>
        /// 业务主键
        /// </summary>
        [VectorStoreData(IsIndexed = true)]
        public int MetaDataId { get; set; }

        /// <summary>
        /// 业务名称
        /// </summary>
        [VectorStoreData]
        public string? MetaDataName { get; set; }

        /// <summary>
        /// 解释
        /// </summary>
        [VectorStoreData]
        public string? MetaDataDescription { get; set; }
 
        /// <summary>
        /// 向量
        /// </summary>
        [VectorStoreVector(Dimensions: 1536, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Hnsw)]
        public ReadOnlyMemory<float>? Embedding { get; set; }
    }

}
