using Microsoft.Extensions.VectorData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBoTx.Net.Application.Contracts.BusinessObjectives.Embeddings
{
    public class BusinessObjectiveSynonymEmbeddingModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        [VectorStoreKey]
        public Guid Id { get; set; }

        /// <summary>
        /// 元数据
        /// </summary>
        public string? MataData { get; set; }

        /// <summary>
        /// 归属目标元数据
        /// </summary>
        [VectorStoreData]
        public BusinessObjectiveMataData? ObjectiveMataData { get; set; }

        /// <summary>
        /// 向量
        /// </summary>
        [VectorStoreVector(Dimensions: 1536, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Hnsw)]
        public ReadOnlyMemory<float>? Embedding { get; set; }
    }

   
}
