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
        /// 类型：field:普通字段、calculated:指标
        /// </summary>
        public string? MetaDataType { get; set; }

        /// <summary>
        /// 业务角色类型
        /// </summary>
        [VectorStoreData]
        public int? MetaDataBusinesBIRole { get; set; }

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
        /// 归属目标元数据
        /// </summary>
        [VectorStoreData]
        public string? ObjectiveMetaDataDescription { get; set; }

        /// <summary>
        /// 字段名向量数据
        /// </summary>
        [VectorStoreVector(Dimensions: 1536, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Hnsw)]
        public ReadOnlyMemory<float>? Embedding { get; set; }
    }
}
