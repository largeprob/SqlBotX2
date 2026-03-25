using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using SqlBoTx.Net.Application.Contracts.BusinessObjectives.Embeddings;
using SqlBoTx.Net.Share.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlBoTx.Net.Application.Vectors
{
    public class QdrantVectorService
    {
        private IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;

        private const string CLIENT_IP = "192.168.0.100";
        public QdrantVectorStore? VectorStore { get; private set; }
        public QdrantClient? VectorClient { get; private set; }
        public QdrantVectorService(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
        {
            _embeddingGenerator = embeddingGenerator;
            try
            {
                VectorClient = new QdrantClient(CLIENT_IP);
                VectorStore = new QdrantVectorStore(VectorClient, ownsClient: true);
            }
            catch (Exception ex)
            {
                throw new BusinessException("QdrantVector001", "实例化Qdrant失败");
            }
        }

        /// <summary>
        /// 批处理
        /// 有的模型限制批处理长度不能大于10
        /// </summary>
        /// <param name="texts"></param>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ReadOnlyMemory<float>>> Embedding(IEnumerable<string> texts, int dimensions = 1536)
        {
            var queryEmbedding = await _embeddingGenerator.GenerateAsync(texts, new EmbeddingGenerationOptions
            {
                Dimensions = dimensions,
            });
            return queryEmbedding.Select(x => x.Vector);
        }

        public async Task<ReadOnlyMemory<float>> Embedding(string text, int dimensions = 1536)
        {
            var queryEmbedding = await _embeddingGenerator.GenerateAsync(text, new EmbeddingGenerationOptions
            {
                Dimensions = dimensions,
            });
            return queryEmbedding.Vector;
        }

        /// <summary>
        /// 获取向量集合
        /// </summary>
        /// <returns></returns>
        public async Task<QdrantCollection<TKey, TRecord>> EnsureCollectionAsync<TKey,TRecord>(string name)
            where TKey : IEquatable<TKey>
            where TRecord : class 
        {
            var collection = VectorStore.GetCollection<TKey, TRecord>(name);
            await collection.EnsureCollectionExistsAsync();
            return collection;
        }

        /// <summary>
        /// 获取向量集合
        /// </summary>
        /// <returns></returns>
        public async Task<List<VectorSearchResult<TRecord>>> SearchAsync<TKey, TRecord>(string name, string text, Expression<Func<TRecord, bool>>? filter = null, Expression<Func<TRecord, object?>>? targetVectorField = null, int top = 10)
            where TKey : IEquatable<TKey>
            where TRecord : class
        {
            var collection = await EnsureCollectionAsync<TKey, TRecord>(name);
            var queryEmbedding = await Embedding(text);
            var results = collection.SearchAsync(queryEmbedding, top, new VectorSearchOptions<TRecord>
            {
                VectorProperty = targetVectorField != null ? targetVectorField : default,
                Filter = filter != null ? filter : default
            });
            return await results.ToListAsync();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public async Task UpdateAsync<TKey, TRecord>(string name, TRecord record)
            where TKey : IEquatable<TKey>
            where TRecord : class
        {
            var collection = await EnsureCollectionAsync<TKey, TRecord>(name);
            await collection.UpsertAsync(record);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public async Task UpdateAsync<TKey, TRecord>(string name, IEnumerable<TRecord> records)
            where TKey : IEquatable<TKey>
            where TRecord : class
        {
            var collection = await EnsureCollectionAsync<TKey, TRecord>(name);
            await collection.UpsertAsync(records);
        }
    }
}
