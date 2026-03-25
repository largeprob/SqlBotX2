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


        #region 元数据
    
        /// <summary>
        /// 主键
        /// </summary>
        [VectorStoreData(IsIndexed = true)]
        public int MetaDataId { get; set; }

        /// <summary>
        /// 1维度2/时间/3度量/4属性
        /// </summary>
        public int? SemanticType { get; set; }
 
        /// <summary>
        /// 字段名称
        /// </summary>
        [VectorStoreData]
        public string? MetaDataName { get; set; }

        /// <summary>
        /// 字段说明
        /// </summary>
        [VectorStoreData]
        public string? MetaDataDescription { get; set; }

        #endregion

        /// <summary>
        /// 归属实体ID
        /// </summary>
        [VectorStoreData(IsIndexed = true)]
        public string EntityId { get; set; }

        /// <summary>
        /// 归属目标元数据
        /// </summary>
        [VectorStoreData(IsIndexed = true)]
        public int ObjectiveMetaDataId { get; set; }

        /// <summary>
        /// 归属目标元数据
        /// </summary>
        [VectorStoreData]
        public string? ObjectiveMetaDataName { get; set; }

        /// <summary>
        /// 字段名向量数据
        /// </summary>
        [VectorStoreVector(Dimensions: 1536, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Hnsw)]
        public ReadOnlyMemory<float>? Embedding { get; set; }
    }
}
